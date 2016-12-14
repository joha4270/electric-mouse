using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.Relations;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using electric_mouse.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace electric_mouse.Services
{
    public class RouteService : IRouteService
    {
        private readonly ApplicationDbContext _dbContext;

        public RouteService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a route to the database. It parses the given date and gets the difficulty based on the difficultyId. The date and difficulty is added to the route.
        /// </summary>
        /// <param name="routeToAdd">This is the route to add.</param>
        /// <param name="date">The date the route was added.</param>
        /// <param name="difficultyId">The id of the difficulty of the route.</param>
        public void AddRoute(Route routeToAdd, string date, int? difficultyId)
        {
            if (string.IsNullOrEmpty(date) || difficultyId == null)
                return;

            routeToAdd.Date = ParseDate(date);
            routeToAdd.Difficulty = GetRouteDifficultyById(difficultyId);

            _dbContext.Routes.Add(routeToAdd);
            _dbContext.SaveChanges();
        }

        private DateTime ParseDate(string date) => DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        /// <summary>
        /// Adds the given route to the given sections by their sectionId.
        /// </summary>
        /// <param name="route">The route to be added to the sections.</param>
        /// <param name="sectionIds">The ids of the sections where the route is to be added.</param>
        public void AddRouteToSections(Route route, List<int> sectionIds)
        {
            if (sectionIds == null)
                return;

            foreach (var sectionId in sectionIds)
            {
                RouteSection section = _dbContext.RouteSections.Include(h => h.RouteHall)
                                                               .First(s => s.RouteSectionID == sectionId);
                section.Routes.Add(new RouteSectionRelation
                {
                    RouteSection = section,
                    Route = route
                });
            }

            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds an attachment to the given route. The attachment consists a video url and relative paths to the images that was added.
        /// </summary>
        /// <param name="route">The route to where the attachment is added.</param>
        /// <param name="videoUrl">The video url added to the attachment.</param>
        /// <param name="relativeImagePaths">The relative image paths to the images on the server.</param>
        public void AddAttachment(Route route, string videoUrl, string[] relativeImagePaths)
        {
            RouteAttachment attachment = new RouteAttachment { VideoUrl = videoUrl, Route = route, RouteID = route.ID };
            _dbContext.RouteAttachments.Add(attachment);

            foreach (string relativeImagePath in relativeImagePaths)
            {
                _dbContext.AttachmentPathRelations.Add(new AttachmentPathRelation
                {
                    ImagePath = relativeImagePath,
                    RouteAttachment = attachment
                });
            }

            _dbContext.SaveChanges();
        }

        public async void UpdateImageAttachments(int attachmentId, params string[] relativeImagePaths)
        {
            RouteAttachment attachmentToAddImagesTo =
                _dbContext.RouteAttachments.First(attachment => attachment.RouteAttachmentID == attachmentId);

            foreach (string relativeImagePath in relativeImagePaths)
            {
                _dbContext.AttachmentPathRelations.Add(new AttachmentPathRelation
                {
                    ImagePath = relativeImagePath,
                    RouteAttachment = attachmentToAddImagesTo
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        public async void UpdateVideoUrlInAttachment(int attachmentId, string videoUrl)
        {
            RouteAttachment attachment = _dbContext.RouteAttachments.First(att => att.RouteAttachmentID == attachmentId);
            attachment.VideoUrl = videoUrl;

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all the route sections that are not archived.
        /// </summary>
        public List<RouteSection> GetAllActiveRouteSections() => _dbContext.RouteSections
                                                                     .Where(section => section.Archived == false)
                                                                     .ToList();

        // TODO: Move to SectionService
        public List<RouteSection> GetAllRouteSections() => _dbContext.RouteSections.ToList();

        /// <summary>
        /// Gets all the route halls that are not archived. It includes the hall's sections as well.
        /// </summary>
        public List<RouteHall> GetAllActiveRouteHalls() => _dbContext.RouteHalls
                                                                     .Include(hall => hall.Sections)
                                                                     .Where(hall => hall.Archived == false)
                                                                     .ToList();

        /// <summary>
        /// Gets all the route difficulties.
        /// </summary>
        public List<RouteDifficulty> GetAllRouteDifficulties() => _dbContext.RouteDifficulties
                                                                            .ToList();

        /// <summary>
        /// Gets a routeDifficulty by the specified id.
        /// </summary>
        public RouteDifficulty GetRouteDifficultyById(int? id) => _dbContext.RouteDifficulties
                                                                           .First(difficulty => difficulty.RouteDifficultyID == id);

        
        private async void AddBuilderToRoute(Route route, ApplicationUser builderToAdd)
        {
            _dbContext.RouteUserRelations.Add(new RouteApplicationUserRelation
            {
                User = builderToAdd,
                Route = route
            });
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Adds the given builders to the specified route.
        /// </summary>
        /// <param name="route">The route to which the builder is added.</param>
        /// <param name="buildersToAdd">The builders that is to be added to the route.</param>
        public void AddBuildersToRoute(Route route, IList<ApplicationUser> buildersToAdd)
        {
            buildersToAdd.ToList().ForEach(user => AddBuilderToRoute(route, user));
        }

        public Route GetRouteWithDifficultyById(int routeId) => _dbContext.Routes
                                                                          .Include(route => route.Difficulty)
                                                                          .First(route => route.ID == routeId);

        /// <summary>
        /// Gets all the routes and filters them based on it being archived, who its creator is and its type.
        /// </summary>
        /// <param name="archived"></param>
        /// <param name="creator"></param>
        /// <param name="type"></param>
        public List<Route> GetRoutesFiltered(bool archived, string creator, RouteType? type)
        {
            IQueryable<Route> routes = _dbContext.Routes
                                                 .Include(route => route.Difficulty)
                                                 .Include(route => route.Creators)
                                                 .ThenInclude(userRelation => userRelation.User)
                                                 .Where(route => route.Archived == archived);

            if (type != null)
                routes = routes.Where(route => route.Type == type);

            if (!string.IsNullOrEmpty(creator))
                routes = routes.Where(route => route.Creators
                                      .Any(userRelation => userRelation.ApplicationUserRefId == creator));

            List<Route> routeList = routes.ToList();
            routeList.ForEach(route => route.Sections = GetRouteSectionsByRouteId(route.ID));

            return routeList;
        }

        public List<Route> TruncateRouteNotes(List<Route> routes, int maxNoteLength)
        {
            foreach (Route route in routes)
            {
                if (route.Note != null && route.Note.Length >= maxNoteLength)
                    route.Note = $"{new string(route.Note.Take(maxNoteLength).ToArray())}...";
            }

            return routes;
        }

        // TODO: move this to SectionService and use it from that instead
        private List<RouteSection> GetRouteSectionsByRouteId(int routeId) =>
            _dbContext.RouteSectionRelations
                      .Include(relation => relation.RouteSection)
                      .Where(relation => relation.RouteID == routeId)
                      .Select(relation => relation.RouteSection).ToList();

        public async void RemoveRoute(int routeId)
        {
            Route routeToDelete = GetRouteById(routeId);
            if (routeToDelete.Archived)
                _dbContext.Routes.Remove(routeToDelete);

            await _dbContext.SaveChangesAsync();
        }

        public async void ActivateRoute(int routeId)
        {
            Route routeToMakeActive = GetRouteById(routeId);
            if (routeToMakeActive.Archived)
            {
                routeToMakeActive.Archived = false;
                _dbContext.Routes.Update(routeToMakeActive);
                await _dbContext.SaveChangesAsync();
            }
        }

        public Route GetRouteById(int routeId) => _dbContext.Routes.First(route => route.ID == routeId);

        /// <summary>
        /// Gets the route by id async. Returns null if the route was not found.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        public async Task<Route> GetRouteByIdAsync(int routeId) => await _dbContext.Routes.FirstOrDefaultAsync(route => route.ID == routeId);

        /// <summary>
        /// Checks whether the route is created by the specified user. It searches through the RouteUserRelations to find a matching routeId and userId.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        /// <param name="userId">The id of the user.</param>
        public async Task<bool> IsRouteCreatedByUser(int routeId, string userId)
        {
            return await _dbContext.RouteUserRelations
                                   .AnyAsync(relation => relation.RouteRefId == routeId
                                                         && relation.ApplicationUserRefId == userId);
        }
        
        /// <summary>
        /// Gets the routeHall by its hallId.
        /// </summary>
        public RouteHall GetRouteHallById(int hallId) => _dbContext.RouteHalls.First(p => p.RouteHallID == hallId);

        /// <summary>
        /// Gets the routeSection that the route is in.
        /// </summary>
        public RouteSection GetRouteSectionThatRouteIsIn(int routeId)
        {
            RouteSectionRelation rs = _dbContext.RouteSectionRelations.First(t => t.RouteID == routeId);

            return _dbContext.RouteSections.First(t => rs.RouteSectionID == t.RouteSectionID);
        }

        /// <summary>
        /// Gets all the users that the route was created by.
        /// </summary>
        /// <param name="routeId">The route id.</param>
        public List<ApplicationUser> GetRouteCreators(int routeId)
        {
            return _dbContext.RouteUserRelations
                             .Where(relation => relation.Route.ID == routeId)
                             .Select(relation => relation.User)
                             .ToList();
        }

        //TODO: Move to CommentService
        public List<Comment> GetCommentsInRoute(int routeId)
        {
            return _dbContext.Comments
                             .Where(comment => comment.RouteID == routeId)
                             .ToList();
        }

        //TODO: Move to AttachmentService
        public string[] GetAllImagePathsInRoute(int routeId)
        {
            AttachmentPathRelation[] attachments =
                _dbContext.AttachmentPathRelations
                          .Include(relation => relation.RouteAttachment)
                          .Where(attachment => attachment.RouteAttachment.RouteID == routeId)
                          .ToArray();

            return attachments?.Select(attachment => attachment.ImagePath)
                              .ToArray();
        }

        //TODO: Move to AttachmentService
        public string GetVideoUrlInRoute(int routeId)
        {
            return _dbContext.RouteAttachments.First(att => att.RouteID == routeId).VideoUrl;
        }

        //TODO: Move to AttachmentService
        /// <summary>
        /// Gets the attachment connected to the route. Is null if none is found.
        /// </summary>
        /// <param name="routeId">The route id.</param>
        public RouteAttachment GetRouteAttachmentInRoute(int routeId)
        {
            return _dbContext.RouteAttachments.FirstOrDefault(att => att.RouteID == routeId);
        }

        // TODO: Move to UserService
        public ApplicationUser GetUserById(string userId)
        {
            return _dbContext.Users.First(u => u.Id == userId);
        }

        /// <summary>
        /// Archives the given route by id.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        public async void ArchiveRoute(int routeId)
        {
            Route route = await GetRouteByIdAsync(routeId);
            route.Archived = true;

            await _dbContext.SaveChangesAsync();
        }

        // TODO: Move to HallService
        public int GetRouteHallIdWhereThereRouteIs(int routeId)
        {
            return _dbContext.RouteSectionRelations
                                 .Include(rel => rel.RouteSection)
                                 .Where(rel => rel.RouteID == routeId)
                                 .Select(rel => rel.RouteSection.RouteHallID)
                                 .First();
        }

        // TODO: Move to SectionService
        public List<int> GetRouteSectionsIdsWhereRouteIs(int routeId)
        {
            return _dbContext.RouteSectionRelations
                                                   .Where(relation => relation.RouteID == routeId)
                                                   .Select(relation => relation.RouteSectionID)
                                                   .ToList();
        }


        /// <summary>
        /// Gets all image paths of the images related to the attachment of the route. The paths are tupled with their attachment id.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        // TODO: Move to AttachmentService
        public Tuple<string, int>[] GetImagePathsWithIds(int routeId)
        {
            List<AttachmentPathRelation> relations = _dbContext.AttachmentPathRelations
                                                                .Include(relation => relation.RouteAttachment)
                                                                .Where(relation => relation.RouteAttachment.RouteID == routeId)
                                                                .ToList();

            IEnumerable<Tuple<string, int>> imagePathsWithIds =
                relations?.Select(
                    attachment => new Tuple<string, int>(attachment.ImagePath, attachment.AttachmentPathRelationID));

            return imagePathsWithIds.ToArray();
        }

        /// <summary>
        /// Removes the given sections from the route, by removing the section relations.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        /// <param name="sectionsToRemove">The ids of the sections that are to be removed.</param>
        public async void RemoveSectionsFromRoute(int routeId, IList<int> sectionsToRemove)
        {
            IQueryable<RouteSectionRelation> sectionsRelatedToRoute =
                GetSectionRelationsRelatedToRoute(routeId);

            List<RouteSectionRelation> sectionsToRemoveFromRoute = await sectionsRelatedToRoute.
                Where(relation => !sectionsToRemove.Contains(relation.RouteSectionID)).ToListAsync();

            _dbContext.RouteSectionRelations.RemoveRange(sectionsToRemoveFromRoute);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Adds the given sections to the route.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        /// <param name="sectionsToAdd">The ids of the sections that are to be added.</param>
        public async void AddSectionToRoute(int routeId, IList<int> sectionsToAdd)
        {
            IQueryable<RouteSectionRelation> sectionsRelatedToRoute =
                GetSectionRelationsRelatedToRoute(routeId);

            List<int> sectionsToAddToRoute = sectionsToAdd
                .Where(id => !sectionsRelatedToRoute.Any(relation => relation.RouteSectionID == id))
                .ToList();

            List<RouteSectionRelation> relationsToAdd = sectionsToAddToRoute.Select(sectionId => new RouteSectionRelation
            {
                Route = GetRouteById(routeId),
                RouteSectionID = sectionId
            }).ToList();

            _dbContext.RouteSectionRelations.AddRange(relationsToAdd);
            await _dbContext.SaveChangesAsync();
        }

        public async void RemoveBuildersFromRoute(int routeId, IList<string> buildersToRemove)
        {
            IQueryable<RouteApplicationUserRelation> usersRelatedToRoute =
                GetUsersRelatedToRoute(routeId);

            List<RouteApplicationUserRelation> usersToRemove = usersRelatedToRoute
                    .Where(relation => !buildersToRemove.Contains(relation.ApplicationUserRefId))
                    .ToList();

            _dbContext.RouteUserRelations.RemoveRange(usersToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public async void AddBuildersToRoute(int routeId, IList<string> buildersToAdd)
        {
            IQueryable<RouteApplicationUserRelation> usersRelatedToRoute =
                GetUsersRelatedToRoute(routeId);

            List<string> usersToAdd =
                buildersToAdd.Where(userId => !usersRelatedToRoute
                                 .Any(relation => relation.ApplicationUserRefId == userId))
                             .ToList();

            List<RouteApplicationUserRelation> relationsToAdd = usersToAdd
                .Select(userId => new RouteApplicationUserRelation
                {
                    Route = GetRouteById(routeId),
                    ApplicationUserRefId = userId
                }).ToList();

            _dbContext.RouteUserRelations.AddRange(relationsToAdd);
            await _dbContext.SaveChangesAsync();
        }

        private IQueryable<RouteApplicationUserRelation> GetUsersRelatedToRoute(int routeId)
        {
            return _dbContext.RouteUserRelations
                             .Where(relation => relation.RouteRefId == routeId);
        }

        private IQueryable<RouteSectionRelation> GetSectionRelationsRelatedToRoute(int routeId)
        {
            return _dbContext.RouteSectionRelations
                          .Where(relation => relation.RouteID == routeId);
        }

        /// <summary>
        /// Removes the image relation to the route and deletes the image on the server.
        /// </summary>
        /// <param name="imagePathRelationIds"></param>
        /// <param name="webRootPath"></param>
        // TODO: Move to AttachmentService
        public async void RemoveImagesFromRoute(string webRootPath, IList<int> imagePathRelationIds)
        {
            // Get the path relations that should be deleted
            List<AttachmentPathRelation> pathRelationsToRemove = _dbContext.AttachmentPathRelations
                .Where(relation => imagePathRelationIds.Contains(relation.AttachmentPathRelationID))
                .ToList();

            // Get the paths to the images on the server that should be deleted
            List<string> imagesOnServerToRemove = pathRelationsToRemove.Select(relation => relation.ImagePath).ToList();

            // Delete the path relations
            _dbContext.AttachmentPathRelations.RemoveRange(pathRelationsToRemove);
            await _dbContext.SaveChangesAsync();

            foreach (string path in imagesOnServerToRemove)
            {
                System.IO.File.Delete(Path.Combine(webRootPath, path));
            }
        }
    }
}

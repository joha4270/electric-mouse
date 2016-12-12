using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.Relations;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using Microsoft.EntityFrameworkCore;
using Models;

namespace electric_mouse.Services
{
    public class RouteService
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

            routeToAdd.Date = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            routeToAdd.Difficulty = GetRouteDifficultyById(difficultyId);

            _dbContext.Routes.Add(routeToAdd);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds the given route to the given sections by their sectionId.
        /// </summary>
        /// <param name="route">The route to be added to the sections.</param>
        /// <param name="sectionIds">The ids of the sections where the route is to be added.</param>
        public void AddRouteToSections(Route route, List<int> sectionIds)
        {
            if (sectionIds == null)
            {
                return;
            }

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

        public void UpdateAttachment(int routeId, string videoUrl, string[] relativeImagePaths)
        {
            
        }

        /// <summary>
        /// Gets all the route sections that are not archived.
        /// </summary>
        public List<RouteSection> GetAllActiveRouteSections() => _dbContext.RouteSections
                                                                     .Where(section => section.Archived == false)
                                                                     .ToList();

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

        
        private void AddBuilderToRoute(Route route, ApplicationUser builderToAdd)
        {
            _dbContext.RouteUserRelations.Add(new RouteApplicationUserRelation
            {
                User = builderToAdd,
                Route = route
            });
        }

        /// <summary>
        /// Adds the given builders to the specified route.
        /// </summary>
        /// <param name="route">The route to which the builder is added.</param>
        /// <param name="buildersToAdd">The builders that is to be added to the route.</param>
        public void AddBuildersToRoute(Route route, params ApplicationUser[] buildersToAdd)
        {
            buildersToAdd.ToList().ForEach(user => AddBuilderToRoute(route, user));
            _dbContext.SaveChanges();
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
                routes = routes.Where
                    (
                        route => route.Creators
                                      .Any(userRelation => userRelation.ApplicationUserRefId == creator)
                    );

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
    }
}

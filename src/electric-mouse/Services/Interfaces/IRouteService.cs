using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using electric_mouse.Models;
using electric_mouse.Models.RouteItems;
using Models;

namespace electric_mouse.Services.Interfaces
{
    public interface IRouteService
    {
        /// <summary>
        /// Adds a route to the database. It parses the given date and gets the difficulty based on the difficultyId. The date and difficulty is added to the route.
        /// </summary>
        /// <param name="routeToAdd">This is the route to add.</param>
        /// <param name="date">The date the route was added.</param>
        /// <param name="difficultyId">The id of the difficulty of the route.</param>
        void AddRoute(Route routeToAdd, string date, int? difficultyId);

        /// <summary>
        /// Adds the given route to the given sections by their sectionId.
        /// </summary>
        /// <param name="route">The route to be added to the sections.</param>
        /// <param name="sectionIds">The ids of the sections where the route is to be added.</param>
        void AddRouteToSections(Route route, List<int> sectionIds);

        /// <summary>
        /// Adds an attachment to the given route. The attachment consists a video url and relative paths to the images that was added.
        /// </summary>
        /// <param name="route">The route to where the attachment is added.</param>
        /// <param name="videoUrl">The video url added to the attachment.</param>
        /// <param name="relativeImagePaths">The relative image paths to the images on the server.</param>
        void AddAttachment(Route route, string videoUrl, string[] relativeImagePaths);

        void UpdateImageAttachments(int attachmentId, params string[] relativeImagePaths);
        void UpdateVideoUrlInAttachment(int attachmentId, string videoUrl);

        /// <summary>
        /// Gets all the route sections that are not archived.
        /// </summary>
        // TODO: Move to SectionService
        List<RouteSection> GetAllActiveRouteSections();

        // TODO: Move to SectionService
        List<RouteSection> GetAllRouteSections();

        /// <summary>
        /// Gets all the route halls that are not archived. It includes the hall's sections as well.
        /// </summary>
        // TODO: Move to HallService
        List<RouteHall> GetAllActiveRouteHalls();

        /// <summary>
        /// Gets all the route difficulties.
        /// </summary>
        // TODO: Move to DifficultyService
        List<RouteDifficulty> GetAllRouteDifficulties();

        /// <summary>
        /// Gets a routeDifficulty by the specified id.
        /// </summary>
        // TODO: Move to DifficultyService
        RouteDifficulty GetRouteDifficultyById(int? id);

        /// <summary>
        /// Adds the given builders to the specified route.
        /// </summary>
        /// <param name="route">The route to which the builder is added.</param>
        /// <param name="buildersToAdd">The builders that is to be added to the route.</param>
        void AddBuildersToRoute(Route route, params ApplicationUser[] buildersToAdd);

        Route GetRouteWithDifficultyById(int routeId);

        /// <summary>
        /// Gets all the routes and filters them based on it being archived, who its creator is and its type.
        /// </summary>
        /// <param name="archived"></param>
        /// <param name="creator"></param>
        /// <param name="type"></param>
        List<Route> GetRoutesFiltered(bool archived, string creator, RouteType? type);

        List<Route> TruncateRouteNotes(List<Route> routes, int maxNoteLength);
        void RemoveRoute(int routeId);
        void ActivateRoute(int routeId);
        Route GetRouteById(int routeId);

        /// <summary>
        /// Gets the route by id async. Returns null if the route was not found.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        Task<Route> GetRouteByIdAsync(int routeId);

        /// <summary>
        /// Checks whether the route is created by the specified user. It searches through the RouteUserRelations to find a matching routeId and userId.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        /// <param name="userId">The id of the user.</param>
        Task<bool> IsRouteCreatedByUser(int routeId, string userId);

        /// <summary>
        /// Gets the routeHall by its hallId.
        /// </summary>
        RouteHall GetRouteHallById(int hallId);

        /// <summary>
        /// Gets the routeSection that the route is in.
        /// </summary>
        RouteSection GetRouteSectionThatRouteIsIn(int routeId);

        /// <summary>
        /// Gets all the users that the route was created by.
        /// </summary>
        /// <param name="routeId">The route id.</param>
        List<ApplicationUser> GetRouteCreators(int routeId);

        List<Comment> GetCommentsInRoute(int routeId);
        string[] GetAllImagePathsInRoute(int routeId);
        string GetVideoUrlInRoute(int routeId);

        /// <summary>
        /// Gets the attachment connected to the route. Is null if none is found.
        /// </summary>
        /// <param name="routeId">The route id.</param>
        RouteAttachment GetRouteAttachmentInRoute(int routeId);

        ApplicationUser GetUserById(string userId);

        /// <summary>
        /// Archives the given route by id.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        void ArchiveRoute(int routeId);

        int GetRouteHallIdWhereThereRouteIs(int routeId);
        List<int> GetRouteSectionsIdsWhereRouteIs(int routeId);

        /// <summary>
        /// Gets all image paths of the images related to the attachment of the route. The paths are tupled with their attachment id.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        // TODO: Move to AttachmentService
        Tuple<string, int>[] GetImagePathsWithIds(int routeId);

        /// <summary>
        /// Removes the given sections from the route, by removing the section relations.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        /// <param name="sectionsToRemove">The ids of the sections that are to be removed.</param>
        void RemoveSectionsFromRoute(int routeId, params int[] sectionsToRemove);

        /// <summary>
        /// Adds the given sections to the route.
        /// </summary>
        /// <param name="routeId">The id of the route.</param>
        /// <param name="sectionsToAdd">The ids of the sections that are to be added.</param>
        void AddSectionToRoute(int routeId, params int[] sectionsToAdd);

        void RemoveBuildersFromRoute(int routeId, params string[] buildersToRemove);
        void AddBuildersToRoute(int routeId, params string[] buildersToAdd);

        /// <summary>
        /// Removes the image relation to the route and deletes the image on the server.
        /// </summary>
        /// <param name="imagePathRelationIds"></param>
        /// <param name="webRootPath"></param>
        // TODO: Move to AttachmentService
        void RemoveImagesFromRoute(string webRootPath, params int[] imagePathRelationIds);
    }
}
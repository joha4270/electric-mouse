using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using Microsoft.EntityFrameworkCore;

namespace electric_mouse.Services
{
    public class RouteService
    {
        private readonly ApplicationDbContext _dbContext;

        public RouteService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRoute(Route routeToAdd, string date, int? difficultyId)
        {
            if (string.IsNullOrEmpty(date) || difficultyId == null)
                return;

            routeToAdd.Date = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            routeToAdd.Difficulty = GetRouteDifficultyById(difficultyId);

            _dbContext.Routes.Add(routeToAdd);
            _dbContext.SaveChanges();
        }

        public void AddAttachment(Route route)
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
    }
}

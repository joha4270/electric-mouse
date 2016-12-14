using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using electric_mouse.Data;
using electric_mouse.Models.Relations;
using electric_mouse.Models.RouteItems;
using electric_mouse.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using electric_mouse.Services.Interfaces;

namespace electric_mouse.Services
{
    public class SectionService : ISectionService
    {
        private readonly ApplicationDbContext _dbContext;

        public SectionService(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Adds a new section to the database.
        /// </summary>
        /// <param name="name">Name of the section.</param>
        /// <param name="routeHallId">The id of the routeHall the section is in.</param>
        public void AddSection(string name, int? routeHallId)
        {
            RouteSection section = new RouteSection
            {
                Name = name,
                RouteHall = GetRouteHallById(routeHallId)
            };
            _dbContext.RouteSections.Add(section);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Archives a section if all the routes in the section are archived.
        /// </summary>
        /// <param name="sectionId">Id that matches the section to be archived.</param>
        public void ArchiveSection(int? sectionId)
        {
            RouteSection section =
                _dbContext.RouteSections.Include(s => s.Routes)
                          .ThenInclude(s => s.Route)
                          .First(s => s.RouteSectionID == sectionId);

            if (section.Routes.All(r => r.Route.Archived))
            {
                section.Archived = true;
                _dbContext.RouteSections.Update(section);
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all the route sections in the database. It includes the routeHalls, routeSectionRelations and the routes connected to these relations.
        /// </summary>
        public List<RouteSection> GetAllRouteSections() => _dbContext.RouteSections
                                                                     .Include(s => s.RouteHall)
                                                                     .Include(s => s.Routes)
                                                                     .ThenInclude(s => s.Route)
                                                                     .ToList();


        /// <summary>
        /// Archives all routes in the section by the given sectionId.
        /// </summary>
        /// <param name="sectionId">Id that matches the section where all the routes should be archived.</param>
        public void ArchiveAllRoutesInSection(int? sectionId)
        {
            List<RouteSectionRelation> relations = _dbContext.RouteSectionRelations
                                                             .Include(rs => rs.Route)
                                                             .Include(rs => rs.RouteSection)
                                                             .Where(rs => rs.RouteSectionID == sectionId)
                                                             .ToList();

            RouteSection section = _dbContext.RouteSections
                                             .Include(s => s.Routes)
                                             .FirstOrDefault(s => s.RouteSectionID == sectionId);

            if (section?.Routes?.Count > 0)
            {
                foreach (RouteSectionRelation relation in relations)
                {
                    relation.Route.Archived = true;
                    _dbContext.Routes.Update(relation.Route);
                }
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all the routeHalls from the database.
        /// </summary>
        public List<RouteHall> GetAllRouteHalls() => _dbContext.RouteHalls.ToList();

        private RouteHall GetRouteHallById(int? id) => _dbContext.RouteHalls.First(h => h.RouteHallID == id);
    }
}

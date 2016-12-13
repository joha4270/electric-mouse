using electric_mouse.Data;
using electric_mouse.Models.RouteItems;
using electric_mouse.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Services
{
    public class HallService:IHallService
    {
        private ApplicationDbContext _dbContext;

        public HallService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public List<RouteHall> GetActiveHalls()
        {
            IQueryable<RouteHall> halls = _dbContext.RouteHalls
                .Where(hall => hall.Archived == false)
                .Include(hall => hall.Sections);
            return halls.ToList();
        }


        public void AddHall(string name, RouteType type)
        {
            
            type--;
            RouteHall hall = new RouteHall
            {
                Name = name,
                Sections = new List<RouteSection>()
            };

            if (type >= 0)
                hall.ExpectedType = type;

            _dbContext.RouteHalls.Add(hall);
            _dbContext.SaveChanges();
            
        }

        public void DeleteHall( int? id)
        {
            var hall = _dbContext.RouteHalls
                .Include(s => s.Sections)
                .First(h => h.RouteHallID == id);

            if (hall.Sections?.Count(s => s.Archived == false) <= 0)
            {
                hall.Archived = true;
                _dbContext.SaveChanges();
            }
        }
    }
       
}

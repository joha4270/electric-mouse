using electric_mouse.Data;
using electric_mouse.Models.RouteItems;
using electric_mouse.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Services
{
    public class DifficultyService : IDifficultyService
    {
        private readonly ApplicationDbContext _dbContext;

        public DifficultyService(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public List<RouteDifficulty> GetAllDifficulties() => _dbContext.RouteDifficulties.ToList();

        /// <summary>
        /// Adds a new difficulty to the database.
        /// </summary>
        /// <param name="name">Displayname of the difficulty.</param>
        /// <param name="color">The representative colour of the difficulty.</param>
        public void AddDifficulty(string name, string color)
        {
            RouteDifficulty difficulty = new RouteDifficulty { Name = name, ColorHex = color};
            _dbContext.RouteDifficulties.Add(difficulty);
            _dbContext.SaveChanges();
        }


        /// <summary>
        /// Removes a difficulty from the database.
        /// </summary>
        /// <param name="difficultyID">The ID of the difficulty.</param>
        public void RemoveDifficulty(int? difficultyID)
        {
            RouteDifficulty difficulty =  _dbContext.RouteDifficulties.First(diff => diff.RouteDifficultyID == difficultyID);
            _dbContext.RouteDifficulties.Remove(difficulty);
            _dbContext.SaveChanges();
        }
    }
}

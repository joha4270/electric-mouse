using electric_mouse.Models.RouteItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace electric_mouse.Models.DifficultyViewModels
{
    public class DifficultyCreateViewModel
    {
        public IList<RouteDifficulty> Difficulties { get; set; }

        public string Name { get; set; }
        public int ID { get; set; }
    }
}

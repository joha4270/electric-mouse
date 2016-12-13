using electric_mouse.Models.RouteItems;
using System.Collections.Generic;

namespace electric_mouse.Models.DifficultyViewModels
{
    public class DifficultyCreateViewModel
    {
        public IList<RouteDifficulty> Difficulties { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public int? ID { get; set; }
    }
}

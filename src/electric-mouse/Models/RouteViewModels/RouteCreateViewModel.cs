using System.Collections.Generic;
using electric_mouse.Models.RouteItems;
using Microsoft.AspNetCore.Http;

namespace electric_mouse.Models.RouteViewModels
{
    public class RouteCreateViewModel
    {
        /// <summary>
        /// List of halls that the user can select
        /// </summary>
        public IList<RouteHall> Halls { get; set; }

        /// <summary>
        /// List of difficulities that the user can select
        /// </summary>
        public IList<RouteDifficulty> Difficulties { get; set; }

        /// <summary>
        /// List of sections the user can select
        /// </summary>
        public IList<RouteSection> Sections { get; set; }

        public int UpdateID { get; set; } = -1;


        public int RouteID { get; set; }
        public int RouteHallID { get; set; }
        public IList<int> RouteSectionID { get; set; }
        public int RouteDifficultyID { get; set; }
        public string Date { get; set; }
        public string GripColor { get; set; }
        public string Note { get; set; }

        public string VideoUrl { get; set; }
        public RouteAttachment Attachment { get; set; }
        public int AttachmentID { get; set; }
        public IList<int> ImagePathRelationID { get; set; }
        public IList<System.Tuple<string, int>> Images { get; set; }
    }
}
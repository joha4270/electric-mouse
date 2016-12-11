namespace electric_mouse.Models.Relations
{
    public class RouteApplicationUserRelation
    {
        public int RouteRefId { get; set; }
        public Route Route { get; set; }

        public string ApplicationUserRefId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
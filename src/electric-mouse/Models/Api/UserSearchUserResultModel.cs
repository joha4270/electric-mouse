namespace electric_mouse.Models.Api
{
    public class UserSearchUserResultModel
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string ImageUrl { get; set; }

        public static UserSearchUserResultModel FromApplicationUser(ApplicationUser applicationUser)
        {
            return new UserSearchUserResultModel
            {
                ImageUrl = applicationUser.URLPath,
                Name = applicationUser.DisplayName,
                UserId = applicationUser.Id
            };
        }
    }
}
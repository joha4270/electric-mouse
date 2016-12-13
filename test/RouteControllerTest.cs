using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;
using Moq;
using electric_mouse.Controllers;
using electric_mouse.Models;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using electric_mouse.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace test
{
    public class RouteControllerTest
    {
        Mock<ILoggerFactory> LoggerFactory { get; }
        Mock<IHostingEnvironment> HostingEnvironment { get; }
        Mock<IUserService> UserService { get; }
        Mock<IAttachmentHandler> AttachmentHandler { get; }
        Mock<IRouteService> RouteService { get; }
        RouteController RouteController { get; }

        // From the docs https://xunit.github.io/docs/shared-context.html:
        // "xUnit.net creates a new instance of the test class for every test that is run, 
        // so any code which is placed into the constructor of the test class will be run for every single test."
        public RouteControllerTest()
        {
            LoggerFactory = new Mock<ILoggerFactory>();
            HostingEnvironment = new Mock<IHostingEnvironment>();
            UserService = new Mock<IUserService>();
            AttachmentHandler = new Mock<IAttachmentHandler>();
            RouteService = new Mock<IRouteService>();
            RouteController = new RouteController(LoggerFactory.Object,
                                                HostingEnvironment.Object,
                                                UserService.Object,
                                                AttachmentHandler.Object,
                                                RouteService.Object);
        }

        #region Create Get

        [Fact]
        public async void CreateGet_ValidRouteCreateViewModel_ReturnViewWithRouteCreateViewModel()
        {
            // Arrange
            UserService.Setup(service => service.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .Returns(Task.FromResult(new ApplicationUser()));

            RouteService.Setup(service => service.GetAllActiveRouteHalls()).Returns(new List<RouteHall>
            {
                new RouteHall(),
                new RouteHall()
            });

            RouteService.Setup(service => service.GetAllRouteDifficulties()).Returns(new List<RouteDifficulty>
            {
                new RouteDifficulty(),
                new RouteDifficulty()
            });

            RouteService.Setup(service => service.GetAllActiveRouteSections()).Returns(new List<RouteSection>
            {
                new RouteSection(),
                new RouteSection()
            });

            // Act
            IActionResult result = await RouteController.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<RouteCreateViewModel>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Halls.Count);
            Assert.Equal(2, model.Difficulties.Count);
            Assert.Equal(2, model.Sections.Count);
            Assert.Equal(1, model.BuilderList.Count);
            Assert.Equal(1, model.Builders.Count);
        }

        #endregion

        #region Create Post

        [Fact]
        public async void CreatePost_RouteCreateViewModelIsValid_SectionAddedAndRedirectsToListAction()
        {
            // Arrange
            var viewModel = new RouteCreateViewModel
            {
                Note = "This is a test note.",
                RouteID = 1,
                GripColor = "000000",
                Type = RouteType.Boulder
            };



            // Act
            IActionResult result = await RouteController.Create(viewModel);
        }

        //public async Task<IActionResult> Create(RouteCreateViewModel model)
        //{
        //    _logger.LogInformation("Received following users [{users}]", string.Join(", ", model.Builders));

        //    // Create Route
        //    Route route = new Route
        //    {
        //        Note = model.Note,
        //        RouteID = model.RouteID,
        //        GripColour = model.GripColor,
        //        Type = model.Type
        //    };
        //    _routeService.AddRoute(route, model.Date, model.RouteDifficultyID);

        //    // Create Section Relation
        //    _routeService.AddRouteToSections(route, model.RouteSectionID.ToList());

        //    //Find all the users by id in model.Builders, in parallel, then discard missing results
        //    IEnumerable<ApplicationUser> builders =
        //        (
        //            await Task.WhenAll
        //                (
        //                    model.Builders
        //                        .Distinct()
        //                        .Select(userId => _userService.FindByIdAsync(userId))
        //                )
        //        )
        //        .Where(user => user != null);

        //    _routeService.AddBuildersToRoute(route, builders.ToArray());

        //    // Add the image(s) and imagepaths to the database, and the videourl
        //    string[] relativeImagePaths = await UploadImages();
        //    _routeService.AddAttachment(route, model.VideoUrl, relativeImagePaths);

        //    return RedirectToAction(nameof(List), "Route");
        //}

        #endregion
    }
}

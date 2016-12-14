using System.Collections.Generic;
using System.IO;
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
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Route = electric_mouse.Models.Route;

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
        // Setup code (runs for every test)
        public RouteControllerTest()
        {
            LoggerFactory = new Mock<ILoggerFactory>();
            HostingEnvironment = new Mock<IHostingEnvironment>();
            UserService = new Mock<IUserService>();
            AttachmentHandler = new Mock<IAttachmentHandler>();
            RouteService = new Mock<IRouteService>();
            RouteController = new RouteController(HostingEnvironment.Object,
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

            #region User Setup
            var userId1 = "0f8fad5b-d9cb-469f-a165-70867728950e";
            var userId2 = "ffffad5b-d9cb-469f-a165-70867728950e";
            var appUser1 = new ApplicationUser {DisplayName = "User1"};
            var appUser2 = new ApplicationUser {DisplayName = "User2"};
            UserService.Setup(service => service.FindByIdAsync(It.Is<string>(id => id.Equals(userId1))))
                       .Returns(Task.FromResult(appUser1));
            UserService.Setup(service => service.FindByIdAsync(It.Is<string>(id => id.Equals(userId2))))
                       .Returns(Task.FromResult(appUser2));
            #endregion

            #region File Setup
            string fakeWebRootPath = "fake/fakepath/veryfakepath/";
            HostingEnvironment.Setup(envi => envi.WebRootPath).Returns(fakeWebRootPath);

            string[] relativePaths =
            {
                "uploads/awidjqo.jpg"
            };

            IFormFileCollection formFileCollection = new FormFileCollection
            {
                new FormFile(Stream.Null, 0, 200, "Images/imageToDelete1", "imageToDelete1.jpg"),
                new FormFile(Stream.Null, 0, 200, "Images/imageToDelete2", "imageToDelete2.jpg"),
                new FormFile(Stream.Null, 0, 200, "Images/imageToUpload", "imageToUpload.jpg")
            };

            AttachmentHandler.Setup(
                                 handler => handler.SaveImagesOnServer(
                                     It.Is<IList<IFormFile>>(
                                         files => files.Intersect(formFileCollection).Count() == files.Count),
                                     It.Is<string>(path => path.Equals(fakeWebRootPath)),
                                     It.Is<string>(uploadPath => uploadPath.Equals("uploads"))))
                             .Returns(Task.FromResult(relativePaths));

            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Form["jfiler-items-exclude-Images-0"]).Returns("[\"imageToDelete1.jpg\",\"imageToDelete2.jpg\"]");
            request.SetupGet(x => x.Form.Files).Returns(formFileCollection);

            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            RouteController.ControllerContext =
                new ControllerContext(new ActionContext(context.Object, new RouteData(), new ControllerActionDescriptor()));
            #endregion

            #region Route & ViewModel
            var viewModel = new RouteCreateViewModel
            {
                Note = "This is a test note.",
                RouteID = 1,
                GripColor = "000000",
                Type = RouteType.Boulder,
                Builders = new List<string>
                {
                    userId1,
                    userId2
                },
                Date = "14-12-2016",
                RouteDifficultyID = 1,
                RouteSectionID = new List<int>
                {
                    1,
                    2
                },
                VideoUrl = "https://www.youtube.com/watch?v=bEYSyfEL8nY"
            };

            Route routeToCreate = new Route
            {
                Note = viewModel.Note,
                RouteID = viewModel.RouteID,
                GripColour = viewModel.GripColor,
                Type = viewModel.Type
            };
            #endregion

            // Act
            IActionResult result = await RouteController.Create(viewModel);

            // Assert
            #region Assertions
            RouteService.Verify(service => service.AddRoute(It.Is<Route>(route => RouteEquals(route, routeToCreate)),
                    It.Is<string>(date => date.SequenceEqual(viewModel.Date)),
                    It.Is<int>(diff => diff == viewModel.RouteDifficultyID)),
                Times.Once);

            RouteService.Verify(service => service.AddRouteToSections(It.Is<Route>(route => RouteEquals(route, routeToCreate)),
                It.Is<List<int>>(idList => idList.SequenceEqual(viewModel.RouteSectionID.ToList()))
                ),
                Times.Once);

            RouteService.Verify(
                service => service.AddBuildersToRoute(It.Is<Route>(route => RouteEquals(route, routeToCreate)),
                    It.Is<ApplicationUser[]>(users => users.SequenceEqual(new[]
                        {
                            appUser1,
                            appUser2
                        })
                    )),
                Times.Once);

            RouteService.Verify(service => service.AddAttachment(It.Is<Route>(route => RouteEquals(route, routeToCreate)),
                It.Is<string>(url => url.Equals(viewModel.VideoUrl)),
                It.Is<string[]>(paths => paths.SequenceEqual(relativePaths))
            ), Times.Once);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Route", redirectToActionResult.ControllerName);
            Assert.Equal("List", redirectToActionResult.ActionName);
            #endregion
        }

        private bool RouteEquals(Route op1, Route op2)
        {
            return op1.Note == op2.Note
                   && op1.RouteID == op2.RouteID
                   && op1.GripColour == op2.GripColour
                   && op1.Type == op2.Type
                   && op1.Date == op2.Date
                   && op1.GripColour == op2.GripColour;
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using electric_mouse.Controllers;
using electric_mouse.Models.RouteItems;
using Microsoft.AspNetCore.Mvc;
using electric_mouse.Services.Interfaces;
using electric_mouse.Models.DifficultyViewModels;
using System.Security.Principal;
using System.Security.Claims;
using electric_mouse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace test
{
    public class DifficultyControllerTest
    {
        #region Create Tests

        // Out of Order
        [Fact(Skip = "Error with the http-context")]
        public async Task Create_NoAdminPrivileges_NotAddedToDatabase()
        {
            // Arrange
            var identity = new GenericIdentity("Jack");
            var principal = new Mock<ClaimsPrincipal>();
            var mockService = new Mock<IDifficultyService>();
            DifficultyController controller = new DifficultyController(mockService.Object);
            var controllerContext = new Mock<ControllerContext>();

            principal.Setup(p => p.IsInRole("Administrator"))
                .Returns(false);
            principal.SetupGet(p => p.Identity.Name).Returns("Jack");
            controllerContext.SetupGet(c => c.HttpContext.User)
                .Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;
            
            var difficultyListViewModel = new DifficultyCreateViewModel { Name = "Red", Color = "#4286f4" };

            // Act
            var result = await controller.Create(difficultyListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Difficulty", redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddDifficulty(It.IsAny<string>(), It.IsAny<string>()), Times.Never); // checks that the sectionService.AddSection was called once.
        }
        


        [Theory]
        [InlineData("Red", "#428")]
        [InlineData("Red", "#4286F4")]
        [InlineData("Red", "#4286f4")]
        public async Task Create_NameValidAndColorValid_AddedToDatabaseAndRedirectsToListAction(string name, string color)
        {
            // Arrange
            var mockService = new Mock<IDifficultyService>();
            DifficultyController controller = new DifficultyController(mockService.Object);
            var difficultyListViewModel = new DifficultyCreateViewModel { Name = name, Color = color };

            // Act
            var result = await controller.Create(difficultyListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Difficulty", redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddDifficulty(It.IsAny<string>(), It.IsAny<string>()), Times.Once); // checks that the sectionService.AddSection was called once.
        }


        [Theory]
        [InlineData("Red", null)]
        [InlineData(null, "#4286f4")]
        [InlineData("Red", "#4286h4")] // Invalid Color
        [InlineData("Red", "#4286f45")] // Invalid Color
        public async Task Create_InputsAreInvalid_NotAddedToDatabaseAndRedirectsToListAction(string name, string color)
        {
            // Arrange
            var mockService = new Mock<IDifficultyService>();
            DifficultyController controller = new DifficultyController(mockService.Object);
            var difficultyListViewModel = new DifficultyCreateViewModel { Name = name, Color = color };

            // Act
            var result = await controller.Create(difficultyListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Difficulty", redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddDifficulty(It.IsAny<string>(), It.IsAny<string>()), Times.Never); // checks that the sectionService.AddSection is never called.
        }

        #endregion

        #region List tests
        [Fact]
        public void List_ValidDifficultyListViewModel_ReturnViewWithDifficultyListViewModel()
        {
            // Arrange
            var mockService = new Mock<IDifficultyService>();
            mockService.Setup(service => service.GetAllDifficulties())
                       .Returns(new List<RouteDifficulty> { new RouteDifficulty { }, new RouteDifficulty { } });
            DifficultyController controller = new DifficultyController(mockService.Object);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<DifficultyCreateViewModel>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Difficulties.Count);
        }
        #endregion

        #region Delete Tests
        [Fact]
        public async Task Delete_IdValid_RemovedFromDatabaseAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<IDifficultyService>();
            DifficultyController controller = new DifficultyController(mockService.Object);
            var difficultyListViewModel = new DifficultyCreateViewModel { ID = 1 };

            // Act
            var result = await controller.Delete(difficultyListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Difficulty", redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockService.Verify(service => service.RemoveDifficulty(It.Is<int?>(i => i == 1)), Times.Once);
        }

        [Fact]
        public async Task Delete_IdInvalid_NotRemovedFromDatabaseAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<IDifficultyService>();
            DifficultyController controller = new DifficultyController(mockService.Object);

            var difficultyListViewModel = new DifficultyCreateViewModel { ID = null };

            // Act
            var result = await controller.Create(difficultyListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Difficulty", redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockService.Verify(service => service.RemoveDifficulty(It.IsAny<int?>()), Times.Never);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using Xunit;
using electric_mouse.Services.Interfaces;
using electric_mouse.Models;
using Microsoft.AspNetCore.Identity;
using electric_mouse.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace test
{
    public class CommentControllerTest
    {
        [Fact]
        public async void Reply_InputsAreValid_AddedToDatabaseAndRedirectsToRouteListAction()
        {
            // Arrange
            var mockService = new Mock<ICommentService>();
            var userService = new Mock<IUserService>();
            CommentsController controller = new CommentsController(userService.Object, mockService.Object);
            var commentViewModel = new CommentViewModel { Content = "Oy m8 wat uup", RouteID = 1 };

            // Act
            var result = await controller.Reply(commentViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Route", redirectToActionResult.ControllerName);
            Assert.Equal("Details", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddComment(It.IsAny< ApplicationUser>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(1, "Nice route")]
        [InlineData(2, "Keep them coming")]
        public async void Add_InputsAreValid_AddedToDatabaseAndRedirectsToRouteListAction(int id, string content)
        {
            // Arrange
            var mockService = new Mock<ICommentService>();
            var userService = new Mock<IUserService>();
            CommentsController controller = new CommentsController(userService.Object, mockService.Object);
            
            // Act
            var result = await controller.AddComment(id, content);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Route", redirectToActionResult.ControllerName);
            Assert.Equal("Details", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddComment(It.IsAny<ApplicationUser>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async void Delete_InputsAreValid_RemovedFromDatabaseAndRedirectsToRouteListAction()
        {
            // Arrange
            var mockService = new Mock<ICommentService>();
            var userService = new Mock<IUserService>();
            CommentsController controller = new CommentsController(userService.Object, mockService.Object);
            var commentViewModel = new CommentViewModel { CommentID = 1 };

            // Act
            var result = await controller.Delete(commentViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Route", redirectToActionResult.ControllerName);
            Assert.Equal("Details", redirectToActionResult.ActionName);
            mockService.Verify(service => service.DeleteComment(It.Is<int>(i => i == 1)), Times.Once);
        }

    }
}

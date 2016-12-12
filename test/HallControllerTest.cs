using electric_mouse.Controllers;
using electric_mouse.Models.HallViewModels;
using electric_mouse.Models.RouteItems;
using electric_mouse.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace test
{
    public class HallControllerTest
    {

        [Fact]
        public void GetActiveHalls_Checks_If_Normal_input_works()
        {
            // Arrange
            var mockService = new Mock<IHallService>();
            mockService.Setup(service => service.GetActiveHalls()).Returns(new List<RouteHall> { new RouteHall(), new RouteHall() });
            HallController controller = new HallController(mockService.Object);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HallCreateViewModel>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Halls.Count);
        }

        #region AddHall tests

        [Fact]
        public async Task AddHall_Checks_If_Normal_input_works()
        {
            // Arrange
            var mockService = new Mock<IHallService>();
            HallController controller = new HallController(mockService.Object);

            var hallListViewModel = new HallCreateViewModel { Name = "boulder", Type = 0 };

            // Act
            var result = await controller.Create(hallListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Hall", redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddHall(It.IsAny<string>(), It.IsAny<RouteType>()), Times.Once); 
        }



        [Fact]
        public async Task AddHall_Name_Is_Empty_String()
        {
            // Arrange
            var mockService = new Mock<IHallService>();
            HallController controller = new HallController(mockService.Object);

            var hallListViewModel = new HallCreateViewModel { Name = null, Type = 0 };

            // Act
            var result = await controller.Create(hallListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(hallListViewModel.Name); /*uden denne pass'er den det burde den ikke */
            Assert.Equal("Hall", redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddHall(It.IsAny<string>(), It.IsAny<RouteType>()), Times.Once);
        }

        #endregion


        [Fact]
        public async Task Delete_hall_Checks_If_Normal_input_works()
        {
            // Arrange
            var mockService = new Mock<IHallService>();
            HallController controller = new HallController(mockService.Object);

            var hallCreateViewModel = new HallCreateViewModel { ID=1 };

            // Act
            var result = await controller.Delete(hallCreateViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Hall", redirectToActionResult.ControllerName);
            Assert.Equal("Create", redirectToActionResult.ActionName); /* why create??  */
            mockService.Verify(service => service.DeleteHall(It.IsAny<int>()), Times.Once);
        }




    }
}
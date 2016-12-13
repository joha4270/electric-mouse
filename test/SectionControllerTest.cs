using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using electric_mouse.Controllers;
using electric_mouse.Data;
using electric_mouse.Models.RouteItems;
using electric_mouse.Models.RouteViewModels;
using electric_mouse.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using electric_mouse.Services.Interfaces;

namespace test
{
    public class SectionControllerTest
    {
        [Fact]
        public void GetAllRouteSections_ThreeSectionsInDatabase_ListCountIsThree()
        {
            // Ignore this test for now.
            // Use in memory testing (integration testing)
            Assert.True(false);
        }

        #region Create tests
        [Fact]
        public async Task Create_SectionNameValidAndHallIDValid_SectionAddedAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<ISectionService>();
            SectionController controller = new SectionController(mockService.Object);

            var sectionListViewModel = new SectionListViewModel {HallID = 1, SectionName = "A"};

            // Act
            var result = await controller.Create(sectionListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Section", redirectToActionResult.ControllerName);
            Assert.Equal("List", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddSection(It.IsAny<string>(), It.IsAny<int?>()), Times.Once); // checks that the sectionService.AddSection was called once.
        }

        [Fact]
        public async Task Create_HallIDNull_SectionNotAddedAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<ISectionService>();
            SectionController controller = new SectionController(mockService.Object);

            var sectionListViewModel = new SectionListViewModel { HallID = null, SectionName = "A" };

            // Act
            var result = await controller.Create(sectionListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Section", redirectToActionResult.ControllerName);
            Assert.Equal("List", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddSection(It.IsAny<string>(), It.IsAny<int?>()), Times.Never); // checks that the sectionService.AddSection is never called.
        }

        [Fact]
        public async Task Create_SectionNameNull_SectionNotAddedAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<ISectionService>();
            SectionController controller = new SectionController(mockService.Object);

            var sectionListViewModel = new SectionListViewModel { HallID = 1, SectionName = null };

            // Act
            var result = await controller.Create(sectionListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Section", redirectToActionResult.ControllerName);
            Assert.Equal("List", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddSection(It.IsAny<string>(), It.IsAny<int?>()), Times.Never); // checks that the sectionService.AddSection is never called.
        }

        [Fact]
        public async Task Create_SectionNameEmpty_SectionNotAddedAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<ISectionService>();
            SectionController controller = new SectionController(mockService.Object);

            var sectionListViewModel = new SectionListViewModel { HallID = 1, SectionName = "" };

            // Act
            var result = await controller.Create(sectionListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Section", redirectToActionResult.ControllerName);
            Assert.Equal("List", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddSection(It.IsAny<string>(), It.IsAny<int?>()), Times.Never); // checks that the sectionService.AddSection is never called.
        }

        [Fact]
        public async Task Create_SectionNameNullAndHallIDNull_SectionNotAddedAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<ISectionService>();
            SectionController controller = new SectionController(mockService.Object);

            var sectionListViewModel = new SectionListViewModel { HallID = null, SectionName = null };

            // Act
            var result = await controller.Create(sectionListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Section", redirectToActionResult.ControllerName);
            Assert.Equal("List", redirectToActionResult.ActionName);
            mockService.Verify(service => service.AddSection(It.IsAny<string>(), It.IsAny<int?>()), Times.Never); // checks that the sectionService.AddSection is never called.
        }
        #endregion

        #region List tests

        [Fact]
        public void List_ValidSectionListViewModel_ReturnViewWithSectionListViewModel()
        {
            // Arrange
            var mockService = new Mock<ISectionService>();
            mockService.Setup(service => service.GetAllRouteSections())
                       .Returns(new List<RouteSection> {new RouteSection(), new RouteSection()});
            mockService.Setup(service => service.GetAllRouteHalls())
                       .Returns(new List<RouteHall> {new RouteHall(), new RouteHall()});
            SectionController controller = new SectionController(mockService.Object);

            // Act
            var result = controller.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<SectionListViewModel>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Sections.Count);
            Assert.Equal(2, model.Halls.Count);
        }

        #endregion

        #region Delete tests

        [Fact]
        public async Task Delete_SectionIdValid_ArchiveSectionCalledAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<ISectionService>();
            SectionController controller = new SectionController(mockService.Object);

            var sectionListViewModel = new SectionListViewModel { SectionID = 1 };

            // Act
            var result = await controller.Delete(sectionListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Section", redirectToActionResult.ControllerName);
            Assert.Equal("List", redirectToActionResult.ActionName);
            mockService.Verify(service => service.ArchiveSection(It.IsAny<int?>()), Times.Once);
        }

        #endregion

        #region Delete tests

        [Fact]
        public async Task Clear_SectionIdValid_ArchiveAllRoutesInSectionCalledAndRedirectsToListAction()
        {
            // Arrange
            var mockService = new Mock<ISectionService>();
            SectionController controller = new SectionController(mockService.Object);

            var sectionListViewModel = new SectionListViewModel { SectionID = 1 };

            // Act
            var result = await controller.Clear(sectionListViewModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Section", redirectToActionResult.ControllerName);
            Assert.Equal("List", redirectToActionResult.ActionName);
            mockService.Verify(service => service.ArchiveAllRoutesInSection(It.IsAny<int?>()), Times.Once);
        }

        #endregion
    }
}

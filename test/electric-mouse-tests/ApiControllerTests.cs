using Xunit;
using Microsoft.EntityFrameworkCore;
using electric_mouse.Data;
using electric_mouse.Controllers;
using Microsoft.AspNetCore.Mvc;
using electric_mouse.Models.Api;
using System.Collections.Generic;

namespace electric_mouse_tests
{
    public class ApiControllerTests
    {
        [Fact]
        public async void EmptyQueryYieldsResults()
        {
            //Create a dbContext to use
            ApplicationDbContext context = CreateFalseUsers();

            //Create the controller we are testing. Using self created dbcontext instead of real data, to control result
            ApiController controller = new ApiController(context);

            //Make the controller do something
            var result = await controller.QueryUsers("");

            //Extract the real data of the responce. If we get something completly unexpected, something went wrong anyway
            JsonResult jsonResult = Assert.IsAssignableFrom<JsonResult>(result);
            List<UserSearchUserResultModel> resultContents = Assert.IsType<List<UserSearchUserResultModel>>(jsonResult);

            //And do a real(ish) check on the data we recived. In this case that we got 3 results (and we know we have 3 users in test db)
            Assert.Equal(3, resultContents.Count);
        }

        [Fact]
        public async void QueryFirstName()
        {

            ApplicationDbContext context = CreateFalseUsers();
            ApiController controller = new ApiController(context);

            JsonResult result = Assert.IsAssignableFrom<JsonResult>(await controller.QueryUsers("Mi"));
            List<UserSearchUserResultModel> resultContents = Assert.IsType<List<UserSearchUserResultModel>>(result.Value);


            Assert.Equal(2, resultContents.Count);

            Assert.Equal("9eca2aca-c076-4abd-9c44-a63b00c445fd", resultContents[0].UserId);
            Assert.Equal("8ecbe246-4e8d-4814-b281-86f46fd0f88c", resultContents[1].UserId);
        }

        [Fact]
        public async void QueryLastName()
        {
            ApplicationDbContext context = CreateFalseUsers();
            ApiController controller = new ApiController(context);

            JsonResult result = Assert.IsAssignableFrom<JsonResult>(await controller.QueryUsers("Duck"));
            List<UserSearchUserResultModel> resultContents = Assert.IsType<List<UserSearchUserResultModel>>(result.Value);


            Assert.Equal(1, resultContents.Count);
            Assert.Equal("Donald Duck", resultContents[0].Name);
            Assert.Equal("419a1094-33fd-4e82-92f8-c16ac73af0e1", resultContents[0].UserId);
        }

        [Fact]
        public async void QueryMiddle()
        {
            ApplicationDbContext context = CreateFalseUsers();
            ApiController controller = new ApiController(context);

            JsonResult result = Assert.IsAssignableFrom<JsonResult>(await controller.QueryUsers("e l"));
            List<UserSearchUserResultModel> resultContents = Assert.IsType<List<UserSearchUserResultModel>>(result.Value);

            Assert.Equal(1, resultContents.Count);
            Assert.Equal("9eca2aca-c076-4abd-9c44-a63b00c445fd", resultContents[0].UserId);
        }

        [Fact]
        public async void QueryNoResult()
        {
            ApplicationDbContext context = CreateFalseUsers();
            ApiController controller = new ApiController(context);

            JsonResult result = Assert.IsAssignableFrom<JsonResult>(await controller.QueryUsers("Scrouge"));
            List<UserSearchUserResultModel> resultContents = Assert.IsType<List<UserSearchUserResultModel>>(result.Value);


            Assert.Equal(0, resultContents.Count);
        }

        private ApplicationDbContext CreateFalseUsers()
        {
            //Should be self explanetory, but create a new entity framework database, stored only in memory
            DbContextOptionsBuilder<ApplicationDbContext> builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase(databaseName: "Add_writes_to_database");
            ApplicationDbContext dummyDbContext = new ApplicationDbContext(builder.Options);

            dummyDbContext.Database.EnsureDeleted(); //Make sure database is clean. Could probably be done cleaner
            dummyDbContext.Database.EnsureCreated();
            dummyDbContext.SaveChanges();
            dummyDbContext.Users.Add(
                new electric_mouse.Models.ApplicationUser
                {
                    DisplayName = "Michelle Larsen",
                    Email = "lady@example.com",
                    UserName = "123",
                    Id = "9eca2aca-c076-4abd-9c44-a63b00c445fd",
                    URLPath = "https://duckbook.tld/m"
                });

            dummyDbContext.Users.Add(
                new electric_mouse.Models.ApplicationUser
                {
                    DisplayName = "Donald Duck",
                    Email = "mrduck@example.com",
                    UserName = "125",
                    Id = "419a1094-33fd-4e82-92f8-c16ac73af0e1",
                    URLPath = "https://duckbook.tld/w"
                });

            dummyDbContext.Users.Add(
                new electric_mouse.Models.ApplicationUser
                {
                    DisplayName = "Mickey mouse",
                    Email = "mickey@example.com",
                    UserName = "127",
                    Id = "8ecbe246-4e8d-4814-b281-86f46fd0f88c",
                    URLPath = "https://duckbook.tld/a"
                });

            dummyDbContext.SaveChanges();

            return dummyDbContext;
        }
    }
}

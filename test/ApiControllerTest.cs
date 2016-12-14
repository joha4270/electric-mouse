using System;
using System.Collections.Generic;
using electric_mouse.Controllers;
using electric_mouse.Data;
using electric_mouse.Models;
using electric_mouse.Services;
using electric_mouse.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using test.Tools;
using Xunit;

namespace test
{
	public class ApiControllerTest
	{
		private IQueryable<ApplicationUser> CreateDummyUsers()
		{
			ApplicationUser user  = new ApplicationUser { DisplayName = "John Doe" };
			ApplicationUser user2 = new ApplicationUser { DisplayName = "Jane Doe" };
			return new List<ApplicationUser>()
				{user, user2}
				.AsQueryable();
		}

		[Fact]
		public async Task User_Returned_By_Query()
		{
			// Arrange
			IQueryable<ApplicationUser> list = CreateDummyUsers();

			var userStore = new Mock<IQueryableUserStore<ApplicationUser>>();
			userStore.Setup(x => x.Users).Returns(list);

			FakeUserManager userManager = new FakeUserManager(userStore);
			IApiService apiService = new ApiService(userManager);

			// Act
			var users = apiService.QueryUsers("jo"); // John Doe
			// Instead of initializing in here, we'll have to do this workaround
			var retrievedUser = users
				.FirstOrDefault(u => u.UserId == list
					                     .FirstOrDefault(x =>
							                     x.DisplayName == "John Doe").Id);

			// Assert
			Assert.True(retrievedUser != null);
		}

		[Fact]
		public async Task Users_NoUsersReturned()
		{
			// Arrange
			IQueryable<ApplicationUser> list = CreateDummyUsers();

			var userStore = new Mock<IQueryableUserStore<ApplicationUser>>();
			userStore.Setup(x => x.Users).Returns(list);

			FakeUserManager userManager = new FakeUserManager(userStore);
			IApiService apiService = new ApiService(userManager);

			// Act
			var users = apiService.QueryUsers("none"); // None
			// Instead of initializing in here, we'll have to do this workaround
			var retrievedUser = users
				.FirstOrDefault(u =>
					u.UserId == list
						.FirstOrDefault(x =>
								x.DisplayName == "None").Id);

			Assert.True(retrievedUser == null);
		}
	}
}
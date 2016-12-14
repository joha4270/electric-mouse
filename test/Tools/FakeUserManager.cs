using System;
using System.Threading.Tasks;
using electric_mouse.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace test.Tools
{
	public class FakeUserManager : UserManager<ApplicationUser>
	{
		// If no userstore is passed
		public FakeUserManager() : this(new Mock<IQueryableUserStore<ApplicationUser>>()) { }

		public FakeUserManager(Mock<IQueryableUserStore<ApplicationUser>> mockUserStore)
			: base(mockUserStore.Object,
				new Mock<IOptions<IdentityOptions>>().Object,
				new Mock<IPasswordHasher<ApplicationUser>>().Object,
				new IUserValidator<ApplicationUser>[0],
				new IPasswordValidator<ApplicationUser>[0],
				new Mock<ILookupNormalizer>().Object,
				new Mock<IdentityErrorDescriber>().Object,
				new Mock<IServiceProvider>().Object,
				new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
		{ }

		public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
		{
			return Task.FromResult(IdentityResult.Success);
		}
	}

}
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;

namespace ems_back.Tests.Utilities
{
	public static class TestHelpers
	{
		public static Mock<UserManager<TUser>> CreateUserManagerMock<TUser>() where TUser : class
		{
			var store = new Mock<IUserStore<TUser>>();
			var mock = new Mock<UserManager<TUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			mock.Object.UserValidators.Add(new UserValidator<TUser>());
			mock.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

			return mock;
		}

		public static Mock<SignInManager<TUser>> CreateSignInManagerMock<TUser>() where TUser : class
		{
			var userManagerMock = CreateUserManagerMock<TUser>();
			var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
			var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();

			return new Mock<SignInManager<TUser>>(
				userManagerMock.Object,
				contextAccessor.Object,
				claimsFactory.Object,
				null,
				null,
				null,
				null);
		}
	}
}
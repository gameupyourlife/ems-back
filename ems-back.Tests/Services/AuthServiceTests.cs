using ems_back.Repo.DTOs.Auth;
using ems_back.Repo.DTOs.Login;
using ems_back.Repo.DTOs.Register;
using ems_back.Repo.Models;
using ems_back.Repo.Interfaces.Service;
using ems_back.Services;
using ems_back.Tests.Utilities;

using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using ems_back.Repo.Models.Types;

namespace ems_back.Tests.Services
{
	public class AuthServiceTests : IDisposable
	{
		private readonly TestReportGenerator _report;
		private readonly Mock<IUserService> _userServiceMock = new();
		private readonly Mock<ITokenService> _tokenServiceMock = new();
		private readonly Mock<IOrganizationService> _organizationServiceMock = new();
		private readonly Mock<ILogger<AuthService>> _loggerMock = new();
		private readonly AuthService _authService;
        private readonly Mock<UserManager<User>> _userManagerMock = new();

        public AuthServiceTests(ITestOutputHelper output)
		{
			_report = new TestReportGenerator(output);
			_authService = new AuthService(
				_userServiceMock.Object,
				_tokenServiceMock.Object,
				_organizationServiceMock.Object,
				_loggerMock.Object,
				_userManagerMock.Object);
		}

		[Fact]
		public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
		{
			var testName = nameof(LoginAsync_ValidCredentials_ReturnsSuccess);
			var startTime = DateTime.Now;
			bool testPassed = false;
			string message = null;

			try
			{
				// Arrange
				var request = new LoginRequest { Email = "test@example.com", Password = "ValidP@ss1" };
				var user = new User 
				{ 
					Id = Guid.NewGuid(), 
					Email = request.Email,
                    FirstName = "Test",
                    LastName = "User"
                };
				var token = "generated.jwt.token";

				_userServiceMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
				_userServiceMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password)).ReturnsAsync(SignInResult.Success);
				_tokenServiceMock.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync(token);
				_organizationServiceMock.Setup(x => x.HandleAutomaticOrganizationMembership(request.Email)).Returns(Task.CompletedTask);

				// Act
				var result = await _authService.LoginAsync(request);

				// Assert
				result.Success.Should().BeTrue();
				result.Token.Should().Be(token);
				_organizationServiceMock.Verify(x => x.HandleAutomaticOrganizationMembership(request.Email), Times.Once);
				testPassed = true;
			}
			catch (Exception ex)
			{
				message = ex.Message;
				throw;
			}
			finally
			{
				var duration = DateTime.Now - startTime;
				_report.AddTestResult(testName, testPassed, duration, message);
			}
		}

		[Fact]
		public async Task LoginAsync_InvalidEmail_ReturnsFailure()
		{
			var request = new LoginRequest { Email = "notfound@example.com", Password = "any" };
			_userServiceMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync((User)null);

			var result = await _authService.LoginAsync(request);

			result.Success.Should().BeFalse();
			result.Token.Should().BeNull();
		}

		[Fact]
		public async Task LoginAsync_WrongPassword_ReturnsFailure()
		{
			var user = new User 
			{ 
				Id = Guid.NewGuid(), 
				Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };
			var request = new LoginRequest { Email = user.Email, Password = "wrongpass" };

			_userServiceMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
			_userServiceMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password)).ReturnsAsync(SignInResult.Failed);

			var result = await _authService.LoginAsync(request);

			result.Success.Should().BeFalse();
			result.Token.Should().BeNull();
		}

		[Fact]
		public async Task RegisterAsync_ValidData_CreatesUser()
		{
			var request = new RegisterRequest
			{
				FirstName = "Jane",
				LastName = "Doe",
				Email = "jane@example.com",
				Password = "ValidP@ss1",
				Role = UserRole.User
			};

			var user = new User 
			{ 
				Id = Guid.NewGuid(), 
				Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

			_userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), request.Password)).ReturnsAsync(IdentityResult.Success);
			_userServiceMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), request.Role.ToString())).ReturnsAsync(IdentityResult.Success);
			_tokenServiceMock.Setup(x => x.GenerateTokenAsync(It.IsAny<User>())).ReturnsAsync("jwt.token");
			_organizationServiceMock.Setup(x => x.HandleAutomaticOrganizationMembership(request.Email)).Returns(Task.CompletedTask);

			var result = await _authService.RegisterAsync(request);

			result.Success.Should().BeTrue();
			result.Token.Should().Be("jwt.token");
		}

		[Fact]
		public async Task RegisterAsync_DuplicateEmail_ReturnsError()
		{
			var request = new RegisterRequest
			{
				FirstName = "John",
				LastName = "Doe",
				Email = "john@example.com",
				Password = "pass",
				Role = UserRole.User
			};

			var identityError = new IdentityError { Description = "Duplicate email" };
			var failedResult = IdentityResult.Failed(identityError);

			_userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<User>(), request.Password)).ReturnsAsync(failedResult);

			var result = await _authService.RegisterAsync(request);

			result.Success.Should().BeFalse();
		}

		public void Dispose()
		{
			_report.Dispose();
		}
	}
}

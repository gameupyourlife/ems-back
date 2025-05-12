//using ems_back.Services;
//using ems_back.Repo.DTOs.Auth;
//using ems_back.Repo.DTOs.Login;
//using ems_back.Repo.Models;
//using Microsoft.AspNetCore.Identity;
//using Moq;
//using FluentAssertions;
//using Xunit;
//using Xunit.Abstractions;
//using ems_back.Tests.Utilities;
//using Microsoft.Extensions.Logging;
//using ems_back.Repo.Interfaces.Service;

//namespace ems_back.Tests.Services
//{
//	public class AuthServiceTests : IDisposable
//	{
//		private readonly TestReportGenerator _report;
//		private readonly Mock<IUserService> _userServiceMock = new();
//		private readonly Mock<ITokenService> _tokenServiceMock = new();
//		private readonly Mock<ILogger<AuthService>> _loggerMock = new();
//		private readonly AuthService _authService;

//		public AuthServiceTests(ITestOutputHelper output)
//		{
//			_report = new TestReportGenerator(output);
//			_authService = new AuthService(
//				_userServiceMock.Object,
//				_tokenServiceMock.Object,
//				_loggerMock.Object);
//		}

//		[Fact]
//		public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
//		{
//			var testName = nameof(LoginAsync_ValidCredentials_ReturnsSuccess);
//			var startTime = DateTime.Now;
//			bool testPassed = false;
//			string message = null;

//			try
//			{
//				// Arrange
//				var request = new LoginRequest { Email = "test@example.com", Password = "ValidP@ss1" };
//				var user = new User { Id = Guid.NewGuid(), Email = request.Email };
//				var token = "generated.jwt.token";

//				_userServiceMock.Setup(x => x.FindByEmailAsync(request.Email))
//					.ReturnsAsync(user);
//				_userServiceMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password))
//					.ReturnsAsync(SignInResult.Success);
//				_tokenServiceMock.Setup(x => x.GenerateTokenAsync(user))
//					.ReturnsAsync(token);

//				// Act
//				var result = await _authService.LoginAsync(request);

//				// Assert
//				result.Success.Should().BeTrue();
//				result.Token.Should().Be(token);
//				testPassed = true;
//			}
//			catch (Exception ex)
//			{
//				message = ex.Message;
//				throw;
//			}
//			finally
//			{
//				var duration = DateTime.Now - startTime;
//				_report.AddTestResult(testName, testPassed, duration, message);
//			}
//		}

//		[Fact]
//		public async Task LoginAsync_InvalidEmail_ReturnsFailure()
//		{
//			// Similar structure with invalid email test
//		}

//		[Fact]
//		public async Task LoginAsync_WrongPassword_ReturnsFailure()
//		{
//			// Similar structure with wrong password test
//		}

//		[Fact]
//		public async Task RegisterAsync_ValidData_CreatesUser()
//		{
//			// Registration test implementation
//		}

//		[Fact]
//		public async Task RegisterAsync_DuplicateEmail_ReturnsError()
//		{
//			// Duplicate email test implementation
//		}

//		public void Dispose()
//		{
//			_report.Dispose();
//		}
//	}
//}
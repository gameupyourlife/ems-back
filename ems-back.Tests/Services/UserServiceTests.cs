//using ems_back.Services;
//using ems_back.Repo.DTOs.User;
//using ems_back.Repo.Models;
//using Moq;
//using FluentAssertions;
//using Xunit;
//using Xunit.Abstractions;
//using Microsoft.AspNetCore.Identity;
//using ems_back.Repo.Interfaces;
//using ems_back.Tests.Utilities;
//using Microsoft.Extensions.Logging;
//using ems_back.Repo.Interfaces.Repository;

//namespace ems_back.Tests.Services
//{
//	public class UserServiceTests : IDisposable
//	{
//		private readonly TestReportGenerator _report;
//		private readonly Mock<UserManager<User>> _userManagerMock;
//		private readonly Mock<IUserRepository> _userRepoMock;
//		private readonly UserService _userService;

//		public UserServiceTests(ITestOutputHelper output)
//		{
//			_report = new TestReportGenerator(output);

//			var store = new Mock<IUserStore<User>>();
//			_userManagerMock = new Mock<UserManager<User>>(
//				store.Object, null, null, null, null, null, null, null, null);

//			_userRepoMock = new Mock<IUserRepository>();
//			_userService = new UserService(
//				_userManagerMock.Object,
//				Mock.Of<SignInManager<User>>(),
//				_userRepoMock.Object,
//				Mock.Of<ILogger<UserService>>());
//		}

//		[Fact]
//		public async Task CreateUserAsync_ValidData_ReturnsUser()
//		{
//			var testName = nameof(CreateUserAsync_ValidData_ReturnsUser);
//			var startTime = DateTime.Now;
//			bool testPassed = false;
//			string message = null;

//			try
//			{
//				// Arrange
//				var userDto = new UserCreateDto
//				{
//					Email = "test@example.com",
//					Password = "ValidP@ss1",
//					FirstName = "Test",
//					LastName = "User"
//				};

//				_userManagerMock.Setup(x => x.FindByEmailAsync(userDto.Email))
//					.ReturnsAsync((User)null);

//				_userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), userDto.Password))
//					.ReturnsAsync(IdentityResult.Success);

//				_userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<UserCreateDto>()))
//					.ReturnsAsync(new UserResponseDto { Email = userDto.Email });

//				// Act
//				var result = await _userService.CreateUserAsync(userDto);

//				// Assert
//				result.Should().NotBeNull();
//				result.Email.Should().Be(userDto.Email);
//				testPassed = true;
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



//		public void Dispose() => _report.Dispose();
//	}
//}
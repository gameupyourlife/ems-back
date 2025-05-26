using AutoMapper;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.Password;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using ems_back.Services;
using ems_back.Tests.Utilities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ems_back.Tests.Services
{
	public class UserServiceTests : IDisposable
	{
		//private readonly TestReportGenerator _report;
		private readonly Mock<UserManager<User>> _userManagerMock;
		private readonly Mock<SignInManager<User>> _signInManagerMock;
		private readonly Mock<IUserRepository> _userRepoMock;
		private readonly Mock<IOrganizationDomainRepository> _orgDomainRepoMock;
		private readonly Mock<IOrganizationUserRepository> _orgMembershipRepoMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly UserService _userService;

		public UserServiceTests(ITestOutputHelper output)
		{
			//_report = new TestReportGenerator(output);

			_userManagerMock = TestHelpers.CreateUserManagerMock<User>();
			_signInManagerMock = TestHelpers.CreateSignInManagerMock<User>();
			_userRepoMock = new Mock<IUserRepository>();
			_orgDomainRepoMock = new Mock<IOrganizationDomainRepository>();
			_orgMembershipRepoMock = new Mock<IOrganizationUserRepository>();
			_mapperMock = new Mock<IMapper>();

			_userService = new UserService(
				_userManagerMock.Object,
				_signInManagerMock.Object,
				_userRepoMock.Object,
				_orgDomainRepoMock.Object,
				_orgMembershipRepoMock.Object,
				Mock.Of<ILogger<UserService>>(),
				_mapperMock.Object);
		}

		[Fact]
		public async Task UpdateUserAsync_ShouldUpdateInRepoAndIdentity()
		{
			// Arrange
			var id = Guid.NewGuid();
			var userDto = new UserUpdateDto { FirstName = "Updated", LastName = "Name" };
			var user = new User { Id = id, FirstName = "Old", LastName = "Name", Email = "test@example.com" };

			_userRepoMock.Setup(r => r.UpdateUserAsync(id, userDto)).ReturnsAsync(new UserResponseDto { Id = id });
			_userManagerMock.Setup(m => m.FindByIdAsync(id.ToString())).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _userService.UpdateUserAsync(id, userDto);

			// Assert
			result.Should().NotBeNull();
			_userManagerMock.Verify(m => m.UpdateAsync(It.IsAny<User>()), Times.Once);
		}

		[Fact]
		public async Task GetUserOrganizationsAsync_ShouldMapEntitiesToDtos()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var orgs = new List<Organization> { new Organization { Id = Guid.NewGuid(), Name = "Org1" } };
			var orgDtos = new List<OrganizationDto> { new OrganizationDto { Name = "Org1" } };

			//_userRepoMock.Setup(r => r.GetUserOrganizationsAsync(userId)).ReturnsAsync(orgs);
			_mapperMock.Setup(m => m.Map<IEnumerable<OrganizationDto>>(orgs)).Returns(orgDtos);

			// Act
			var result = await _userService.GetUserOrganizationsAsync(userId);

			// Assert
			result.Should().HaveCount(1).And.ContainSingle(o => o.Name == "Org1");
		}

		[Fact]
		public async Task HandleAutomaticOrganizationMembership_ShouldAddMembership()
		{
			// Arrange
			var email = "user@company.com";
			var domain = "company.com";
			var userId = Guid.NewGuid();
			var organizationId = Guid.NewGuid();

			var user = new User
			{
				Id = userId,
				Email = email,
				FirstName = "Test",
				LastName = "User"
			};
			var orgDomain = new OrganizationDomain
			{
				OrganizationId = organizationId,
				Domain = "test.com"
			};

			_userManagerMock.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);
			_orgDomainRepoMock.Setup(r => r.GetByDomainAsync(domain)).ReturnsAsync(orgDomain);
			_orgMembershipRepoMock.Setup(r => r.ExistsAsync(userId, organizationId)).ReturnsAsync(false);
			_orgMembershipRepoMock.Setup(r => r.AddAsync(It.IsAny<OrganizationUser>())).Returns(Task.CompletedTask);

			// Act
			await _userService.HandleAutomaticOrganizationMembership(email);

			// Assert
			_orgMembershipRepoMock.Verify(r => r.AddAsync(It.Is<OrganizationUser>(ou =>
				ou.UserId == userId && ou.OrganizationId == organizationId)), Times.Once);
		}

		[Fact]
		public async Task HandleAutomaticOrganizationMembership_ShouldSkip_WhenAlreadyMember()
		{
			// Arrange
			var email = "user@company.com";
			var domain = "company.com";
			var userId = Guid.NewGuid();
			var organizationId = Guid.NewGuid();

			var user = new User
			{
				Id = userId,
				Email = email,
				FirstName = "Test",
				LastName = "User"
			};
			var orgDomain = new OrganizationDomain
			{
				OrganizationId = organizationId,
				Domain = "test.com"
			};

			_userManagerMock.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);
			_orgDomainRepoMock.Setup(r => r.GetByDomainAsync(domain)).ReturnsAsync(orgDomain);
			_orgMembershipRepoMock.Setup(r => r.ExistsAsync(userId, organizationId)).ReturnsAsync(true);

			// Act
			await _userService.HandleAutomaticOrganizationMembership(email);

			// Assert
			_orgMembershipRepoMock.Verify(r => r.AddAsync(It.IsAny<OrganizationUser>()), Times.Never);
		}

		[Fact]
		public async Task CreateUserAsync_ShouldThrow_WhenEmailAlreadyExists()
		{
			// Arrange
			var userDto = new UserCreateDto
			{
				Email = "duplicate@example.com",
				Password = "Test123!",
				FirstName = "Test",
				LastName = "User"
			};

			_userManagerMock.Setup(m => m.FindByEmailAsync(userDto.Email))
				.ReturnsAsync(new User 
				{ 
					Email = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName
                });

			// Act
			var act = async () => await _userService.CreateUserAsync(userDto);

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage($"*{userDto.Email}*");
		}

		[Fact]
		public async Task RestrictedAction_ShouldThrow_WhenUserIsNotInRequiredRole()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test ", LastName = "User ", Email = "test@user.com" };

			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

			// Act
			var act = async () => await _userService.PerformRestrictedAdminAction(userId);

			// Assert
			await act.Should().ThrowAsync<UnauthorizedAccessException>()
				.WithMessage("*nicht autorisiert*");
		}

		[Fact]
		public async Task RestrictedAction_ShouldThrow_WhenUserDoesNotExist()
		{
			// Arrange
			var userId = Guid.NewGuid();
			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((User)null);

			// Act
			var act = async () => await _userService.PerformRestrictedAdminAction(userId);

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("*existiert nicht*");
		}

		[Fact]
		public async Task RestrictedAction_ShouldSucceed_WhenUserIsInRequiredRole()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Admin", LastName = "Test", Email = "admin@user.com" };

			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

			// Act
			var result = await _userService.PerformRestrictedAdminAction(userId);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public async Task GetAllUsersAsync_ShouldReturnMappedUserDtos()
		{
			// Arrange
			var users = new List<User>
			{
				new User { Id = Guid.NewGuid(), Email = "user1@test.com",FirstName = "Test",LastName = "Users"},
				new User {Id = Guid.NewGuid(), Email = "user2@test.com", FirstName = "Test", LastName = "Users"}	
			};

			var userDtos = users.Select(u => new UserResponseDto { Id = u.Id, Email = u.Email ,LastName= "Users" }).ToList();

			_userRepoMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(userDtos);
			_mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDto>>(users)).Returns(userDtos);

			// Act
			var result = await _userService.GetAllUsersAsync();

			// Assert
			result.Should().HaveCount(2);
			result.Should().Contain(dto => dto.Email == "user1@test.com");
			result.Should().Contain(dto => dto.Email == "user2@test.com");
		}

		[Fact]
		public async Task GetUserByIdAsync_ShouldReturnMappedUserDto()
		{
			// Arrange
			var userId = Guid.NewGuid();

			var user = new User { Id = userId, Email = "test@user.com",LastName = "test", FirstName = "USer"};
			var userDto = new UserResponseDto { Id = userId, Email = "test@user.com" };

			_userRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(userDto);
			_mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userDto);

			// Act
			var result = await _userService.GetUserByIdAsync(userId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(userId);
			result.Email.Should().Be("test@user.com");
		}

		[Fact]
		public async Task GetUserByIdAsync_ShouldThrow_WhenUserNotFound()
		{
			// Arrange
			var userId = Guid.NewGuid();
			_userRepoMock.Setup(r => r.GetUserByIdAsync(userId));
				

			// Act
			Func<Task> act = async () => await _userService.GetUserByIdAsync(userId);

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("*not found*");
		}

		[Fact]
		public async Task CreateUserAsync_ShouldCreateUserWithPassword()
		{
			// Arrange
			var user = new User { Email = "new@user.com",FirstName = "Test",LastName = "User"};
			var password = "Test123!";

			_userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync((User)null);
			_userManagerMock.Setup(m => m.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _userService.CreateUserAsync(user, password);

			// Assert
			result.Should().Be(IdentityResult.Success);
			_userManagerMock.Verify(m => m.CreateAsync(user, password), Times.Once);
		}

		[Fact]
		public async Task ResetPasswordAsync_ShouldResetPassword()
		{
			// Arrange
			var resetDto = new PasswordResetDto
			{
				Email = "user@test.com",
				NewPassword = "NewPassword123!",
				token = "reset-token"
			};

			var user = new User {Email = resetDto.Email, FirstName = "Test", LastName = "User" };

			_userManagerMock.Setup(m => m.FindByEmailAsync(resetDto.Email)).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.ResetPasswordAsync(user, resetDto.token, resetDto.NewPassword))
				.ReturnsAsync(IdentityResult.Success);

			// Act
			await _userService.ResetPasswordAsync(resetDto);

			// Assert
			_userManagerMock.Verify(m => m.ResetPasswordAsync(user, resetDto.token, resetDto.NewPassword), Times.Once);
		}

		[Fact]
		public async Task ResetPasswordAsync_ShouldThrow_WhenUserNotFound()
		{
			// Arrange
			var resetDto = new PasswordResetDto
			{
				Email = "nonexistent@test.com",
				NewPassword = "NewPassword123!",
				token = "reset-token"
			};

			_userManagerMock.Setup(m => m.FindByEmailAsync(resetDto.Email)).ReturnsAsync((User)null);

			// Act
			Func<Task> act = async () => await _userService.ResetPasswordAsync(resetDto);

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("*not found*");
		}

		[Fact]
		public async Task DeleteUserAsync_ShouldReturnTrue_WhenUserDeleted()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new User {Id = userId, FirstName = "Test", LastName = "User" };

			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _userService.DeleteUserAsync(userId);

			// Assert
			result.Should().BeTrue();
			_userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
		}

		[Fact]
		public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotFound()
		{
			// Arrange
			var userId = Guid.NewGuid();
			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((User)null);

			// Act
			var result = await _userService.DeleteUserAsync(userId);

			// Assert
			result.Should().BeFalse();
		}

		[Fact]
		public async Task GetUserByEmailAsync_ShouldReturnMappedUserDto()
		{
			// Arrange
			var email = "test@user.com";
			var user = new User { Email = email,FirstName = "Test",LastName = "User"};
			var userDto = new UserResponseDto { Email = email };

			_userRepoMock.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(userDto);
			_mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userDto);

			// Act
			var result = await _userService.GetUserByEmailAsync(email);

			// Assert
			result.Should().NotBeNull();
			result.Email.Should().Be(email);
		}

		[Fact]
		public async Task GetUsersByOrganizationAsync_ShouldReturnMappedUserDtos()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var users = new List<User>
			{
				new User {Id = Guid.NewGuid(), Email = "org1@test.com", FirstName = "Test", LastName = "Users"},
				new User {Id = Guid.NewGuid(), Email = "org2@test.com", FirstName = "Test", LastName = "Users"}
			};

			var userDtos = users.Select(u => new UserResponseDto { Id = u.Id, Email = u.Email }).ToList();

			_userRepoMock.Setup(r => r.GetUsersByOrganizationAsync(orgId)).ReturnsAsync(userDtos

			);
			_mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDto>>(users)).Returns(userDtos);

			// Act
			var result = await _userService.GetUsersByOrganizationAsync(orgId);

			// Assert
			result.Should().HaveCount(2);
			result.Should().Contain(dto => dto.Email == "org1@test.com");
			result.Should().Contain(dto => dto.Email == "org2@test.com");
		}

		[Fact]
		public async Task FindByEmailAsync_ShouldReturnUser()
		{
			// Arrange
			var email = "test@user.com";
			var user = new User {Email = email, FirstName = "Test", LastName = "User" };

			_userManagerMock.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);

			// Act
			var result = await _userService.FindByEmailAsync(email);

			// Assert
			result.Should().NotBeNull();
			result.Email.Should().Be(email);
		}

		[Fact]
		public async Task AddToRoleAsync_ShouldAddUserToRole()
		{
			// Arrange
			var user = new User { FirstName = "Test", LastName = "User" };
			var role = "Admin";

			_userManagerMock.Setup(m => m.AddToRoleAsync(user, role)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _userService.AddToRoleAsync(user, role);

			// Assert
			result.Should().Be(IdentityResult.Success);
			_userManagerMock.Verify(m => m.AddToRoleAsync(user, role), Times.Once);
		}

		[Fact]
		public async Task CheckPasswordSignInAsync_ShouldReturnSignInResult()
		{
			// Arrange
			var user = new User {FirstName = "Test", LastName = "User" };

			var password = "password";
			var signInResult = SignInResult.Success;

			_signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, password, false))
				.ReturnsAsync(signInResult);

			// Act
			var result = await _userService.CheckPasswordSignInAsync(user, password);

			// Assert
			result.Should().Be(signInResult);
		}

		[Fact]
		public async Task DeleteUserByIdOrEmailAsync_ShouldDeleteById_WhenIdProvided()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new User {Id = userId, FirstName = "Test", LastName = "User" };

			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _userService.DeleteUserByIdOrEmailAsync(userId, null);

			// Assert
			result.Should().BeTrue();
			_userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
		}

		[Fact]
		public async Task DeleteUserByIdOrEmailAsync_ShouldDeleteByEmail_WhenEmailProvided()
		{
			// Arrange
			var email = "delete@me.com";
			var user = new User {Email = email, FirstName = "Test", LastName = "User"	 };

			_userManagerMock.Setup(m => m.FindByEmailAsync(email)).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

			// Act
			var result = await _userService.DeleteUserByIdOrEmailAsync(null, email);

			// Assert
			result.Should().BeTrue();
			_userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
		}

		[Fact]
		public async Task DeleteUserByIdOrEmailAsync_ShouldReturnFalse_WhenNeitherProvided()
		{
			// Act
			var result = await _userService.DeleteUserByIdOrEmailAsync(null, null);

			// Assert
			result.Should().BeFalse();
		}

		[Fact]
		public async Task IsUserInOrgOrAdmin_ShouldReturnTrue_WhenUserIsAdmin()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "User",LastName = "Test"};

			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

			// Act
			var result = await _userService.IsUserInOrgOrAdmin(orgId, userId);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public async Task IsUserInOrgOrAdmin_ShouldReturnTrue_WhenUserIsInOrg()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User {Id = userId, FirstName = "Test", LastName = "User" };

			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
			_orgMembershipRepoMock.Setup(r => r.ExistsAsync(userId, orgId)).ReturnsAsync(true);

			// Act
			var result = await _userService.IsUserInOrgOrAdmin(orgId, userId);

			// Assert
			result.Should().BeTrue();
		}

		[Fact]
		public async Task IsUserInOrgOrAdmin_ShouldReturnFalse_WhenNeitherAdminNorInOrg()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };

			_userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
			_userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
			_orgMembershipRepoMock.Setup(r => r.ExistsAsync(userId, orgId)).ReturnsAsync(false);

			// Act
			var result = await _userService.IsUserInOrgOrAdmin(orgId, userId);

			// Assert
			result.Should().BeFalse();

		}
		
		public void Dispose() => Dispose();

	}
}
using AutoMapper;
using ems_back.Repo.DTOs.Organization;
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
		private readonly TestReportGenerator _report;
		private readonly Mock<UserManager<User>> _userManagerMock;
		private readonly Mock<SignInManager<User>> _signInManagerMock;
		private readonly Mock<IUserRepository> _userRepoMock;
		private readonly Mock<IOrganizationDomainRepository> _orgDomainRepoMock;
		private readonly Mock<IOrganizationUserRepository> _orgMembershipRepoMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly UserService _userService;

		public UserServiceTests(ITestOutputHelper output)
		{
			_report = new TestReportGenerator(output);

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

			var user = new User { Id = userId, Email = email };
			var orgDomain = new OrganizationDomain { OrganizationId = organizationId };

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

			var user = new User { Id = userId, Email = email };
			var orgDomain = new OrganizationDomain { OrganizationId = organizationId };

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
				.ReturnsAsync(new User { Email = userDto.Email });

			// Act
			var act = async () => await _userService.CreateUserAsync(userDto);

			// Assert
			await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage($"*{userDto.Email}*");
		}

		public void Dispose() => _report.Dispose();
	}
}

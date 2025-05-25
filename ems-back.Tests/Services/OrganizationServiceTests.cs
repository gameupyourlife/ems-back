using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Domain;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using ems_back.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ems_back.Tests.Helper;
using Xunit;

namespace ems_back.Tests.Services
{
	public class OrganizationServiceTests
	{
		private readonly Mock<IOrganizationRepository> _mockOrgRepo;
		private readonly Mock<IOrganizationUserRepository> _mockOrgUserRepo;
		private readonly Mock<IOrganizationDomainRepository> _mockOrgDomainRepo;
		private readonly Mock<UserManager<User>> _mockUserManager;
		private readonly Mock<ILogger<OrganizationService>> _mockLogger;
		private readonly Mock<IUserRepository> _mockUserRepo;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<IUserService> _mockUserService;
		private readonly OrganizationService _service;
	


		public OrganizationServiceTests()
		{
			_mockOrgRepo = new Mock<IOrganizationRepository>();
			_mockOrgUserRepo = new Mock<IOrganizationUserRepository>();
			_mockOrgDomainRepo = new Mock<IOrganizationDomainRepository>();
			_mockUserManager = new Mock<UserManager<User>>(
				Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
			_mockLogger = new Mock<ILogger<OrganizationService>>();
			_mockUserRepo = new Mock<IUserRepository>();
			_mockMapper = new Mock<IMapper>();
			_mockUserService = new Mock<IUserService>();
			
			_service = new OrganizationService(
				_mockUserService.Object,
			null,
				_mockOrgRepo.Object,
				_mockOrgUserRepo.Object,

				_mockOrgDomainRepo.Object,
				_mockUserRepo.Object,
				_mockUserManager.Object,
				_mockMapper.Object,
				_mockLogger.Object);
		}

		#region GetAllOrganizationsAsync Tests

		[Fact]
		public async Task GetAllOrganizationsAsync_AdminUser_ReturnsOrganizations()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
			var orgs = new List<OrganizationResponseDto> { new OrganizationResponseDto() };

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x => x.GetAllOrganizationsAsync())
				.ReturnsAsync(orgs);

			// Act
			var result = await _service.GetAllOrganizationsAsync(userId);

			// Assert
			Assert.Single(result);
		}

		[Fact]
		public async Task GetAllOrganizationsAsync_NonAdminUser_ThrowsUnauthorized()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(false);

			// Act & Assert
			await Assert.ThrowsAsync<UnauthorizedAccessException>(
				() => _service.GetAllOrganizationsAsync(userId));
		}

		[Fact]
		public async Task GetAllOrganizationsAsync_UserNotFound_ThrowsUnauthorized()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync((User)null);

			// Act & Assert
			await Assert.ThrowsAsync<UnauthorizedAccessException>(
				() => _service.GetAllOrganizationsAsync(userId));
		}

		#endregion

		#region GetOrganizationByIdAsync Tests

		[Fact]
		public async Task GetOrganizationByIdAsync_ValidId_ReturnsOrganization()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var expectedOrg = new OrganizationResponseDto { Id = orgId };

			_mockOrgRepo.Setup(x => x.GetOrganizationByIdAsync(orgId))
				.ReturnsAsync(expectedOrg);

			// Act
			var result = await _service.GetOrganizationByIdAsync(orgId);

			// Assert
			Assert.Equal(orgId, result.Id);
		}

		[Fact]
		public async Task GetOrganizationByIdAsync_InvalidId_ReturnsNull()
		{
			// Arrange
			var orgId = Guid.NewGuid();

			_mockOrgRepo.Setup(x => x.GetOrganizationByIdAsync(orgId))
				.ReturnsAsync((OrganizationResponseDto)null);

			// Act
			var result = await _service.GetOrganizationByIdAsync(orgId);

			// Assert
			Assert.Null(result);
		}

		#endregion

		#region CreateOrganizationAsync Tests

		[Fact]
		public async Task CreateOrganizationAsync_ValidData_CreatesOrganization()
		{
			// Arrange
			var orgDto = new OrganizationCreateDto { Domain = "test.com" };
			var expectedOrg = new OrganizationResponseDto { Domains = new List<string>() { "test.com" } };

			_mockOrgRepo.Setup(x => x.DomainExistsAsync(orgDto.Domain))
				.ReturnsAsync(false);
			_mockOrgRepo.Setup(x => x.CreateOrganizationAsync(orgDto))
				.ReturnsAsync(expectedOrg);

			// Act
			var result = await _service.CreateOrganizationAsync(orgDto);

			// Assert
			Assert.Equal(orgDto.Domain, result.PrimaryDomain);
		}

		[Fact]
		public async Task CreateOrganizationAsync_DuplicateDomain_ThrowsConflict()
		{
			// Arrange
			var orgDto = new OrganizationCreateDto { Domain = "test.com" };

			_mockOrgRepo.Setup(x => x.DomainExistsAsync(orgDto.Domain))
				.ReturnsAsync(true);

			// Act & Assert
			await Assert.ThrowsAsync<DomainConflictException>(
				() => _service.CreateOrganizationAsync(orgDto));
		}

		#endregion

		#region UpdateOrganizationAsync Tests

		[Fact]
		public async Task UpdateOrganizationAsync_AdminUser_UpdatesOrganization()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
			var orgDto = new OrganizationUpdateDto { Name = "Updated" };
			var org = new Organization { Id = orgId, Name = "Original" };
			var expectedResponse = new OrganizationResponseDto { Id = orgId, Name = "Updated" };

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x =>
					x.GetByIdAsync(orgId, It.IsAny<Func<IQueryable<Organization>, IQueryable<Organization>>>()))
				.ReturnsAsync(org);
			_mockMapper.Setup(x => x.Map(orgDto, org))
				.Verifiable();
			_mockOrgRepo.Setup(x => x.UpdateAsync(org))
				.Returns(Task.CompletedTask);
			_mockMapper.Setup(x => x.Map<OrganizationResponseDto>(org))
				.Returns(expectedResponse);

			// Act
			var result = await _service.UpdateOrganizationAsync(orgId, orgDto, userId);

			// Assert
			Assert.Equal("Updated", result.Name);
			_mockMapper.Verify();
		}

		[Fact]
		public async Task UpdateOrganizationAsync_OwnerUser_UpdatesOrganization()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
			var orgDto = new OrganizationUpdateDto { Name = "Updated" };
			var org = new Organization { Id = orgId, Name = "Original" };

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(false);
			_mockOrgUserRepo.Setup(x => x.IsUserOrganizationOwner(userId, orgId))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x =>
					x.GetByIdAsync(orgId, It.IsAny<Func<IQueryable<Organization>, IQueryable<Organization>>>()))
				.ReturnsAsync(org);

			// Act
			await _service.UpdateOrganizationAsync(orgId, orgDto, userId);

			// Assert
			_mockOrgRepo.Verify(x => x.UpdateAsync(org), Times.Once);
		}

		[Fact]
		public async Task UpdateOrganizationAsync_UnauthorizedUser_ThrowsException()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
			var orgDto = new OrganizationUpdateDto();

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(false);
			_mockOrgUserRepo.Setup(x => x.IsUserOrganizationOwner(userId, orgId))
				.ReturnsAsync(false);

			// Act & Assert
			await Assert.ThrowsAsync<UnauthorizedAccessException>(
				() => _service.UpdateOrganizationAsync(orgId, orgDto, userId));
		}

		[Fact]
		public async Task UpdateOrganizationAsync_OrganizationNotFound_ReturnsNull()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
			var orgDto = new OrganizationUpdateDto();

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x =>
					x.GetByIdAsync(orgId, It.IsAny<Func<IQueryable<Organization>, IQueryable<Organization>>>()))
				.ReturnsAsync((Organization)null);

			// Act
			var result = await _service.UpdateOrganizationAsync(orgId, orgDto, userId);

			// Assert
			Assert.Null(result);
		}

		#endregion

		#region DeleteOrganizationAsync Tests

		[Fact]
		public async Task DeleteOrganizationAsync_AdminUser_DeletesOrganization()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x => x.DeleteOrganizationAsync(orgId, userId))
				.ReturnsAsync(true);

			// Act
			var result = await _service.DeleteOrganizationAsync(userId, orgId);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task DeleteOrganizationAsync_NonAdminUser_ThrowsException()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };

			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(false);

			// Act & Assert
			await Assert.ThrowsAsync<UnauthorizedAccessException>(
				() => _service.DeleteOrganizationAsync(userId, orgId));
		}

		#endregion

		#region GetOrganizationDomainsAsync Tests

		[Fact]
		public async Task GetOrganizationDomainsAsync_AdminUser_ReturnsDomains()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
			var domains = new List<string> { "test.com" };

			_mockOrgRepo.Setup(x => x.OrganizationExistsAsync(orgId))
				.ReturnsAsync(true);
			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x => x.GetOrganizationDomainsAsync(orgId))
				.ReturnsAsync(domains);

			// Act
			var result = await _service.GetOrganizationDomainsAsync(orgId, userId);

			// Assert
			Assert.Single(result);
		}

		[Fact]
		public async Task GetOrganizationDomainsAsync_OwnerUser_ReturnsDomains()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };
			var domains = new List<string> { "test.com" };

			_mockOrgRepo.Setup(x => x.OrganizationExistsAsync(orgId))
				.ReturnsAsync(true);
			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(false);
			_mockOrgUserRepo.Setup(x => x.IsUserOrganizationOwner(userId, orgId))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x => x.GetOrganizationDomainsAsync(orgId))
				.ReturnsAsync(domains);

			// Act
			var result = await _service.GetOrganizationDomainsAsync(orgId, userId);

			// Assert
			Assert.Single(result);
		}

		[Fact]
		public async Task GetOrganizationDomainsAsync_UnauthorizedUser_ThrowsException()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };

			_mockOrgRepo.Setup(x => x.OrganizationExistsAsync(orgId))
				.ReturnsAsync(true);
			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(false);
			_mockOrgUserRepo.Setup(x => x.IsUserOrganizationOwner(userId, orgId))
				.ReturnsAsync(false);

			// Act & Assert
			await Assert.ThrowsAsync<UnauthorizedAccessException>(
				() => _service.GetOrganizationDomainsAsync(orgId, userId));
		}

		[Fact]
		public async Task GetOrganizationDomainsAsync_OrgNotFound_ReturnsNull()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			_mockOrgRepo.Setup(x => x.OrganizationExistsAsync(orgId))
				.ReturnsAsync(false);

			// Act
			var result = await _service.GetOrganizationDomainsAsync(orgId, userId);

			// Assert
			Assert.Null(result);
		}

		#endregion

		#region AddDomainToOrganizationAsync Tests

		[Fact]
		public async Task AddDomainToOrganizationAsync_ValidData_AddsDomain()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var domain = "test.com";
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };

			_mockOrgRepo.Setup(x => x.OrganizationExistsAsync(orgId))
				.ReturnsAsync(true);
			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x => x.IsDomainAvailableAsync(domain, orgId))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x => x.AddDomainToOrganizationAsync(orgId, domain))
				.ReturnsAsync(true);

			// Act
			var result = await _service.AddDomainToOrganizationAsync(orgId, domain, userId);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task AddDomainToOrganizationAsync_DomainExists_ReturnsFalse()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var domain = "test.com";
			var user = new User { Id = userId, FirstName = "Test", LastName = "User" };

			_mockOrgRepo.Setup(x => x.OrganizationExistsAsync(orgId))
				.ReturnsAsync(true);
			_mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
				.ReturnsAsync(user);
			_mockUserManager.Setup(x => x.IsInRoleAsync(user, nameof(UserRole.Admin)))
				.ReturnsAsync(true);
			_mockOrgRepo.Setup(x => x.IsDomainAvailableAsync(domain, orgId))
				.ReturnsAsync(false);

			// Act
			var result = await _service.AddDomainToOrganizationAsync(orgId, domain, userId);

			// Assert
			Assert.False(result);
		}

		#endregion

		#region GetUsersByOrganizationAsync Tests

		[Fact]
		public async Task GetUsersByOrganizationAsync_ReturnsUsers()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var users = new List<UserResponseDto> { new UserResponseDto() };

			_mockUserRepo.Setup(x => x.GetUsersByOrganizationAsync(orgId))
				.ReturnsAsync(users);

			// Act
			var result = await _service.GetUsersByOrganizationAsync(orgId);

			// Assert
			Assert.Single(result);
		}

		#endregion

		#region HandleAutomaticOrganizationMembership Tests

		[Fact]
		public async Task HandleAutomaticOrganizationMembership_ValidEmail_AddsUser()
		{
			// Arrange
			var email = "user@test.com";
			var domain = "test.com";
			var orgDomain = new OrganizationDomain { OrganizationId = Guid.NewGuid(), Domain = "test.com" };
			var user = new User { Id = Guid.NewGuid(), Email = email, LastName = "Test", FirstName = "User" };

			_mockOrgDomainRepo.Setup(x => x.GetByDomainAsync(domain))
				.ReturnsAsync(orgDomain);
			_mockUserManager.Setup(x => x.FindByEmailAsync(email))
				.ReturnsAsync(user);
			_mockOrgUserRepo.Setup(x => x.ExistsAsync(user.Id, orgDomain.OrganizationId))
				.ReturnsAsync(false);

			// Act
			await _service.HandleAutomaticOrganizationMembership(email);

			// Assert
			_mockOrgUserRepo.Verify(x => x.AddAsync(It.Is<OrganizationUser>(ou =>
					ou.UserId == user.Id && ou.OrganizationId == orgDomain.OrganizationId)),
				Times.Once);
		}

		[Fact]
		public async Task HandleAutomaticOrganizationMembership_NoDomain_DoesNothing()
		{
			// Arrange
			var email = "user@test.com";
			var domain = "test.com";

			_mockOrgDomainRepo.Setup(x => x.GetByDomainAsync(domain))
				.ReturnsAsync((OrganizationDomain)null);

			// Act
			await _service.HandleAutomaticOrganizationMembership(email);

			// Assert
			_mockOrgUserRepo.Verify(x => x.AddAsync(It.IsAny<OrganizationUser>()), Times.Never);
		}

		#endregion

		#region GetOrganizationMembersAsync Tests

		[Fact]
		public async Task GetOrganizationMembersAsync_OrgExists_ReturnsMembers()
		{
			// Arrange
			var orgId = Guid.NewGuid();
			var members = new List<UserResponseDto> { new UserResponseDto() };

			_mockOrgRepo.Setup(x => x.OrganizationExistsAsync(orgId))
				.ReturnsAsync(true);
			_mockOrgUserRepo.Setup(x => x.GetUsersByOrganizationAsync(orgId))
				.ReturnsAsync(members);

			// Act
			var result = await _service.GetOrganizationMembersAsync(orgId);

			// Assert
			Assert.Single(result);
		}

		[Fact]
		public async Task GetOrganizationMembersAsync_OrgNotExists_ReturnsEmpty()
		{
			// Arrange
			var orgId = Guid.NewGuid();

			_mockOrgRepo.Setup(x => x.OrganizationExistsAsync(orgId))
				.ReturnsAsync(false);

			// Act
			var result = await _service.GetOrganizationMembersAsync(orgId);

			// Assert
			Assert.Empty(result);
		}

		
			#endregion
		}
}
//using Xunit;
//using Moq;
//using AutoMapper;
//using ems_back.Repo.DTOs.Email;
//using ems_back.Repo.Interfaces.Repository;
//using ems_back.Repo.Models;
//using ems_back.Repo.Services;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using ems_back.Repo.Models.Types;
//using System.ComponentModel.DataAnnotations;

//namespace ems_back.Repo.Services.Tests
//{
//	public class MailTemplateServiceTests : IDisposable
//	{
//		private readonly Mock<IMailTemplateRepository> _mockRepo;
//		private readonly Mock<IMapper> _mockMapper;
//		private readonly Mock<UserManager<User>> _mockUserManager;
//		private readonly Mock<IOrganizationRepository> _mockOrgRepo;
//		private readonly Mock<ILogger<MailTemplateService>> _mockLogger;
//		private readonly MailTemplateService _service;

//		private readonly Guid _testOrgId = Guid.NewGuid();
//		private readonly Guid _testUserId = Guid.NewGuid();
//		private readonly Guid _testTemplateId = Guid.NewGuid();

//		public MailTemplateServiceTests()
//		{
//			_mockRepo = new Mock<IMailTemplateRepository>();
//			_mockMapper = new Mock<IMapper>();
//			_mockUserManager = new Mock<UserManager<User>>(
//				Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
//			_mockOrgRepo = new Mock<IOrganizationRepository>();
//			_mockLogger = new Mock<ILogger<MailTemplateService>>();

//			_service = new MailTemplateService(
//				_mockRepo.Object,
//				_mockMapper.Object,
//				_mockUserManager.Object,
//				_mockOrgRepo.Object,
//				_mockLogger.Object);
//		}

//		public void Dispose()
//		{
//			// Cleanup if needed
//		}

//		#region GetTemplateAsync Tests
//		[Fact]
//		public async Task GetTemplateAsync_ExistingId_ReturnsTemplate()
//		{
//			// Arrange
//			var template = CreateTestMailTemplate();
//			var mailDto = CreateTestMailDto();

//			_mockRepo.Setup(r => r.GetByIdAsync(_testTemplateId))
//				.ReturnsAsync(template);
//			_mockMapper.Setup(m => m.Map<MailDto>(template))
//				.Returns(mailDto);

//			// Act
//			var result = await _service.GetTemplateAsync(_testTemplateId);

//			// Assert
//			Assert.Equal(_testTemplateId, result.Id);
//			_mockRepo.Verify(r => r.GetByIdAsync(_testTemplateId), Times.Once);
//		}

//		[Fact]
//		public async Task GetTemplateAsync_NonExistentId_ReturnsNull()
//		{
//			// Arrange
//			_mockRepo.Setup(r => r.GetByIdAsync(_testTemplateId))
//				.ReturnsAsync((MailTemplate)null);

//			// Act
//			var result = await _service.GetTemplateAsync(_testTemplateId);

//			// Assert
//			Assert.Null(result);
//		}
//		#endregion

//		#region GetTemplatesForOrganizationAsync Tests
//		[Fact]
//		public async Task GetTemplatesForOrganizationAsync_ValidOrgId_ReturnsTemplates()
//		{
//			// Arrange
//			var templates = new List<MailTemplate> { CreateTestMailTemplate() };
//			var mailDtos = new List<MailDto> { CreateTestMailDto() };

//			_mockOrgRepo.Setup(o => o.OrganizationExistsAsync(_testOrgId))
//				.ReturnsAsync(true);
//			_mockRepo.Setup(r => r.GetByOrganizationAsync(_testOrgId))
//				.ReturnsAsync(templates);
//			_mockMapper.Setup(m => m.Map<IEnumerable<MailDto>>(templates))
//				.Returns(mailDtos);

//			// Act
//			var result = await _service.GetTemplatesForOrganizationAsync(_testOrgId);

//			// Assert
//			Assert.Single(result);
//			_mockRepo.Verify(r => r.GetByOrganizationAsync(_testOrgId), Times.Once);
//		}

//		[Fact]
//		public async Task GetTemplatesForOrganizationAsync_NonExistentOrg_ThrowsKeyNotFoundException()
//		{
//			// Arrange
//			_mockOrgRepo.Setup(o => o.OrganizationExistsAsync(_testOrgId))
//				.ReturnsAsync(false);

//			// Act & Assert
//			await Assert.ThrowsAsync<KeyNotFoundException>(() =>
//				_service.GetTemplatesForOrganizationAsync(_testOrgId));
//		}
//		#endregion

//		#region CreateTemplateAsync Tests
//		[Fact]
//		public async Task CreateTemplateAsync_ValidInput_CreatesTemplate()
//		{
//			// Arrange
//			var dto = CreateTestCreateMailDto();
//			var template = CreateTestMailTemplate();
//			var mailDto = CreateTestMailDto();

//			SetupUserWithPermission();
//			_mockOrgRepo.Setup(o => o.OrganizationExistsAsync(_testOrgId))
//				.ReturnsAsync(true);
//			_mockMapper.Setup(m => m.Map<MailTemplate>(dto))
//				.Returns(template);
//			_mockRepo.Setup(r => r.CreateAsync(template))
//				.ReturnsAsync(template);
//			_mockMapper.Setup(m => m.Map<MailDto>(template))
//				.Returns(mailDto);

//			// Act
//			var result = await _service.CreateTemplateAsync(_testOrgId, _testUserId, dto);

//			// Assert
//			Assert.Equal(template.Id, result.Id);
//			Assert.Equal(_testOrgId, template.OrganizationId);
//			Assert.Equal(_testUserId, template.CreatedBy);
//			_mockRepo.Verify(r => r.CreateAsync(template), Times.Once);
//		}

//		[Fact]
//		public async Task CreateTemplateAsync_UserWithoutPermission_ThrowsUnauthorized()
//		{
//			// Arrange
//			var dto = CreateTestCreateMailDto();
//			SetupUserWithoutPermission();

//			// Act & Assert
//			await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
//				_service.CreateTemplateAsync(_testOrgId, _testUserId, dto));
//		}

//		[Fact]
//		public async Task CreateTemplateAsync_MissingRequiredFields_ThrowsValidationException()
//		{
//			// Arrange
//			var invalidDto = new CreateMailDto
//			{
//				Name = "", // Still invalid but satisfies 'required'
//				Subject = "", // Still invalid but satisfies 'required'
//				Body = "", // Still invalid but satisfies 'required'
//				sendToAllParticipants = false // Required
//			};
//			SetupUserWithPermission();

//			// Act & Assert
//			await Assert.ThrowsAsync<ValidationException>(() =>
//				_service.CreateTemplateAsync(_testOrgId, _testUserId, invalidDto));
//		}
//		#endregion

//		#region UpdateTemplateAsync Tests
//		[Fact]
//		public async Task UpdateTemplateAsync_ValidInput_UpdatesTemplate()
//		{
//			// Arrange
//			var dto = CreateTestCreateMailDto();
//			var existingTemplate = CreateTestMailTemplate();
//			var updatedTemplate = CreateTestMailTemplate();
//			var mailDto = CreateTestMailDto();

//			SetupUserWithPermission();
//			_mockRepo.Setup(r => r.GetByIdAsync(_testTemplateId))
//				.ReturnsAsync(existingTemplate);
//			_mockRepo.Setup(r => r.UpdateAsync(existingTemplate))
//				.ReturnsAsync(updatedTemplate);
//			_mockMapper.Setup(m => m.Map<MailDto>(updatedTemplate))
//				.Returns(mailDto);

//			// Act
//			var result = await _service.UpdateTemplateAsync(_testTemplateId, _testUserId, dto);

//			// Assert
//			Assert.Equal(updatedTemplate.Id, result.Id);
//			Assert.Equal(_testUserId, existingTemplate.UpdatedBy);
//			Assert.NotNull(existingTemplate.UpdatedAt);
//			_mockRepo.Verify(r => r.UpdateAsync(existingTemplate), Times.Once);
//		}

//		[Fact]
//		public async Task UpdateTemplateAsync_NonExistentTemplate_ThrowsKeyNotFoundException()
//		{
//			// Arrange
//			var dto = CreateTestCreateMailDto();
//			SetupUserWithPermission();
//			_mockRepo.Setup(r => r.GetByIdAsync(_testTemplateId))
//				.ReturnsAsync((MailTemplate)null);

//			// Act & Assert
//			await Assert.ThrowsAsync<KeyNotFoundException>(() =>
//				_service.UpdateTemplateAsync(_testTemplateId, _testUserId, dto));
//		}
//		#endregion

//		#region DeleteTemplateAsync Tests
//		[Fact]
//		public async Task DeleteTemplateAsync_ExistingId_ReturnsTrue()
//		{
//			// Arrange
//			SetupUserWithPermission();
//			_mockRepo.Setup(r => r.ExistsAsync(_testTemplateId))
//				.ReturnsAsync(true);
//			_mockRepo.Setup(r => r.DeleteAsync(_testTemplateId))
//				.ReturnsAsync(true);

//			// Act
//			var result = await _service.DeleteTemplateAsync(_testTemplateId, _testUserId);

//			// Assert
//			Assert.True(result);
//			_mockRepo.Verify(r => r.DeleteAsync(_testTemplateId), Times.Once);
//		}

//		[Fact]
//		public async Task DeleteTemplateAsync_NonExistentId_ReturnsFalse()
//		{
//			// Arrange
//			SetupUserWithPermission();
//			_mockRepo.Setup(r => r.ExistsAsync(_testTemplateId))
//				.ReturnsAsync(false);

//			// Act
//			var result = await _service.DeleteTemplateAsync(_testTemplateId, _testUserId);

//			// Assert
//			Assert.False(result);
//		}
//		#endregion

//		#region Helper Methods
//		private void SetupUserWithPermission()
//		{
//			var user = new User { Id = _testUserId, FirstName = " ",LastName = " "};
			
//			_mockUserManager.Setup(u => u.FindByIdAsync(_testUserId.ToString()))
//				.ReturnsAsync(user);
//			_mockUserManager.Setup(u => u.GetRolesAsync(user))
//				.ReturnsAsync(new List<string> { nameof(UserRole.Admin) });
//		}

//		private void SetupUserWithoutPermission()
//		{
//			var user = new User { Id = _testUserId, FirstName = " ", LastName = " " };
//			_mockUserManager.Setup(u => u.FindByIdAsync(_testUserId.ToString()))
//				.ReturnsAsync(user);
//			_mockUserManager.Setup(u => u.GetRolesAsync(user))
//				.ReturnsAsync(new List<string>());
//		}

//		private MailTemplate CreateTestMailTemplate(Guid? id = null)
//		{
//			return new MailTemplate
//			{
//				Id = id ?? _testTemplateId,
//				Name = "Test Template",
//				Subject = "Test Subject",
//				Description = "Test Description",
//				Body = "<p>Test Body</p>",
//				Recipients = new List<Guid> { Guid.NewGuid() },
//				ScheduledFor = DateTime.UtcNow.AddDays(1),
//				isUserCreated = true,
//				SendToAllParticipants = false,
//				OrganizationId = _testOrgId,
//				CreatedBy = _testUserId,
//				CreatedAt = DateTime.UtcNow.AddDays(-1),
//				UpdatedBy = null,
//				UpdatedAt = null
//			};
//		}

//		private MailDto CreateTestMailDto(Guid? id = null)
//		{
//			return new MailDto
//			{
//				Id = id ?? _testTemplateId,
//				Name = "Test Template",
//				Subject = "Test Subject",
//				Description = "Test Description",
//				Body = "<p>Test Body</p>",
//				Recipients = new List<Guid> { Guid.NewGuid() },
//				ScheduledFor = DateTime.UtcNow.AddDays(1),
//				IsUserCreated = true,
//				sendToAllParticipants = false,
//				CreatedAt = DateTime.UtcNow.AddDays(-1),
//				UpdatedAt = DateTime.UtcNow.AddDays(-1),
//				CreatedBy = _testUserId,
//				UpdatedBy = null
//			};
//		}

//		private CreateMailDto CreateTestCreateMailDto()
//		{
//			return new CreateMailDto
//			{
//				Name = "Test Template",
//				Subject = "Test Subject",
//				Description = "Test Description",
//				Body = "<p>Test Body</p>",
//				Recipients = new List<Guid> { Guid.NewGuid() },
//				ScheduledFor = DateTime.UtcNow.AddDays(1),
//				sendToAllParticipants = false
//			};
//		}
//		#endregion
//	}
//}
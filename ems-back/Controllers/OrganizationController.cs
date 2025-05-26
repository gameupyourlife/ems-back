using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using ems_back.Repo.DTOs.Domain;
using ems_back.Services;
using Microsoft.AspNetCore.Authorization;
using ems_back.Repo.Models.Types;

namespace ems_back.Controllers
{
	[Route("api/orgs")]
	[ApiController]
	public class OrganizationsController : ControllerBase
	{
		private readonly IOrganizationService _organizationService;
		private readonly ILogger<OrganizationsController> _logger;

		public OrganizationsController(
			IOrganizationService organizationService,
			ILogger<OrganizationsController> logger)
		{
			_organizationService = organizationService;
			_logger = logger;
		}

		private string? GetAuthenticatedUserId()
		{
			return User.FindFirstValue(ClaimTypes.NameIdentifier);
		} 
		
		// GET: api/orgs
		[HttpGet]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
		public async Task<ActionResult<IEnumerable<OrganizationResponseDto>>> GetOrganizations()
		{
			try
			{
				var userIdStr = GetAuthenticatedUserId();
				if (string.IsNullOrEmpty(userIdStr))
				{
					_logger.LogWarning("Unauthorized access attempt to GetOrganizations");
					return Unauthorized();
				}

				var userId = Guid.Parse(userIdStr);

				_logger.LogInformation("User {UserId} requested organization list", userId);

				var organizations = await _organizationService.GetAllOrganizationsAsync(userId);

				_logger.LogInformation("User {UserId} retrieved {Count} organizations", userId, organizations.Count());

				return Ok(organizations);
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Unauthorized access in GetOrganizations");
				return Forbid();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all organizations");
				return StatusCode(500, "Internal server error");
			}
		}

		// POST: api/orgs
		[HttpPost]
		public async Task<ActionResult<OrganizationResponseDto>> CreateOrganization(
			[FromBody] OrganizationCreateDto organizationDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var createdOrg = await _organizationService.CreateOrganizationAsync(organizationDto);

				return CreatedAtAction(
					nameof(GetOrganization),
					new { orgId = createdOrg.Id },
					createdOrg);
			}
			catch (DomainConflictException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating organization");
				return StatusCode(500, "Internal server error");
			}
		}

		// GET: api/orgs/{orgId}
		[HttpGet("{orgId}")]
		public async Task<ActionResult<OrganizationResponseDto>> GetOrganization(Guid orgId)
		{
			try
			{
				var organization = await _organizationService.GetOrganizationByIdAsync(orgId);
				return organization == null ? NotFound() : Ok(organization);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting organization {OrgId}", orgId);
				return StatusCode(500, "Internal server error");
			}
		}

		// PUT: api/orgs/{orgId}
		[HttpPut("{orgId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateOrganization(
			Guid orgId,
			[FromBody] OrganizationUpdateDto organizationDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var updatedByUserId = GetAuthenticatedUserId();
				if (updatedByUserId == null)
				{
					return Unauthorized("User not authenticated");
				}

				var result = await _organizationService.UpdateOrganizationAsync(
					orgId,
					organizationDto,
					Guid.Parse(updatedByUserId));

				return result == null ? NotFound() : NoContent();
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning(ex, "Invalid argument in organization update");
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating organization {OrgId}", orgId);
				return StatusCode(500, "Internal server error");
			}
		}

		// DELETE: api/orgs/{orgId}
		[HttpDelete("{orgId}")]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
		public async Task<IActionResult> DeleteOrganization(Guid orgId)
		{
			try
			{
				var userIdStr = GetAuthenticatedUserId();
				if (string.IsNullOrEmpty(userIdStr))
				{
					_logger.LogWarning("Unauthorized delete attempt for organization {OrgId}", orgId);
					return Unauthorized("User not authenticated");
				}

				var userId = Guid.Parse(userIdStr);

				_logger.LogInformation("User {UserId} is attempting to delete organization {OrgId}", userId, orgId);

				var success = await _organizationService.DeleteOrganizationAsync(userId, orgId);

				if (success)
				{
					_logger.LogInformation("Organization {OrgId} was successfully deleted by user {UserId}", orgId, userId);
					return Ok(new { message = "Organization deleted successfully" });
				}
				else
				{
					_logger.LogWarning("Attempt to delete organization {OrgId} by user {UserId} failed: Not found", orgId, userId);
					return NotFound();
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.LogWarning(ex, "Unauthorized delete attempt for organization {OrgId}", orgId);
				return Forbid();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error deleting organization {OrgId}", orgId);
				return Unauthorized("Organization cannot be deleted");
			}
		}


		// DOMAIN MANAGEMENT ENDPOINTS

		// GET: api/orgs/{orgId}/domains
		[HttpGet("{orgId}/domains")]
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]
		public async Task<ActionResult<IEnumerable<string>>> GetOrganizationDomains(Guid orgId)
		{
			try
			{
				var userIdStr = GetAuthenticatedUserId();
				if (string.IsNullOrEmpty(userIdStr))
				{
					_logger.LogWarning("Unauthorized domain access attempt for organization {OrgId}", orgId);
					return Unauthorized("User not authenticated");
				}

				var userId = Guid.Parse(userIdStr);
				_logger.LogInformation("User {UserId} is attempting to fetch domains for organization {OrgId}", userId, orgId);

				var domains = await _organizationService.GetOrganizationDomainsAsync(orgId, userId);
				if (domains == null)
				{
					return NotFound($"Organization with ID {orgId} not found.");
				}

				return Ok(domains);
			}
			catch (UnauthorizedAccessException)
			{
				_logger.LogWarning("User not authorized to access domains of organization {OrgId}", orgId);
				return Forbid(); // 403
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving domains for organization {OrgId}", orgId);
				return StatusCode(500, "Internal server error");
			}
		}

		// POST: api/orgs/{orgId}/domains
		[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Owner)}")]

		[HttpPost("{orgId}/domains")]
		public async Task<IActionResult> AddDomainToOrganization(
			Guid orgId,
			[FromBody] AddDomainDto domainDto)
		{
			
			try
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId)) return Unauthorized();

				var result = await _organizationService.AddDomainToOrganizationAsync(
					orgId,
					domainDto.Domain,
					Guid.Parse(userId));


				return result
					? CreatedAtAction(
						nameof(GetOrganizationDomains),
						new { orgId },
						new { domain = domainDto.Domain })
					: BadRequest("Failed to add domain");
			}
			catch (UnauthorizedAccessException) { return Forbid(); }
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding domain to organization {OrgId}, orgId");
				return StatusCode(500, "Internal server error");
			}
		}

		// MEMBER MANAGEMENT ENDPOINTS
		[HttpGet("{orgId}/members")]
		public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetMembers(Guid orgId)
		{
			try
			{
				var members = await _organizationService.GetOrganizationMembersAsync(orgId);
				return Ok(members);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting members for organization {OrgId}", orgId);
				return StatusCode(500, "Internal server error");
			}
		}


	}
}
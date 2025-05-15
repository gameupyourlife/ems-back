using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo.Interfaces.Service;
using Microsoft.Extensions.Logging;
using ems_back.Services;

namespace ems_back.Controllers
{
	[Route("api/orgs")]
	[ApiController]
	public class OrganizationsController : ControllerBase
	{
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IOrganizationService _organizationService;
		private readonly ILogger<OrganizationService> _logger;

		public OrganizationsController(IOrganizationRepository organizationRepository, IOrganizationService organizationService, ILogger<OrganizationService> logger)
		{
			_organizationRepository = organizationRepository;
			_organizationService = organizationService;
			_logger = logger;
		}

		// GET: api/orgs
		[HttpGet]
		public async Task<ActionResult<IEnumerable<OrganizationResponseDto>>> GetOrganizations()
		{
			try
			{
				var organizations = await _organizationRepository.GetAllOrganizationsAsync();
				return Ok(organizations);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
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

				// Check if domain is already registered
				if (await _organizationRepository.DomainExistsAsync(organizationDto.Domain))
				{
					return Conflict(new { message = "This domain is already registered to another organization" });
				}

				var createdOrg = await _organizationRepository.CreateOrganizationAsync(organizationDto);

				// Fix: Use the correct action name that matches your route
				return CreatedAtAction(
					actionName: nameof(GetOrganization), // Make sure this matches the actual action method name
					routeValues: new { orgId = createdOrg.Id }, // Must match the route parameter name
					value: createdOrg);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET: api/orgs/{orgId}
		[HttpGet("{orgId}")]
		public async Task<ActionResult<OrganizationResponseDto>> GetOrganization(Guid orgId)
		{
			try
			{
				var organization = await _organizationRepository.GetOrganizationByIdAsync(orgId);
				return organization == null ? NotFound() : Ok(organization);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT: api/orgs/{orgId} calling Service not repo. SRprinzip begin
		
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

				// Get current user ID from auth context
				var updatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

				if (string.IsNullOrEmpty(updatedByUserId))
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
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Database error while updating organization");
				return StatusCode(500, "Failed to update organization due to database error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error updating organization");
				return StatusCode(500, "An unexpected error occurred");
			}
		}

		// DELETE: api/orgs/{orgId}
			[HttpDelete("{orgId}")]
		public async Task<IActionResult> DeleteOrganization(Guid orgId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}// Get current user ID from auth context
				var updatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(updatedByUserId))
				{
					return Unauthorized("User not authenticated");
				}

				var success = await _organizationRepository.DeleteOrganizationAsync(orgId, Guid.Parse(updatedByUserId));
				if (!success)
					return NotFound(new { message = "Organization not found" });

				return Ok(new { message = "Organization deleted successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
			}
		}

		// DOMAIN MANAGEMENT ENDPOINTS

		// GET: api/orgs/{orgId}/domains
		[HttpGet("{orgId}/domains")]
		public async Task<ActionResult<IEnumerable<string>>> GetOrganizationDomains(Guid orgId)
		{
			try
			{
				var domains = await _organizationRepository.GetOrganizationDomainsAsync(orgId);
				return Ok(domains);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// POST: api/orgs/{orgId}/domains
		[HttpPost("{orgId}/domains")]
		public async Task<IActionResult> AddDomainToOrganization(
			Guid orgId,
			[FromBody] AddDomainDto domainDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Check if domain is available
				if (!await _organizationRepository.IsDomainAvailableAsync(domainDto.Domain))
				{
					return Conflict(new { message = "Domain is already registered" });
				}

				var success = await _organizationRepository.AddDomainToOrganizationAsync(orgId, domainDto.Domain);
				if (!success)
				{
					return NotFound(new { message = "Organization not found" });
				}

				return CreatedAtAction(
					nameof(GetOrganizationDomains),
					new { orgId },
					new { domain = domainDto.Domain });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// MEMBER MANAGEMENT ENDPOINTS (existing implementation)
		[HttpGet("{orgId}/members")]
		public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetMembers(Guid orgId)
		{
			try
			{
				var members = await _organizationRepository.GetOrganizationsByUserAsync(orgId);
				return Ok(members);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// ... rest of your existing member endpoints ...
	}

	public class AddDomainDto
	{
		[Required]
		[StringLength(255)]
		public string Domain { get; set; }
	}
}
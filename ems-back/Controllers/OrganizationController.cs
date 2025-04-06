using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class OrganizationsController : ControllerBase
	{
		private readonly IOrganizationRepository _organizationRepository;

		public OrganizationsController(IOrganizationRepository organizationRepository)
		{
			_organizationRepository = organizationRepository;
		}

		// GET: api/organizations
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

		// GET: api/organizations/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<OrganizationResponseDto>> GetOrganization(Guid id)
		{
			try
			{
				var organization = await _organizationRepository.GetOrganizationByIdAsync(id);
				return organization == null ? NotFound() : Ok(organization);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET: api/organizations/user/{userId}
		[HttpGet("user/{userId}")]
		public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetOrganizationsByUser(Guid userId)
		{
			try
			{
				var organizations = await _organizationRepository.GetOrganizationsByUserAsync(userId);
				return Ok(organizations);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET: api/organizations/{id}/member-count
		[HttpGet("{id}/member-count")]
		public async Task<ActionResult<int>> GetMemberCount(Guid id)
		{
			try
			{
				var count = await _organizationRepository.GetMemberCountAsync(id);
				return Ok(count);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// POST: api/organizations
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

				var createdOrg = await _organizationRepository.CreateOrganizationAsync(organizationDto);
				return CreatedAtAction(nameof(GetOrganization), new { id = createdOrg.Id }, createdOrg);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT: api/organizations/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateOrganization(
			Guid id,
			[FromBody] OrganizationUpdateDto organizationDto)
		{
			try
			{
				var result = await _organizationRepository.UpdateOrganizationAsync(id, organizationDto);
				return result == null ? NotFound() : NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// DELETE: api/organizations/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteOrganization(Guid id)
		{
			try
			{
				var success = await _organizationRepository.DeleteOrganizationAsync(id);
				if (!success)
					return NotFound(new { message = "Organization not found" });

				return Ok(new { message = "Organization deleted successfully" }); // Change from NoContent() to Ok()
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
			}
		}

	}
}
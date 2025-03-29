using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
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
		public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizations()
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
		public async Task<ActionResult<Organization>> GetOrganization(Guid id)
		{
			try
			{
				var organization = await _organizationRepository.GetOrganizationByIdAsync(id);

				if (organization == null)
				{
					return NotFound();
				}

				return Ok(organization);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET: api/organizations/user/{userId}
		[HttpGet("user/{userId}")]
		public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizationsByUser(Guid userId)
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
		public async Task<ActionResult<Organization>> CreateOrganization([FromBody] Organization organization)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// In production, get these from authenticated user
				organization.CreatedBy = Guid.Parse("a1b2c3d4-1234-5678-9012-abcdef123456"); // Temp user ID
				organization.UpdatedBy = organization.CreatedBy;
				organization.CreatedAt = DateTime.UtcNow;
				organization.UpdatedAt = DateTime.UtcNow;

				await _organizationRepository.AddOrganizationAsync(organization);

				return CreatedAtAction(nameof(GetOrganization), new { id = organization.Id }, organization);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT: api/organizations/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateOrganization(Guid id, [FromBody] Organization organization)
		{
			try
			{
				if (id != organization.Id)
				{
					return BadRequest("ID mismatch");
				}

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var existingOrg = await _organizationRepository.GetOrganizationByIdAsync(id);
				if (existingOrg == null)
				{
					return NotFound();
				}

				// In production, get this from authenticated user
				organization.UpdatedBy = Guid.Parse("a1b2c3d4-1234-5678-9012-abcdef123456"); // Temp user ID
				organization.UpdatedAt = DateTime.UtcNow;

				await _organizationRepository.UpdateOrganizationAsync(organization);

				return NoContent();
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
				var organization = await _organizationRepository.GetOrganizationByIdAsync(id);
				if (organization == null)
				{
					return NotFound();
				}

				await _organizationRepository.DeleteOrganizationAsync(id);

				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
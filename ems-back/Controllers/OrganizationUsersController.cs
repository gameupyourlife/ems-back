using AutoMapper;
using ems_back.Repo.DTOs;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ems_back.Controllers
{

	namespace ems_back.Controllers
	{
		[Route("api/[controller]")]
		[ApiController]
		[Authorize]
		public class OrganizationUsersController : ControllerBase
		{
			private readonly IOrganizationUserRepository _repository;
			private readonly IMapper _mapper;

			public OrganizationUsersController(
				IOrganizationUserRepository repository,
				IMapper mapper)
			{
				_repository = repository;
				_mapper = mapper;
			}

			// GET: api/organizationusers/organization/{orgId}
			[HttpGet("organization/{orgId}")]
			public async Task<ActionResult<IEnumerable<OrganizationUserDto>>> GetByOrganization(Guid orgId)
			{
				var orgUsers = await _repository.GetByOrganizationIdAsync(orgId);
				return Ok(_mapper.Map<IEnumerable<OrganizationUserDto>>(orgUsers));
			}

			// GET: api/organizationusers/user/{userId}
			[HttpGet("user/{userId}")]
			public async Task<ActionResult<IEnumerable<OrganizationUserDto>>> GetByUser(Guid userId)
			{
				var userOrgs = await _repository.GetByUserIdAsync(userId);
				return Ok(_mapper.Map<IEnumerable<OrganizationUserDto>>(userOrgs));
			}

			// GET: api/organizationusers/{id}
			[HttpGet("{id}")]
			public async Task<ActionResult<OrganizationUserDto>> GetById(Guid id)
			{
				var orgUser = await _repository.GetByIdAsync(id);
				if (orgUser == null) return NotFound();
				return Ok(_mapper.Map<OrganizationUserDto>(orgUser));
			}

			// POST: api/organizationusers
			[HttpPost]
			public async Task<ActionResult<OrganizationUserDto>> Create([FromBody] CreateOrganizationUserDto dto)
			{
				if (await _repository.ExistsAsync(dto.OrganizationId, dto.UserId))
					return Conflict("User already exists in this organization");

				var orgUser = _mapper.Map<OrganizationUser>(dto);
				orgUser.JoinedAt = DateTime.UtcNow;
				orgUser.IsOrganizationAdmin = dto.UserRole == UserRole.Admin;

				await _repository.AddAsync(orgUser);

				return CreatedAtAction(nameof(GetById),
					new { id = orgUser.Id },
					_mapper.Map<OrganizationUserDto>(orgUser));
			}

			// PUT: api/organizationusers/{id}/role
			[HttpPut("{id}/role")]
			public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateOrganizationUserDto dto)
			{
				var orgUser = await _repository.GetByIdAsync(id);
				if (orgUser == null) return NotFound();

				orgUser.UserRole = dto.UserRole;
				orgUser.IsOrganizationAdmin = dto.UserRole == UserRole.Admin;

				await _repository.UpdateAsync(orgUser);
				return NoContent();
			}

			// DELETE: api/organizationusers/{id}
			[HttpDelete("{id}")]
			public async Task<IActionResult> Delete(Guid id)
			{
				var orgUser = await _repository.GetByIdAsync(id);
				if (orgUser == null) return NotFound();

				await _repository.RemoveAsync(id);
				return NoContent();
			}

			// GET: api/organizationusers/organization/{orgId}/count
			[HttpGet("organization/{orgId}/count")]
			public async Task<ActionResult<int>> GetMemberCount(Guid orgId)
			{
				var count = await _repository.GetMemberCountAsync(orgId);
				return Ok(count);
			}
		}
	}
}

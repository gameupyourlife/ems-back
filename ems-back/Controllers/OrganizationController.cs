using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ems_back.Controllers
{
    [Route("api/orgs")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationRepository _organizationRepository;

        public OrganizationsController(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
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

                var createdOrg = await _organizationRepository.CreateOrganizationAsync(organizationDto);
                return CreatedAtAction(nameof(GetOrganization), new { id = createdOrg.Id }, createdOrg);
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

        // PUT: api/orgs/{orgId}
        [HttpPut("{orgId}")]
        public async Task<IActionResult> UpdateOrganization(
            Guid orgId,
            [FromBody] OrganizationUpdateDto organizationDto)
        {
            try
            {
                var result = await _organizationRepository.UpdateOrganizationAsync(orgId, organizationDto);
                return result == null ? NotFound() : NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/orgs/{orgId}
        [HttpDelete("{orgId}")]
        public async Task<IActionResult> DeleteOrganization(Guid orgId)
        {
            try
            {
                var success = await _organizationRepository.DeleteOrganizationAsync(orgId);
                if (!success)
                    return NotFound(new { message = "Organization not found" });

                return Ok(new { message = "Organization deleted successfully" }); // Change from NoContent() to Ok()
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // GET: api/orgs/{orgId}/members
        [HttpGet("{orgId}/members")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetMembers(Guid orgId)
        {
            // Vom Copiloten erstellt und nicht geprüft
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

        // POST: api/orgs/{orgId}/members
        [HttpPost("{orgId}/members")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> AddMembers(
            Guid orgId,
            [FromBody] IEnumerable<UserCreateDto> userDtos)
        {
            // Vom Copiloten erstellt und nicht geprüft
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var members = await _organizationRepository.GetOrganizationsByUserAsync(orgId);
                return CreatedAtAction(nameof(GetMembers), new { orgId }, members);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/orgs/{orgId}/members/{userId}
        [HttpPut("{orgId}/members/{userId}")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> UpdateMembers(
            Guid orgId,
            Guid userId,
            [FromBody] UserUpdateDto userDto)
        {
            // Vom Copiloten erstellt und nicht geprüft
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

        // DELETE: api/orgs/{orgId}/members/{userId}
        [HttpDelete("{orgId}/members/{userId}")]
        public async Task<IActionResult> DeleteMember(Guid orgId, Guid userId)
        {
            // Vom Copiloten erstellt und nicht geprüft
            try
            {
                var success = await _organizationRepository.DeleteOrganizationAsync(orgId);
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
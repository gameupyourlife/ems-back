using ems_back.Repo.DTOs.File;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
	private readonly IFileRepository _fileRepository;
	private readonly ILogger<FilesController> _logger;

	public FilesController(
		IFileRepository fileRepository,
		ILogger<FilesController> logger)
	{
		_fileRepository = fileRepository;
		_logger = logger;
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<FileDetailedDto>> GetFile(Guid id)
	{
		try
		{
			var file = await _fileRepository.GetByIdAsync(id);
			if (file == null) return NotFound();
			return Ok(file);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting file with id {FileId}", id);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpGet("user/{userId}")]
	public async Task<ActionResult<IEnumerable<FileDto>>> GetFilesByUser(Guid userId)
	{
		try
		{
			var files = await _fileRepository.GetByUserAsync(userId);
			return Ok(files);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting files for user {UserId}", userId);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpGet("type/{type}")]
	public async Task<ActionResult<IEnumerable<FileDto>>> GetFilesByType(FileType type)
	{
		try
		{
			var files = await _fileRepository.GetByTypeAsync(type);
			return Ok(files);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting files of type {FileType}", type);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpPost]
	public async Task<ActionResult<FileDetailedDto>> UploadFile([FromBody] FileCreateDto fileDto)
	{
		try
		{
			var createdFile = await _fileRepository.AddAsync(fileDto);
			return CreatedAtAction(
				nameof(GetFile),
				new { id = createdFile.Id },
				createdFile);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error uploading file");
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateFile(Guid id, [FromBody] FileUpdateDto fileDto)
	{
		try
		{
			if (id != fileDto.Id) return BadRequest("ID mismatch");

			var updatedFile = await _fileRepository.UpdateAsync(fileDto);
			if (updatedFile == null) return NotFound();

			return NoContent();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating file with id {FileId}", id);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteFile(Guid id)
	{
		try
		{
			var result = await _fileRepository.DeleteAsync(id);
			if (!result) return NotFound();
			return NoContent();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting file with id {FileId}", id);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpGet("user/{userId}/count")]
	public async Task<ActionResult<int>> GetFileCountByUser(Guid userId)
	{
		try
		{
			var count = await _fileRepository.GetCountByUserAsync(userId);
			return Ok(count);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting file count for user {UserId}", userId);
			return StatusCode(500, "Internal server error");
		}
	}
}
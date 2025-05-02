using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.File;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
	private readonly IFileService _fileService;
	private readonly ILogger<FilesController> _logger;

	public FilesController(
		IFileService fileService,
		ILogger<FilesController> logger)
	{
		_fileService = fileService;
		_logger = logger;
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<FileDetailedDto>> GetFile(string id)
	{
		try
		{
			var file = await _fileService.GetFileByIdAsync(id);
			return file == null ? NotFound() : Ok(file);
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
			var files = await _fileService.GetFilesByUserAsync(userId);
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
			var files = await _fileService.GetFilesByTypeAsync(type);
			return Ok(files);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting files of type {FileType}", type);
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpPost]
	public async Task<ActionResult<FileDetailedDto>> UploadFile([FromForm] FileUploadDto fileForm)
	{
		try
		{
			var createdFile = await _fileService.UploadFileAsync(fileForm);
			return CreatedAtAction(
				nameof(GetFile),
				new { id = createdFile.Id },
				createdFile);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (MinioException)
		{
			return StatusCode(500, "Error uploading file to storage");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error uploading file");
			return StatusCode(500, "Internal server error");
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteFile(string id)
	{
		try
		{
			var success = await _fileService.DeleteFileAsync(id);
			return success ? NoContent() : NotFound();
		}
		catch (MinioException)
		{
			return StatusCode(500, "Error deleting file from storage");
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
			var count = await _fileService.GetFileCountByUserAsync(userId);
			return Ok(count);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting file count for user {UserId}", userId);
			return StatusCode(500, "Internal server error");
		}
	}
}

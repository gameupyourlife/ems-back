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
	public class FilesController : ControllerBase
	{
		private readonly IFileRepository _fileRepository;

		public FilesController(IFileRepository fileRepository)
		{
			_fileRepository = fileRepository;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<EventFile>> GetFile(Guid id)
		{
			var file = await _fileRepository.GetByIdAsync(id);
			if (file == null) return NotFound();
			return Ok(file);
		}

		[HttpGet("user/{userId}")]
		public async Task<ActionResult<IEnumerable<EventFile>>> GetFilesByUser(Guid userId)
		{
			return Ok(await _fileRepository.GetByUserAsync(userId));
		}

		[HttpGet("type/{type}")]
		public async Task<ActionResult<IEnumerable<EventFile>>> GetFilesByType(FileType type)
		{
			return Ok(await _fileRepository.GetByTypeAsync(type));
		}

		[HttpPost]
		public async Task<ActionResult<EventFile>> UploadFile([FromBody] EventFile file)
		{
			file.UploadedAt = DateTime.UtcNow;
			var createdFile = await _fileRepository.AddAsync(file);
			return CreatedAtAction(nameof(GetFile), new { id = createdFile.Id }, createdFile);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFile(Guid id)
		{
			await _fileRepository.DeleteAsync(id);
			return NoContent();
		}

		[HttpGet("user/{userId}/count")]
		public async Task<ActionResult<int>> GetFileCountByUser(Guid userId)
		{
			return Ok(await _fileRepository.GetCountByUserAsync(userId));
		}
	}
}
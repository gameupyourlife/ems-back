using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;

public class FileRepository : IFileRepository
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

	public FileRepository(ApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<FileDetailedDto> GetByIdAsync(Guid id)
	{
		var file = await _context.Files
			.Include(f => f.Uploader)
			.AsNoTracking()
			.FirstOrDefaultAsync(f => f.Id == id);

		return _mapper.Map<FileDetailedDto>(file);
	}

	public async Task<IEnumerable<FileDto>> GetByUserAsync(Guid userId)
	{
		var files = await _context.Files
			.Where(f => f.UploadedBy == userId)
			.AsNoTracking()
			.ToListAsync();

		return _mapper.Map<IEnumerable<FileDto>>(files);
	}

	public async Task<IEnumerable<FileDto>> GetByTypeAsync(FileType type)
	{
		var files = await _context.Files
			.Where(f => f.Type == type)
			.AsNoTracking()
			.ToListAsync();

		return _mapper.Map<IEnumerable<FileDto>>(files);
	}

	public async Task<FileDetailedDto> AddAsync(FileCreateDto fileDto)
	{
		var file = _mapper.Map<EventFile>(fileDto);
		file.UploadedAt = DateTime.UtcNow;

		await _context.Files.AddAsync(file);
		await _context.SaveChangesAsync();

		return await GetByIdAsync(file.Id);
	}

	public async Task<FileDetailedDto> UpdateAsync(FileUpdateDto fileDto)
	{
		var existingFile = await _context.Files.FindAsync(fileDto.Id);
		if (existingFile == null) return null;

		_mapper.Map(fileDto, existingFile);
		_context.Files.Update(existingFile);
		await _context.SaveChangesAsync();

		return await GetByIdAsync(fileDto.Id);
	}

	public async Task<bool> DeleteAsync(Guid id)
	{
		var file = await _context.Files.FindAsync(id);
		if (file == null) return false;

		_context.Files.Remove(file);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> ExistsAsync(Guid id)
	{
		return await _context.Files.AnyAsync(f => f.Id == id);
	}

	public async Task<int> GetCountByUserAsync(Guid userId)
	{
		return await _context.Files
			.CountAsync(f => f.UploadedBy == userId);
	}
}
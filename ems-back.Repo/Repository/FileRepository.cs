using ems_back.Repo.Data;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
	public class FileRepository : IFileRepository
	{
		private readonly ApplicationDbContext _context;

		public FileRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<EventFile> AddAsync(EventFile file)
		{
			await _context.Files.AddAsync(file);
			await _context.SaveChangesAsync();
			return file;
		}

		public async Task DeleteAsync(Guid id)
		{
			var file = await GetByIdAsync(id);
			if (file != null)
			{
				_context.Files.Remove(file);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsAsync(Guid id)
		{
			return await _context.Files.AnyAsync(f => f.Id == id);
		}

		public async Task<EventFile> GetByIdAsync(Guid id)
		{
			return await _context.Files
				.Include(f => f.Uploader)
				.FirstOrDefaultAsync(f => f.Id == id);
		}

		public async Task<IEnumerable<EventFile>> GetByTypeAsync(FileType type)
		{
			return await _context.Files
				.Where(f => f.Type == type)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<EventFile>> GetByUserAsync(Guid userId)
		{
			return await _context.Files
				.Where(f => f.UploadedBy == userId)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<int> GetCountByUserAsync(Guid userId)
		{
			return await _context.Files
				.CountAsync(f => f.UploadedBy == userId);
		}

		public async Task UpdateAsync(EventFile file)
		{
			_context.Files.Update(file);
			await _context.SaveChangesAsync();
		}
	}
}

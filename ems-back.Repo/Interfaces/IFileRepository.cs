// IFileRepository.cs
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
	public interface IFileRepository
	{
		Task<EventFile> GetByIdAsync(Guid id);
		Task<IEnumerable<EventFile>> GetByUserAsync(Guid userId);
		Task<IEnumerable<EventFile>> GetByTypeAsync(FileType type);
		Task<EventFile> AddAsync(EventFile file);
		Task DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
		Task<int> GetCountByUserAsync(Guid userId);
		Task UpdateAsync(EventFile file);
	}
}
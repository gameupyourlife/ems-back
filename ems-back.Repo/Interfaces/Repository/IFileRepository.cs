using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.File;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
    public interface IFileRepository
	{
		Task<FileDto> GetByIdAsync(string id);
		Task<IEnumerable<FileDto>> GetByUserAsync(Guid userId);
		Task<IEnumerable<FileDto>> GetByTypeAsync(FileType type);
		Task<FileDto> AddAsync(FileCreateDto fileDto);
		Task<FileDto> UpdateAsync(FileUpdateDto fileDto);
		Task<bool> DeleteAsync(string id);
		Task<bool> ExistsAsync(string id);
		Task<int> GetCountByUserAsync(Guid userId);
	}
}
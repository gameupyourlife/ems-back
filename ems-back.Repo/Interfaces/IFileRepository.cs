﻿// IFileRepository.cs
using ems_back.Repo.DTOs;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces
{
	public interface IFileRepository
	{
		Task<FileDetailedDto> GetByIdAsync(Guid id);
		Task<IEnumerable<FileDto>> GetByUserAsync(Guid userId);
		Task<IEnumerable<FileDto>> GetByTypeAsync(FileType type);
		Task<FileDetailedDto> AddAsync(FileCreateDto fileDto);
		Task<FileDetailedDto> UpdateAsync(FileUpdateDto fileDto);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
		Task<int> GetCountByUserAsync(Guid userId);
	}
}
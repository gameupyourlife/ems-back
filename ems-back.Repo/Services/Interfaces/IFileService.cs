using ems_back.Repo.DTOs.File;
using ems_back.Repo.DTOs;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services.Interfaces
{
	public interface IFileService
	{
		Task<FileDetailedDto> GetFileByIdAsync(string id);
		Task<IEnumerable<FileDto>> GetFilesByUserAsync(Guid userId);
		Task<IEnumerable<FileDto>> GetFilesByTypeAsync(FileType type);
		Task<FileDetailedDto> UploadFileAsync(FileUploadDto fileForm);
		Task<bool> DeleteFileAsync(string id);
		Task<int> GetFileCountByUserAsync(Guid userId);
	}
}

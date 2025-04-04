using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.File
{
    // Basic response DTO
    public class FileDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public FileType Type { get; set; }
        public DateTime UploadedAt { get; set; }
        public string OriginalName { get; set; }
        public string ContentType { get; set; }
        public long SizeInBytes { get; set; }
    }
}

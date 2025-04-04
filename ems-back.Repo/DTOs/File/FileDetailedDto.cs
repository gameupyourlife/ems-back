using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.User;

namespace ems_back.Repo.DTOs.File
{
    public class FileDetailedDto : FileDto
    {
        public UserDto Uploader { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	public class FileDetailedDto : FileDto
	{
		public UserDto Uploader { get; set; }
	}
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Domain
{
	public class AddDomainDto
	{
		[Required]
		[StringLength(255)]
		public string Domain { get; set; }
	}
}

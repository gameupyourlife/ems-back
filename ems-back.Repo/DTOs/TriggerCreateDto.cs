using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	public class TriggerCreateDto
	{
		[Required]
		public TriggerType Type { get; set; }

		public string Details { get; set; }

		[Required]
		public Guid FlowId { get; set; }
	}
}

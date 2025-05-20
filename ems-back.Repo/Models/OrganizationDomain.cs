using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Models
{
	public class OrganizationDomain
	{
		public Guid Id { get; set; }

		[Required]
		[MaxLength(255)] 
		public string Domain { get; set; }  
		public Guid OrganizationId { get; set; }

		// Navigation property
		public Organization Organization { get; set; }
	}
}

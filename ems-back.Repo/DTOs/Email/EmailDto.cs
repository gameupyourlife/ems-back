using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Email
{
    public class EmailDto
    {
        public Guid MailId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }


		//[Column(TypeName = "jsonb")]
		public IEnumerable<Guid> Recipients { get; set; }
        public DateTime? ScheduledFor { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        [Required]
		public Guid EventId { get; set; }

	}
}

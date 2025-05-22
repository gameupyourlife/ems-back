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
    public class MailDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public string? Description { get; set; }
        public IEnumerable<Guid>? Recipients { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public required bool IsUserCreated { get; set; }
        public required bool SendToAllParticipants { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
	}
}

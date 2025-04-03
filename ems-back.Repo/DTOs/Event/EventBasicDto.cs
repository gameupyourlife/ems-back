using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    // Basic response DTO
    public class EventBasicDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Image { get; set; }
        public EventCategory? Category { get; set; }
        public EventStatus? Status { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int AttendeeCount { get; set; }
        public int AgendaItemCount { get; set; }
    }
}

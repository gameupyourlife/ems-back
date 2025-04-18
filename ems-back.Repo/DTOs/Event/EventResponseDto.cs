﻿using ems_back.Repo.DTOs.User;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    // For API responses
    public class EventResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Image { get; set; }
        public EventCategory? Category { get; set; }
        public EventStatus? Status { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        // Flattened user info
        public UserDto Creator { get; set; }
        public UserDto Updater { get; set; }

        // Counts instead of full collections
        public int AttendeeCount { get; set; }
        public int AgendaItemCount { get; set; }
    }
}

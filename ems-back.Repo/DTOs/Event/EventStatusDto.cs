﻿using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Event
{
    // For updating event status
    public class EventStatusDto
    {
        [Required]
        public EventStatus Status { get; set; }

        [Required]
        public Guid UpdatedBy { get; set; }
    }
}

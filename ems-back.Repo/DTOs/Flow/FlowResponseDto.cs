﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.User;

namespace ems_back.Repo.DTOs.Flow
{
    // Detailed response (with creator/updater info)
    public class FlowResponseDto : FlowBasicDto
    {
        public UserDto Creator { get; set; }
        public UserDto Updater { get; set; }

        public bool stillPending { get; set; } = false;
        public bool multipleRuns { get; set; } = false;
        public Guid OrganizationId { get; set; }
        public Guid EventId { get; set; } // <-- Important!
	}
}

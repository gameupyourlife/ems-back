﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.DTOs.Flow;

namespace ems_back.Repo.DTOs.Action
{
    // Detailed response DTO (with Flow information)
    public class ActionDetailedDto : ActionDto
    {
        public FlowBasicDto Flow { get; set; }
    }
}

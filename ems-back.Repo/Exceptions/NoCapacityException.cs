﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Exceptions
{
    public class NoCapacityException : Exception
    {
        public NoCapacityException(string message) : base(message) { }
    }
}

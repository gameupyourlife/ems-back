using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs.Domain
{
	public class DomainConflictException : Exception
	{
		public DomainConflictException(string message) : base(message) { }
	}
}

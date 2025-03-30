using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.DTOs
{
	// Detailed response (with creator/updater info)
	public class FlowResponseDto : FlowBasicDto
	{
		public UserDto Creator { get; set; }
		public UserDto Updater { get; set; }
	}
}

using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Service
{
	public interface IOrganizationDomainRepository
	{
		Task<OrganizationDomain?> GetByDomainAsync(string domain);
	}
}

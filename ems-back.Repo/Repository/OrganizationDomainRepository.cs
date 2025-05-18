using ems_back.Repo.Data;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
	public class OrganizationDomainRepository : IOrganizationDomainRepository
	{
		private readonly ApplicationDbContext _context;

		public OrganizationDomainRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<OrganizationDomain> GetByDomainAsync(string domain)
		{ 
			return await _context.OrganizationDomain
				.Include(od => od.Organization)
				.FirstOrDefaultAsync(od => od.Domain == domain.ToLower());
		}
	}
	}

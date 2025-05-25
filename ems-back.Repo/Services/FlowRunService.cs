using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Flow.FlowsRun;
using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.Interfaces.Service;
using Microsoft.Extensions.Logging;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using ems_back.Repo.Repository;

namespace ems_back.Repo.Services
{
    public class FlowRunService : IFlowRunService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FlowRunService> _logger;
        private readonly IFlowRunRepository _flowRunRepository;

        public FlowRunService(ApplicationDbContext context, ILogger<FlowRunService> logger, IFlowRunRepository repo)
        {
            _context = context;
            _logger = logger;
            _flowRunRepository = repo;
        }

        public async Task<List<FlowsRunResponseDto>> GetAllForOrganizationAsync(Guid orgId)
        {
            var flowRuns = await _flowRunRepository.GetAllForOrganization(orgId);

            return flowRuns.Select(fr => new FlowsRunResponseDto
            {
                FlowId = fr.FlowId,
                EventId = fr.Flow?.EventId ?? Guid.Empty, // Sicherstellen, dass Flow geladen ist
                Status = fr.Status,
                Timestamp = fr.Timestamp,
                Logs = fr.Logs
            }).ToList();
        }

        public async Task<List<FlowsRunResponseDto>> GetByEventAsync(Guid orgId, Guid eventId)
        {
            var flowRuns = await _flowRunRepository.GetByEvent(orgId, eventId);

            return flowRuns.Select(fr => new FlowsRunResponseDto
            {
                FlowId = fr.FlowId,
                EventId = fr.Flow?.EventId ?? Guid.Empty,
                Status = fr.Status,
                Timestamp = fr.Timestamp,
                Logs = fr.Logs
            }).ToList();
        }
    }

}

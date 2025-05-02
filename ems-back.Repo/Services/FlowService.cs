using ems_back.Repo.DTOs.Flow;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
	public class FlowService : IFlowService
	{
		private readonly IFlowRepository _flowRepository;
		private readonly ILogger<FlowService> _logger;

		public FlowService(IFlowRepository flowRepository, ILogger<FlowService> logger)
		{
			_flowRepository = flowRepository;
			_logger = logger;
		}

		public async Task<IEnumerable<FlowBasicDto>> GetAllActiveFlowsAsync()
		{
			return await _flowRepository.GetAllActiveAsync();
		}

		public async Task<FlowResponseDto> GetFlowByIdAsync(Guid id)
		{
			var flow = await _flowRepository.GetByIdAsync(id);
			if (flow == null)
			{
				_logger.LogWarning("Flow with id {FlowId} not found", id);
			}
			return flow;
		}

		public async Task<FlowDetailedDto> GetFlowDetailsAsync(Guid id)
		{
			var flow = await _flowRepository.GetWithDetailsAsync(id);
			if (flow == null)
			{
				_logger.LogWarning("Detailed flow with id {FlowId} not found", id);
			}
			return flow;
		}

		public async Task<FlowResponseDto> CreateFlowAsync(FlowCreateDto flowDto)
		{
			return await _flowRepository.AddAsync(flowDto);
		}

		public async Task<bool> UpdateFlowAsync(Guid id, FlowUpdateDto flowDto)
		{
			if (id != flowDto.Id)
			{
				return false;
			}
			return await _flowRepository.UpdateAsync(id, flowDto) != null;
		}

		public async Task<FlowResponseDto> ToggleFlowStatusAsync(Guid id, FlowStatusDto statusDto)
		{
			return await _flowRepository.ToggleStatusAsync(id, statusDto);
		}

		public async Task<bool> DeleteFlowAsync(Guid id)
		{
			return await _flowRepository.DeleteAsync(id);
		}
	}
}

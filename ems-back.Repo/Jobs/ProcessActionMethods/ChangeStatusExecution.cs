﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo.Data;

namespace ems_back.Repo.Jobs.ProcessActionMethods
{
    public class ChangeStatusExecution
    {
        private readonly ApplicationDbContext _dbContext;

        // Constructor to initialize _dbContext
        public ChangeStatusExecution(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task HandleChangeStatusActionAsync(StatusChangeModel action)
        {
            try
            {
                var flow = await _dbContext.Flows
                    .Include(f => f.Event)
                    .FirstOrDefaultAsync(f => f.FlowId == action.FlowId);

                if (flow == null)
                {
                    throw new InvalidOperationException($"Flow mit ID {action.FlowId} wurde nicht gefunden.");
                }

                if (flow.Event == null)
                {
                    throw new InvalidOperationException($"Flow mit ID {action.FlowId} hat kein zugeordnetes Event.");
                }

                flow.Event.Status = action.NewStatus;

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var run = new FlowsRun
                {
                    Id = Guid.NewGuid(),
                    FlowId = action.FlowId,
                    Status = FlowRunStatus.Failed,
                    Timestamp = DateTime.UtcNow,
                    Logs = ex.ToString()
                };

                _dbContext.FlowsRun.Add(run);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}

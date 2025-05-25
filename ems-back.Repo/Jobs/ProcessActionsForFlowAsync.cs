using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Jobs.ProcessActionMethods;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Jobs
{
    public class ProcessActionsForFlow
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ChangeDescriptionExecution _changeDescriptionExecution;
        private readonly ChangeImageExecution _changeImageExecution;
        private readonly ChangeStatusExecution _changeStatusExecution;
        private readonly ChangeTitleExecution _changeTitleExecution;
        private readonly SendEmailExecution _sendEmailExecution;

        // Constructor to initialize _dbContext
        public ProcessActionsForFlow(ApplicationDbContext dbContext, ChangeImageExecution changeImageExecution, 
            ChangeStatusExecution changeStatusExecution, ChangeTitleExecution changeTitleExecution, 
            SendEmailExecution sendEmailExecution, ChangeDescriptionExecution changeDescriptionExecution)
        {
            _dbContext = dbContext;
            _changeImageExecution = changeImageExecution;
            _changeStatusExecution = changeStatusExecution;
            _changeTitleExecution = changeTitleExecution;
            _sendEmailExecution = sendEmailExecution;
            _changeDescriptionExecution = changeDescriptionExecution;
        }

        public async Task ProcessActionsForFlowAsync(Guid flowId, List<BaseAction> allActions)
        {
            // Neuen FlowsRun anlegen und direkt auf Running setzen
            var flowRun = new FlowsRun
            {
                Id = Guid.NewGuid(),
                FlowId = flowId,
                Status = FlowRunStatus.Failed,
                Timestamp = DateTime.UtcNow,
                Logs = $"Flow [{flowId}] um {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} gestartet."
            };

            _dbContext.FlowsRun.Add(flowRun);
            await _dbContext.SaveChangesAsync();

            var actionsForFlow = allActions.Where(a => a.FlowId == flowId).ToList();

            try
            {
                foreach (var action in actionsForFlow)
                {
                    switch (action.ActionType)
                    {
                        case ActionType.SendEmail:
                            await _sendEmailExecution.HandleSendEmailActionAsync((EmailActionModel)action);
                            break;

                        case ActionType.ChangeStatus:
                            await _changeStatusExecution.HandleChangeStatusActionAsync((StatusChangeModel)action);
                            break;

                        case ActionType.ChangeImage:
                            await _changeImageExecution.HandleChangeImageActionAsync((ImageChangeModel)action);
                            break;

                        case ActionType.ChangeTitle:
                            await _changeTitleExecution.HandleChangeTitleActionAsync((TitleChangeModel)action);
                            break;

                        case ActionType.ChangeDescription:
                            await _changeDescriptionExecution.HandleChangeDescriptionActionAsync((DescriptionChangeModel)action);
                            break;

                        default:
                            Console.WriteLine($"Unbekannter ActionType: {action.ActionType}");
                            break;
                    }
                }

                // Alle Actions erfolgreich → Status auf Completed setzen
                var run = new FlowsRun
                {
                    Id = Guid.NewGuid(),
                    FlowId = flowId,
                    Status = FlowRunStatus.Failed,
                    Timestamp = DateTime.UtcNow,
                    Logs = $"Flow [{flowId}] wurde um {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} erfolgreich ausgeführt."
                };

                _dbContext.FlowsRun.Add(run);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var run = new FlowsRun
                {
                    Id = Guid.NewGuid(),
                    FlowId = flowId,
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

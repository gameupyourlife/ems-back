using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Jobs
{
    public class ProcessActionsForFlow
    {
        private readonly ApplicationDbContext _dbContext;

        // Constructor to initialize _dbContext
        public ProcessActionsForFlow(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
                            await HandleSendEmailActionAsync((EmailActionModel)action);
                            break;

                        case ActionType.ChangeStatus:
                            await HandleChangeStatusActionAsync((StatusChangeModel)action);
                            break;

                        case ActionType.ChangeImage:
                            await HandleChangeImageActionAsync((ImageChangeModel)action);
                            break;

                        case ActionType.ChangeTitle:
                            await HandleChangeTitleActionAsync((TitleChangeModel)action);
                            break;

                        case ActionType.ChangeDescription:
                            await HandleChangeDescriptionActionAsync((DescriptionChangeModel)action);
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

        private Task HandleSendEmailActionAsync(EmailActionModel action)
        {
            Console.WriteLine($"[Action] Sende Email mit MailId: {action.MailId} für Flow {action.FlowId}");
            // TODO: EmailService.SendMail(action.MailId);
            return Task.CompletedTask;
        }

        private async Task HandleChangeStatusActionAsync(StatusChangeModel action)
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


        private async Task HandleChangeDescriptionActionAsync(DescriptionChangeModel action)
        {
            try
            {
                var flow = await _dbContext.Flows
                    .Include(f => f.Event)
                    .FirstOrDefaultAsync(f => f.FlowId == action.FlowId);

                if (flow == null)
                    throw new InvalidOperationException($"Flow mit ID {action.FlowId} wurde nicht gefunden.");

                if (flow.Event == null)
                    throw new InvalidOperationException($"Flow mit ID {action.FlowId} hat kein zugeordnetes Event.");

                flow.Event.Description = action.NewDescription;

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

        private async Task HandleChangeImageActionAsync(ImageChangeModel action)
        {
            try
            {
                var flow = await _dbContext.Flows
                    .Include(f => f.Event)
                    .FirstOrDefaultAsync(f => f.FlowId == action.FlowId);

                if (flow == null)
                    throw new InvalidOperationException($"Flow mit ID {action.FlowId} wurde nicht gefunden.");

                if (flow.Event == null)
                    throw new InvalidOperationException($"Flow mit ID {action.FlowId} hat kein zugeordnetes Event.");

                // Hier das Bild im Event aktualisieren (Property ggf. anpassen)
                flow.Event.Image = action.NewImage;

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

        private async Task HandleChangeTitleActionAsync(TitleChangeModel action)
        {
            try
            {
                var flow = await _dbContext.Flows
                    .Include(f => f.Event)
                    .FirstOrDefaultAsync(f => f.FlowId == action.FlowId);

                if (flow == null)
                    throw new InvalidOperationException($"Flow mit ID {action.FlowId} wurde nicht gefunden.");

                if (flow.Event == null)
                    throw new InvalidOperationException($"Flow mit ID {action.FlowId} hat kein zugeordnetes Event.");

                flow.Event.Title = action.NewTitle;

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

                throw new ApplicationException($"Fehler beim Ändern des Titels für FlowId {action.FlowId}: {ex.Message}", ex);
            }
        }

    }
}

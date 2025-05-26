using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Models;
using ems_back.Repo.Services;

namespace ems_back.Repo.Jobs.ProcessActionMethods
{
    public class SendEmailExecution
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly MailService _mailService;

        // Constructor to initialize _dbContext
        public SendEmailExecution(ApplicationDbContext dbContext, MailService mailService)
        {
            _dbContext = dbContext;
            _mailService = mailService;
        }
        public async Task HandleSendEmailActionAsync(EmailActionModel action)
        {
            try
            {
                // Flow auslesen
                var flow = await _dbContext.Flows.FindAsync(action.FlowId);
                if (flow == null)
                    throw new Exception($"Flow mit ID {action.FlowId} nicht gefunden.");

                var flowId = flow.FlowId;

                // Event auslesen
                var evt = await _dbContext.Events.FindAsync(flow.EventId);
                if (evt == null)
                    throw new Exception($"Event mit ID {flow.EventId} nicht gefunden.");

                var eventId = evt.Id;

                // Organization auslesen
                var org = await _dbContext.Organizations.FindAsync(evt.OrganizationId);
                if (org == null)
                    throw new Exception($"Organisation mit ID {evt.OrganizationId} nicht gefunden.");

                var organizationId = org.Id;

                await _mailService.SendMailByFlowAsync(organizationId, eventId, action.MailId);
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

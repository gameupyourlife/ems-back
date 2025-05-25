using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Actions.ActionModels;

namespace ems_back.Repo.Jobs.ProcessActionMethods
{
    public class SendEmailExecution
    {
        private readonly ApplicationDbContext _dbContext;

        // Constructor to initialize _dbContext
        public SendEmailExecution(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task HandleSendEmailActionAsync(EmailActionModel action)
        {
            Console.WriteLine($"[Action] Sende Email mit MailId: {action.MailId} für Flow {action.FlowId}");
            // TODO: EmailService.SendMail(action.MailId);
            return Task.CompletedTask;
        }
    }
}

using ems_back.Repo.Data;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
    public class MailQueueService : IMailQueueService
    {
        private readonly ApplicationDbContext _context;

        public MailQueueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task EnqueueAsnyc(string toEmail, string toName, string subject, string body)
        {
            var mailQueueEntry = new MailQueueEntry
            {
                ToEmail = toEmail,
                ToName = toName,
                Subject = subject,
                Body = body,
                Status = MailRunStatus.Pending,
            };

            _context.MailQueueEntries.Add(mailQueueEntry);
            return _context.SaveChangesAsync();
        }
    }
}

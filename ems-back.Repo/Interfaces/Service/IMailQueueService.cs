using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Interfaces.Service
{
    public interface IMailQueueService
    {
        Task EnqueueAsnyc(string toEmail, string toName, string subject, string body);
    }
}

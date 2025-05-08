using ems_back.Repo.Models.Types;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Data;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;

namespace ems_back.Repo.Jobs.Actions
{
    public class StatusChangeAction : BaseAction
    {
        private readonly ApplicationDbContext _dbContext;

        public StatusChangeAction(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public bool ParseTriggers(string triggerJson, TriggerType triggerType, Guid eventId)
        {
            var trigger = ParseTrigger(triggerJson, triggerType);

            return trigger switch
            {
                DateTrigger dt => HandleDateTrigger(dt),
                RelativeDateTrigger rdt => HandleRelativeDateTrigger(rdt, eventId),
                NumOfAttendeesTrigger nat => HandleNumOfAttendeesTrigger(nat, eventId),
                StatusTrigger st => HandleStatusTrigger(st, eventId),
                RegistrationTrigger rt => HandleRegistrationTrigger(rt, eventId),
                _ => throw new NotSupportedException("Unbekannter Trigger-Typ.")
            };
        }

        private bool HandleDateTrigger(DateTrigger trigger)
        {
            throw new NotImplementedException();
        }

        private bool HandleRelativeDateTrigger(RelativeDateTrigger trigger, Guid eventId)
        {
            throw new NotImplementedException();
        }

        private bool HandleNumOfAttendeesTrigger(NumOfAttendeesTrigger trigger, Guid eventId)
        {
            throw new NotImplementedException();
        }

        private bool HandleStatusTrigger(StatusTrigger trigger, Guid eventId)
        {
            throw new NotImplementedException();
        }

        private bool HandleRegistrationTrigger(RegistrationTrigger trigger, Guid eventId)
        {
            throw new NotImplementedException();
        }
    }
}

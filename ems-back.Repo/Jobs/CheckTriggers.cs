using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Data;
using System.Text.Json.Serialization;
using ems_back.Repo.Jobs.CheckTriggerMethods; // oder Newtonsoft.Json

namespace ems_back.Repo.Jobs
{
    public class CheckTriggers
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DateTriggerEvaluator _dateTriggerEvaluator;
        private readonly NumOfAttendeesTriggerEvalator _numOfAttendeesTriggerEvaluator;
        private readonly RelativeDateTriggerEvaluator _relativeDateTriggerEvaluator;
        private readonly StatusTriggerEvaluator _statusTriggerEvaluator;

        public CheckTriggers(ApplicationDbContext dbContext, NumOfAttendeesTriggerEvalator numOfAttendeesTriggerEvaluator, 
            RelativeDateTriggerEvaluator relativeDateTriggerEvaluator, StatusTriggerEvaluator statusTriggerEvaluator,
            DateTriggerEvaluator dateTriggerEvaluator)
        {
            _dbContext = dbContext;
            _numOfAttendeesTriggerEvaluator = numOfAttendeesTriggerEvaluator;
            _relativeDateTriggerEvaluator = relativeDateTriggerEvaluator;
            _statusTriggerEvaluator = statusTriggerEvaluator;
            _dateTriggerEvaluator = dateTriggerEvaluator;
        }

        public async Task<List<(Guid FlowId, bool IsTriggered)>> CheckTriggersAsync(IEnumerable<BaseTrigger> triggers)
        {
            var results = new List<(Guid FlowId, bool IsTriggered)>();

            // Gruppiere nur Trigger mit gültiger FlowId
            var groupedByFlow = triggers
                .Where(t => t.FlowId.HasValue)
                .GroupBy(t => t.FlowId!.Value); // FlowId ist hier garantiert nicht null

            foreach (var group in groupedByFlow)
            {
                bool allMet = true;

                foreach (var trigger in group)
                {
                    bool isMet = trigger.TriggerType switch
                    {
                        TriggerType.Date => trigger is DateTrigger dt && _dateTriggerEvaluator.IsDateTriggerMet(dt),
                        TriggerType.RelativeDate => trigger is RelativeDateTrigger rdt && await _relativeDateTriggerEvaluator.IsRelativeDateTriggerMetAsync(rdt),
                        TriggerType.NumOfAttendees => trigger is NumOfAttendeesTrigger nat && await _numOfAttendeesTriggerEvaluator.IsNumOfAttendeesTriggerMetAsync(nat),
                        TriggerType.Status => trigger is StatusTrigger st && await _statusTriggerEvaluator.IsStatusTriggerMetAsync(st),
                        _ => false
                    };

                    if (!isMet)
                    {
                        allMet = false;
                        break;
                    }
                }

                results.Add((group.Key, allMet)); // group.Key ist vom Typ Guid
            }

            return results;
        }

    }
}
using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Jobs.Mapping.Actions;
using ems_back.Repo.Models.Types;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Jobs
{
    public class MapActions
    {
        private readonly ApplicationDbContext _dbContext;

        public MapActions(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BaseAction>> GetMappedActionsAsync()
        {
            var actions = await _dbContext.Set<Models.Action>()
                .Include(t => t.Flow)
                .Where(t => t.FlowId != null &&
                            t.Flow != null &&
                            !t.Flow.IsActive &&
                            t.Flow.stillPending)
                .OrderBy(t => t.FlowId)
                .ToListAsync();

            var result = new List<BaseAction>();

            foreach (var action in actions)
            {
                BaseAction mapped = action.Type switch
                {
                    ActionType.ChangeDescription => ChangeDescriptionMapping.MapDescriptionChange(action),
                    ActionType.SendEmail => EmailMapping.MapEmail(action),
                    ActionType.ChangeImage => ChangeImageMapping.MapImageChange(action),
                    ActionType.ChangeStatus => ChangeStatusMapping.MapStatusChange(action),
                    ActionType.ChangeTitle => ChangeTitleMapping.MapTitleChange(action),
                    _ => throw new InvalidOperationException($"Unhandled ActionType: {action.Type}")
                };

                result.Add(mapped);
            }

            return result;
        }
        
    }
}


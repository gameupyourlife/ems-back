using ems_back.Repo.Data;
using ems_back.Repo.Jobs.Actions.ActionModels;
using ems_back.Repo.Models.Types;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            Console.WriteLine($"[MapActions] Found {actions.Count} actions to map.");

            var result = new List<BaseAction>();

            foreach (var action in actions)
            {
                BaseAction mapped = action.Type switch
                {
                    ActionType.ChangeDescription => MapDescriptionChange(action),
                    ActionType.SendEmail => MapEmail(action),
                    ActionType.ChangeImage => MapImageChange(action),
                    ActionType.ChangeStatus => MapStatusChange(action),
                    ActionType.ChangeTitle => MapTitleChange(action),
                    _ => throw new InvalidOperationException($"Unhandled ActionType: {action.Type}")
                };

                result.Add(mapped);
            }

            return result;
        }

        // Ensure that the JSON string is valid and matches the expected structure
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        private static DescriptionChangeModel MapDescriptionChange(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<DescriptionChangeModel>(action.Details)
                          ?? throw new InvalidOperationException("Invalid JSON for DescriptionChangeModel");

            return new DescriptionChangeModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.ChangeDescription,
                NewDescription = details.NewDescription
            };
        }

        private static EmailActionModel MapEmail(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<EmailActionModel>(action.Details)
                          ?? throw new InvalidOperationException("Invalid JSON for EmailActionModel");

            return new EmailActionModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.SendEmail,
                MailId = (Guid)details.MailId
            };
        }

        private static ImageChangeModel MapImageChange(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<ImageChangeModel>(action.Details)
                          ?? throw new InvalidOperationException("Invalid JSON for ImageChangeModel");

            return new ImageChangeModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.ChangeImage,
                NewImage = details.NewImage
            };
        }

        private static StatusChangeModel MapStatusChange(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<StatusChangeModel>(action.Details, _jsonOptions)
                          ?? throw new InvalidOperationException("Invalid JSON for StatusChangeModel");

            return new StatusChangeModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.ChangeStatus,
                NewStatus = details.NewStatus
            };
        }

        private static TitleChangeModel MapTitleChange(Models.Action action)
        {
            var details = JsonSerializer.Deserialize<TitleChangeModel>(action.Details)
                          ?? throw new InvalidOperationException("Invalid JSON for TitleChangeModel");

            return new TitleChangeModel
            {
                ActionId = action.Id,
                FlowId = action.FlowId ?? throw new InvalidOperationException("FlowId missing"),
                ActionType = ActionType.ChangeTitle,
                NewTitle = details.NewTitle
            };
        }
    }
}


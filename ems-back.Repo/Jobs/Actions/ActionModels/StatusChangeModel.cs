using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Actions.ActionModels
{
    public class StatusChangeModel : BaseAction
    {
        [JsonPropertyName("newStatus")]
        public EventStatus NewStatus { get; set; }
    }
}

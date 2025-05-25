using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Actions.ActionModels
{
    public class DescriptionChangeModel : BaseAction 
    {
        [JsonPropertyName("newDescription")]
        public required string NewDescription { get; set; }

    }
}

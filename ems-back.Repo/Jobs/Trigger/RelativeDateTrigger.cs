using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Trigger
{
    public enum RelativeDateOperator
    {
        Before,
        After,
        Equal
    }

    public enum RelativeDateValueType
    {
        Hours,
        Days,
        Weeks,
        Months
    }

    public enum RelativeDateRelativeTo
    {
        EventStart,
        EventEnd
    }

    public enum RelativeDateRelativeComparison
    {
        Before,
        After
    }

    public class RelativeDateTrigger : BaseTrigger
    {
        [JsonPropertyName("operator")]
        public RelativeDateOperator Operator { get; set; }
        [JsonPropertyName("value")]
        public int Value { get; set; }
        [JsonPropertyName("valueType")]
        public RelativeDateValueType ValueType { get; set; }
        [JsonPropertyName("valueRelativeTo")]
        public RelativeDateRelativeTo ValueRelativeTo { get; set; }
        [JsonPropertyName("valueRelativeOperator")]
        public RelativeDateRelativeComparison ValueRelativeOperator { get; set; }
    }

}

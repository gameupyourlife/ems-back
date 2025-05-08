using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public class RelativeDateTrigger : ITrigger
    {
        public TriggerType TriggerType => TriggerType.RelativeDate;
        public RelativeDateOperator Operator { get; init; }
        public int Value { get; init; }
        public RelativeDateValueType ValueType { get; init; }
        public RelativeDateRelativeTo ValueRelativeTo { get; init; }
        public RelativeDateRelativeComparison ValueRelativeOperator { get; init; }
    }

}

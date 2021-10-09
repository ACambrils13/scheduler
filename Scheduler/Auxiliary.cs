using Scheduler.Resources;
using System;

namespace Scheduler
{
    public class Auxiliary
    {
        public static void CheckNotNull(object[] Elements)
        {
            foreach (object Element in Elements)
            {
                if (Element == null)
                {
                    throw new Exception(TextResources.ExcNull);
                }
            }
        }
    }
    public struct LimitsConfig
    {
        public LimitsConfig(DateTime? Start, DateTime? End)
        {
            this.StartDate = Start;
            this.EndDate = End;
        }

        public DateTime? StartDate { get; }
        public DateTime? EndDate { get; }
    }

    public enum ScheduleType
    {
        Once,
        Recurring,
        Undefined
    }

    public enum OccurrencyPeriod
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Undefined = 99
    }
}

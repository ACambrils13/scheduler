using System;

namespace Scheduler
{
    public struct LimitsConfig
    {
        public LimitsConfig(DateTime? Start, DateTime? End)
        {
            ScheduleConfigValidator.ValidateLimits(Start, End);
            this.StartLimit = Start;
            this.EndLimit = End;
        }

        public DateTime? StartLimit { get; }
        public DateTime? EndLimit { get; }
    }

    public enum ScheduleTypeEnum
    {
        Once,
        Recurring
    }

    public enum OccurrencyPeriodEnum
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public enum DailyFrecuencyEnum
    {
        Hours,
        Minutes,
        Seconds
    }
}

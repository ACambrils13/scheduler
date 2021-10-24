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
            ScheduleConfigValidator.ValidateLimits(Start, End);
            this.StartLimit = Start;
            this.EndLimit = End;
        }

        public DateTime? StartLimit { get; }
        public DateTime? EndLimit { get; }
    }

    public struct Week
    {
        public Week(bool Mo, bool Tu, bool We, bool Th, bool Fr, bool Sa, bool Su)
        {
            this.Monday = Mo;
            this.Tuesday = Tu;
            this.Wednesday = We;
            this.Thursday = Th;
            this.Friday = Fr;
            this.Saturday = Sa;
            this.Sunday = Su;
        }

        public bool Monday { get; }
        public bool Tuesday { get; }
        public bool Wednesday { get; }
        public bool Thursday { get; }
        public bool Friday { get; }
        public bool Saturday { get; }
        public bool Sunday { get; }
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
        Yearly,
        Undefined = 99
    }

    public enum DailyFrecuencyEnum
    {
        Hours,
        Minutes,
        Seconds,
        Undefined = 99
    }
}

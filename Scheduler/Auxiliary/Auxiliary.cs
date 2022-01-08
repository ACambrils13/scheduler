using Scheduler.Validators;
using System;

namespace Scheduler.Auxiliary
{
    public struct DateLimitsConfig
    {
        public DateLimitsConfig(DateTime? start, DateTime? end)
        {
            ScheduleConfigValidator.ValidateDateLimits(start, end);
            this.StartLimit = start;
            this.EndLimit = end;
        }

        public DateTime? StartLimit { get; }
        public DateTime? EndLimit { get; }
    }

    public struct HourLimitsConfig
    {
        public HourLimitsConfig(TimeSpan? start, TimeSpan? end)
        {
            ScheduleConfigValidator.ValidateHourLimits(start, end);
            this.StartLimit = start;
            this.EndLimit = end;
        }

        public TimeSpan? StartLimit { get; }
        public TimeSpan? EndLimit { get; }
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

    public enum MonthlyFrecuencyEnum
    {
        First,
        Second,
        Third,
        Fourth,
        Last
    }

    public enum MonthlyDayEnum
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday,
        Day,
        Weekday,
        WeekendDay
    }

    public enum LanguageEnum
    {
        EnglishUK,
        EnglishUS,
        Spanish
    }
}

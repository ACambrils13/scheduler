using Scheduler.Auxiliary;
using System;

namespace Scheduler.Configuration
{
    public class SchedulerConfigurator
    {

        public SchedulerConfigurator() { }

        public DateTime? CurrentDate { get; set; }
        public ScheduleTypeEnum? Type { get; set; }
        public LimitsConfig? DateLimits { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public OccurrencyPeriodEnum? PeriodType { get; set; }
        public int? OcurrencyPeriod { get; set; }
        public DateTime? DailyScheduleHour { get; set; }
        public DailyFrecuencyEnum? DailyFrecuency { get; set; }
        public int? DailyFrecuencyPeriod { get; set; }
        public LimitsConfig? DailyLimits { get; set; }
        public DayOfWeek[] WeeklyDays { get; set; }
    }
}

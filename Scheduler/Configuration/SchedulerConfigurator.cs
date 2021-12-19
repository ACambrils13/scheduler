using Scheduler.Auxiliary;
using System;
using System.Collections.Generic;

namespace Scheduler.Configuration
{
    public class SchedulerConfigurator
    {

        public SchedulerConfigurator() { }

        public DateTime? CurrentDate { get; set; }
        public ScheduleTypeEnum? Type { get; set; }
        public DateLimitsConfig? DateLimits { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public OccurrencyPeriodEnum? PeriodType { get; set; }
        public int? OcurrencyPeriod { get; set; }
        public TimeSpan? DailyScheduleHour { get; set; }
        public DailyFrecuencyEnum? DailyFrecuency { get; set; }
        public int? DailyFrecuencyPeriod { get; set; }
        public HourLimitsConfig? DailyLimits { get; set; }
        public List<DayOfWeek> WeeklyDays { get; set; }
        public bool? MonthlyDaySelection { get; set; }
        public int? MonthlyDay { get; set; }
        public MonthlyFrecuencyEnum? MonthlyFrecuency { get; set; }
        public MonthlyDayEnum? MonthlyWeekday { get; set; }
    }
}

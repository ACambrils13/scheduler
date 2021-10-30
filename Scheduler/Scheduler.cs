using System;

namespace Scheduler
{
    public class Scheduler
    {

        public Scheduler() { }

        public DateTime? CurrentDate { get; set; }
        public ScheduleTypeEnum? Type { get; set; }
        public LimitsConfig? DateLimits { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public OccurrencyPeriodEnum? PeriodType { get; set; }
        public int? OcurrencyPeriod { get; set; }
        public Week? WeeklyDays { get; set; }
        public DailyFrecuencyEnum? DailyFrecuency { get; set; }

        public DateTime? DailyScheduleHour { get; set; }
        public int? DailyFrecuencyPeriod { get; set; }
        public LimitsConfig? DailyLimits { get; set; }

        public ScheduleEvent GetNextExecution()
        {
            ScheduleConfigurator Configurator = new ScheduleConfigurator(this);
            return Configurator.ScheduleNextExecution();
        }
    }
}

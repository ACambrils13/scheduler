using System;

namespace Scheduler
{
    public class Scheduler
    {
        private ScheduleConfig configurator;

        public Scheduler() 
        {
            this.configurator = new ScheduleConfig();
        }

        public void ConfigureScheduler (DateTime CurrentDate, ScheduleTypeEnum Type, DateTime? ExecutionDate, OccurrencyPeriodEnum? RecPeriodType, )

        public ScheduleEvent GetNextExecution(DateTime CurrentDate, ScheduleTypeEnum Type, DateTime? ExecutionDate, OccurrencyPeriodEnum? PeriodType, int? Period, DateTime? Start, DateTime? End)
        {

            LimitsConfig Limits = new(Start, End);
            switch (Type)
            {
                case ScheduleTypeEnum.Once:
                    this.Configurator = new ScheduleConfigOnce(CurrentDate, Type, Limits, ExecutionDate);
                    break;
                case ScheduleTypeEnum.Recurring:
                    this.Configurator = new ScheduleConfigRecurring(CurrentDate, Type, Limits, PeriodType, Period);
                    break;
            }

            return this.Configurator.ScheduleNextExecution();
        }
    }
}

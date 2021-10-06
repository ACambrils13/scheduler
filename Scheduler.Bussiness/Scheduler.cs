using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class Scheduler
    {
        private ScheduleConfig Configurator;

        public Scheduler() { }

        public ScheduleEvent GetNextExecution(DateTime CurrentDate, ScheduleType Type, DateTime? ExecutionDate, OccurrencyPeriod PeriodType, int? Period, DateTime? Start, DateTime? End)
        {
            LimitsConfig Limits = new(Start, End);
            switch (Type)
            {
                case ScheduleType.Once:
                    this.Configurator = new ScheduleConfigOnce(CurrentDate, Type, Limits, ExecutionDate);
                    break;
                case ScheduleType.Recurring:
                    this.Configurator = new ScheduleConfigRecurring(CurrentDate, Type, Limits, PeriodType, Period);
                    break;
            }

            return this.Configurator.ScheduleNextExecution();
        }
    }
}

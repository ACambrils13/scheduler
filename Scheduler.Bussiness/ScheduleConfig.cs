using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public abstract class ScheduleConfig
    {
        public ScheduleConfig() { }

        public static abstract ScheduleEvent ScheduleNextExecution(DateTime CurrentDate) { }

        public ScheduleType Type { get; private set; }
        public string ExecutionDescription { get; private set; }
    }

    public class ScheduleConfigOnce : ScheduleConfig
    {

    }

    public class ScheduleConfigRecurring : ScheduleConfig
    {

    }

    public class Scheduler
    {
        public Scheduler() { }

        public ScheduleConfig SetConfig (ScheduleType Type, OccurrencyPeriod )
    }

    public enum ScheduleType
    {
        Once,
        Recurring
    }

    public enum OccurrencyPeriod
    {
        Daily = 1,
        Weekly = 7,
        Monthly,
        Yearly
    }
}

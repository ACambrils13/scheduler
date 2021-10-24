using Scheduler.Resources;
using System;

namespace Scheduler
{
    public class ScheduleEvent
    {
        public DateTime ExecutionDate { get; set; }
        public string ExecutionDescription { get; set; }
    }
}

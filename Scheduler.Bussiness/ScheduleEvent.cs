using Scheduler.Resources;
using System;

namespace Scheduler
{
    public class ScheduleEvent
    {
        public ScheduleEvent(DateTime ExecutionTime, ScheduleType ExecutionType, LimitsConfig Limits)
        {
            this.ExecutionTime = ExecutionTime;
            this.ExecutionType = ExecutionType;
            this.Limits = Limits;
            this.FormatEventDescription(); 
        }

        public DateTime ExecutionTime { get; private set; }
        public ScheduleType ExecutionType { get; private set; }
        public string ExecutionDescription { get; private set; }
        public LimitsConfig Limits { get; private set; }

        private void FormatEventDescription()
        {
            string ExectionDate = this.ExecutionTime.ToString("d");
            string ExecutionHour = this.ExecutionTime.ToString("t");
            this.ExecutionDescription = string.Format(TextResources.EventScheduleDescription, this.ExecutionType.ToString(), ExectionDate, ExecutionHour, this.Limits.StartDate.ToString("d"));
        }
    }
}

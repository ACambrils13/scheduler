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

        public DateTime ExecutionTime { get; }
        public ScheduleType ExecutionType { get; }
        public string ExecutionDescription { get; set; }
        public LimitsConfig Limits { get; }

        private void FormatEventDescription()
        {
            string ExectionDate = this.ExecutionTime.ToString("d");
            string ExecutionHour = this.ExecutionTime.ToString("t");
            string StartDate = this.Limits.StartDate?.ToString("d") ?? TextResources.NotDefined;
            this.ExecutionDescription = string.Format(TextResources.EventScheduleDescription, 
                this.ExecutionType.ToString(), ExectionDate, ExecutionHour, StartDate);
        }
    }
}

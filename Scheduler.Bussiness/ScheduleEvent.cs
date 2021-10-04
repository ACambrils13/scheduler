using Scheduler.Resources;
using System;

namespace Scheduler
{
    public class ScheduleEvent
    {
        public ScheduleEvent(DateTime ExecutionTime, ScheduleType ExecutionType, DateTime StartDate)
        {
            ValidateDate(StartDate);
            ValidateDate(ExecutionTime);

            this.ExecutionTime = ExecutionTime;
            this.ExecutionType = ExecutionType;
            this.ExecutionStartDate = StartDate;
            this.FormatEventDescription();
            
        }

        public DateTime ExecutionTime { get; private set; }
        public ScheduleType ExecutionType { get; private set; }
        public DateTime ExecutionStartDate { get; private set; }
        public string ExecutionDescription { get; private set; }

        private static void ValidateDate (DateTime Date)
        {
            if (string.IsNullOrWhiteSpace(Date.ToString()))
            {
                throw new Exception(TextResources.ExcDate);
            }
        }

        private void FormatEventDescription()
        {
            string ExectionDate = this.ExecutionTime.ToString("d");
            string ExecutionHour = this.ExecutionTime.ToString("t");
            this.ExecutionDescription = string.Format(TextResources.EventScheduleDescription, this.ExecutionType.ToString(), ExectionDate, ExecutionHour, this.ExecutionStartDate.ToString("d"));
        }
    }
}

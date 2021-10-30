namespace Scheduler
{
    public class ScheduleOnce
    {
        public static ScheduleEvent GetNextExecution(Scheduler configuration)
        {
            ScheduleConfigValidator.ValidateDateNullable(configuration.ScheduleDate, nameof(configuration.ScheduleDate));
            string Description = EventDescriptionFormatter.GetScheduleOnceDesc(configuration.ScheduleDate.Value, configuration.DateLimits);
            return new ScheduleEvent()
            {
                ExecutionDate = configuration.ScheduleDate.Value,
                ExecutionDescription = Description
            };
        }
    }
}

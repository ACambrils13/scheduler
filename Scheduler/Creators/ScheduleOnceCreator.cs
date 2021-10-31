namespace Scheduler.Creators
{
    internal class ScheduleOnceCreator : ScheduleEventCreator
    {
        internal ScheduleOnceCreator(Scheduler scheduler) : base(scheduler) { }

        internal override ScheduleEvent GetNextExecution()
        {
            ScheduleConfigValidator.ValidateOnceSchedule(this.configuration);
            string Description = EventDescriptionFormatter.GetScheduleOnceDesc(this.configuration.ScheduleDate.Value, this.configuration.DateLimits);
            return new ScheduleEvent()
            {
                ExecutionDate = this.configuration.ScheduleDate.Value,
                ExecutionDescription = Description
            };
        }
    }
}

namespace Scheduler.Creators
{
    internal class ScheduleOnceCreator : ScheduleEventCreator
    {
        internal ScheduleOnceCreator(Scheduler scheduler) : base(scheduler) { }

        internal override ScheduleEvent GetNextExecution()
        {
            ScheduleConfigValidator.ValidateDateNullable(this.configuration.ScheduleDate, nameof(this.configuration.ScheduleDate));
            string Description = EventDescriptionFormatter.GetScheduleOnceDesc(this.configuration.ScheduleDate.Value, this.configuration.DateLimits);
            return new ScheduleEvent()
            {
                ExecutionDate = this.configuration.ScheduleDate.Value,
                ExecutionDescription = Description
            };
        }
    }
}

namespace Scheduler.Creators
{
    internal abstract class ScheduleEventCreator
    {
        internal readonly Scheduler configuration;

        internal ScheduleEventCreator(Scheduler scheduler)
        {
            this.configuration = scheduler;
        }

        internal abstract ScheduleEvent GetNextExecution();
    }
}

namespace Scheduler.Creators
{
    internal abstract class ScheduleEventCreator
    {

        internal ScheduleEventCreator()
        { }

        internal abstract ScheduleEvent GetNextExecution(SchedulerConfigurator config);
    }
}

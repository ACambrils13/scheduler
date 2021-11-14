using Scheduler.Auxiliary;
using Scheduler.Configuration;
using Scheduler.Validators;

namespace Scheduler.Creators
{
    internal class ScheduleOnceCreator : ScheduleEventCreator
    {

        internal override ScheduleEvent GetNextExecution(SchedulerConfigurator config)
        {
            ScheduleConfigValidator.ValidateOnceSchedule(config);
            string Description = EventDescriptionFormatter.GetScheduleOnceDesc(config.ScheduleDate.Value, config.DateLimits);
            return new ScheduleEvent()
            {
                ExecutionDate = config.ScheduleDate.Value,
                ExecutionDescription = Description
            };
        }
    }
}

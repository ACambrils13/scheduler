using Scheduler.Auxiliary;
using Scheduler.Configuration;
using Scheduler.Creators;
using Scheduler.Validators;

namespace Scheduler
{
    public class Scheduler
    {
        public static ScheduleEvent GetNextExecution(SchedulerConfigurator config)
        {
            ScheduleConfigValidator.ValidateBasicProperties(config);
            ScheduleEventCreator eventCreator = config.Type switch
            {
                ScheduleTypeEnum.Once => new ScheduleOnceCreator(),
                ScheduleTypeEnum.Recurring => new ScheduleRecurringCreator(),
                _ => new ScheduleOnceCreator(),
            };
            return eventCreator.GetNextExecution(config);
        }
    }
}


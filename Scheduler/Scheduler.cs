using Scheduler.Creators;

namespace Scheduler
{
    public class Scheduler
    {
        private ScheduleEventCreator eventCreator;
        public Scheduler()
        { }

        public ScheduleEvent ScheduleNextExecution(SchedulerConfigurator config)
        {
            ScheduleConfigValidator.ValidateBasicProperties(config);
            switch (config.Type)
            {
                case ScheduleTypeEnum.Once:
                    this.eventCreator = new ScheduleOnceCreator();
                    break;
                case ScheduleTypeEnum.Recurring:
                    this.eventCreator = new ScheduleRecurringCreator();
                    break;
            }

            return this.eventCreator.GetNextExecution(config);
        }
    }
}


using Scheduler.Creators;

namespace Scheduler
{
    public class ScheduleConfigurator
    {
        private readonly Scheduler scheduleProperties;
        private ScheduleEventCreator eventCreator;
        public ScheduleConfigurator(Scheduler scheduleProperties)
        {
            this.scheduleProperties = scheduleProperties;
        }

        public ScheduleEvent ScheduleNextExecution()
        {
            ScheduleConfigValidator.ValidateBasicProperties(this.scheduleProperties);
            switch (this.scheduleProperties.Type)
            {
                case ScheduleTypeEnum.Once:
                    this.eventCreator = new ScheduleOnceCreator(this.scheduleProperties);
                    break;
                case ScheduleTypeEnum.Recurring:
                    this.eventCreator = new ScheduleRecurringCreator(this.scheduleProperties);
                    break;
            }

            return this.eventCreator.GetNextExecution();
        }
    }
}


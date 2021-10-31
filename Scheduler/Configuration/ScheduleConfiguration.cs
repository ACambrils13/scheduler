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

    //public class ScheduleOnce
    //{
    //    public static ScheduleEvent GetNextExecution(Scheduler configuration)
    //    {
    //        return null;
    //    }

        //public ScheduleConfigOnce(DateTime CurrentDate, ScheduleTypeEnum Type, LimitsConfig Limits, DateTime? ExecutionDate)
        //    : base(CurrentDate, Type, Limits) 
        //{
        //    Auxiliary.CheckNotNull(new object[] { ExecutionDate });
        //    this.ScheduleDate = ExecutionDate.Value;
        //}

        //public override ScheduleEvent ScheduleNextExecution()
        //{
        //    return new ScheduleEvent(this.ScheduleDate, this.Type, this.DateLimits);
        //}
    //}

    //public class ScheduleConfigRecurring : ScheduleConfig
    //{
    //    public ScheduleConfigRecurring(DateTime CurrentDate, ScheduleTypeEnum Type, LimitsConfig Limits, OccurrencyPeriodEnum? PeriodType, int? Period)
    //        : base(CurrentDate, Type, Limits)
    //    {
    //        Auxiliary.CheckNotNull(new object[] { Period, PeriodType });
    //        this.PeriodType = PeriodType.Value;
    //        this.OcurrencyPeriod = Period.Value;
    //    }

    //    public override ScheduleEvent ScheduleNextExecution()
    //    {
    //        switch (this.PeriodType)
    //        {
    //            case OccurrencyPeriodEnum.Daily:
    //                this.ScheduleDate = this.CurrentDate.AddDays(this.OcurrencyPeriod);
    //                break;
    //            case OccurrencyPeriodEnum.Monthly:
    //                this.ScheduleDate = this.CurrentDate.AddMonths(this.OcurrencyPeriod);
    //                break;
    //            case OccurrencyPeriodEnum.Weekly:
    //                this.ScheduleDate = this.CurrentDate.AddDays(this.OcurrencyPeriod * 7);
    //                break;
    //            case OccurrencyPeriodEnum.Yearly:
    //                this.ScheduleDate = this.CurrentDate.AddYears(this.OcurrencyPeriod);
    //                break;
    //        }
    //        return new ScheduleEvent(this.ScheduleDate, this.Type, this.DateLimits);
    //    }
    //}
}


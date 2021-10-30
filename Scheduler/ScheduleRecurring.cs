using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    internal class ScheduleRecurring
    {
        private static Scheduler Config { get; set; }

        internal static ScheduleEvent GetNextExecution(Scheduler configuration)
        {
            ScheduleConfigValidator.ValidateRecurringSchedule(configuration);

            Config = configuration;

            switch (Config.PeriodType)
            {
                case OccurrencyPeriodEnum.Daily:
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    break;
            }


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

//public ScheduleConfigRecurring(DateTime CurrentDate, ScheduleTypeEnum Type, LimitsConfig Limits, OccurrencyPeriodEnum? PeriodType, int? Period)
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

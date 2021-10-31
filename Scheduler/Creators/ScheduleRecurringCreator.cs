using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Creators
{
    internal class ScheduleRecurringCreator : ScheduleEventCreator
    {
        private DateTime NextExecutionDate;

        internal ScheduleRecurringCreator(Scheduler scheduler) : base(scheduler) { }

        internal override ScheduleEvent GetNextExecution()
        {
            ScheduleConfigValidator.ValidateRecurringSchedule(this.configuration);

            switch (this.configuration.PeriodType)
            {
                case OccurrencyPeriodEnum.Daily:
                    return this.GetNextExecutionDaily();
                case OccurrencyPeriodEnum.Weekly:
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    return this.GetNextExecutionMonthly();
                case OccurrencyPeriodEnum.Yearly:
                    return this.GetNextExecutionYearly();
            }


            ScheduleConfigValidator.ValidateDateNullable(configuration.ScheduleDate, nameof(configuration.ScheduleDate));
            string Description = EventDescriptionFormatter.GetScheduleOnceDesc(configuration.ScheduleDate.Value, configuration.DateLimits);
            return new ScheduleEvent()
            {
                ExecutionDate = configuration.ScheduleDate.Value,
                ExecutionDescription = Description
            };
        }

        private ScheduleEvent GetNextExecutionDaily()
        {
            this.NextExecutionDate = this.configuration.CurrentDate.Value.AddDays(this.configuration.OcurrencyPeriod.Value);
            DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate.Date);
            while (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate.AddDays(this.configuration.OcurrencyPeriod.Value);
                NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            }
            string Description = EventDescriptionFormatter.GetScheduleRecurrentDesc(this.configuration);
            return new ScheduleEvent()
            {
                ExecutionDate = this.NextExecutionDate,
                ExecutionDescription = Description
            };
        }

        private ScheduleEvent GetNextExecutionMonthly()
        {
            this.NextExecutionDate = this.configuration.CurrentDate.Value.AddMonths(this.configuration.OcurrencyPeriod.Value);
            DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate.Date);
            while (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate.AddMonths(this.configuration.OcurrencyPeriod.Value);
                NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            }
            string Description = EventDescriptionFormatter.GetScheduleRecurrentDesc(this.configuration);
            return new ScheduleEvent()
            {
                ExecutionDate = this.NextExecutionDate,
                ExecutionDescription = Description
            };
        }

        private ScheduleEvent GetNextExecutionYearly()
        {
            this.NextExecutionDate = this.configuration.CurrentDate.Value.AddYears(this.configuration.OcurrencyPeriod.Value);
            DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate.Date);
            while (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate.AddYears(this.configuration.OcurrencyPeriod.Value);
                NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            }
            string Description = EventDescriptionFormatter.GetScheduleRecurrentDesc(this.configuration);
            return new ScheduleEvent()
            {
                ExecutionDate = this.NextExecutionDate,
                ExecutionDescription = Description
            };
        }


        private DateTime CalculateDailyConfigHour(DateTime ExecDate)
        {
            DateTime NewDate;
            if (this.configuration.DailyScheduleHour.HasValue)
            {
                NewDate = ExecDate.Date.Add(this.configuration.DailyScheduleHour.Value.TimeOfDay);
            }
            else 
            {
                NewDate = this.CalculateDailyConfigHourReccurent(ExecDate);
            }
            return NewDate;
        }

        private DateTime CalculateDailyConfigHourReccurent(DateTime ExecDate)
        {
            DateTime NewDate = ExecDate.Date;
            DateTime StartLimit = ExecDate.Date;
            DateTime EndLimit = ExecDate.Date;

            if (this.configuration.DailyLimits.HasValue)
            {
                StartLimit.Add(this.configuration.DailyLimits.Value.StartLimit.HasValue
                    ? this.configuration.DailyLimits.Value.StartLimit.Value.TimeOfDay
                    : TimeSpan.Zero);
                EndLimit.Add(this.configuration.DailyLimits.Value.EndLimit.HasValue
                    ? this.configuration.DailyLimits.Value.EndLimit.Value.TimeOfDay
                    : TimeSpan.FromDays(1));
                NewDate = StartLimit;
            }
            while (DateTime.Compare(ExecDate, NewDate) > 0)
            {
                switch (this.configuration.DailyFrecuency)
                {
                    case DailyFrecuencyEnum.Hours:
                        NewDate.AddHours(this.configuration.DailyFrecuencyPeriod.Value);
                        break;
                    case DailyFrecuencyEnum.Minutes:
                        NewDate.AddMinutes(this.configuration.DailyFrecuencyPeriod.Value);
                        break;
                    case DailyFrecuencyEnum.Seconds:
                        NewDate.AddSeconds(this.configuration.DailyFrecuencyPeriod.Value);
                        break;
                }
                if (DateTime.Compare(NewDate, EndLimit) > 0)
                {
                    NewDate = EndLimit;
                    break;
                }
            }
            return NewDate;
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

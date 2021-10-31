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
            ScheduleEvent NextEvent = new();
            switch (this.configuration.PeriodType)
            {
                case OccurrencyPeriodEnum.Daily:
                    NextEvent = this.GetNextExecutionDaily();
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    NextEvent = this.GetNextExecutionMonthly();
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    NextEvent = this.GetNextExecutionYearly();
                    break;
            }
            ScheduleConfigValidator.ValidateLimits(NextEvent.ExecutionDate, this.configuration.DateLimits);
            return NextEvent;
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
            this.NextExecutionDate = NewDate;
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
            this.NextExecutionDate = NewDate;
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
            this.NextExecutionDate = NewDate;
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
            DateTime EndLimit = ExecDate.Date.AddDays(1);

            if (this.configuration.DailyLimits.HasValue)
            {
                if (this.configuration.DailyLimits.Value.StartLimit.HasValue)
                {
                    StartLimit = StartLimit.Add(this.configuration.DailyLimits.Value.StartLimit.Value.TimeOfDay);
                }
                if (this.configuration.DailyLimits.Value.EndLimit.HasValue)
                {
                    EndLimit = ExecDate.Date.Add(this.configuration.DailyLimits.Value.EndLimit.Value.TimeOfDay);
                }
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

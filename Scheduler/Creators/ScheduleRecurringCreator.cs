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
            this.NextExecutionDate = this.configuration.CurrentDate.Value;
            if (this.configuration.DateLimits?.StartLimit != null
                && DateTime.Compare(this.NextExecutionDate,this.configuration.DateLimits.Value.StartLimit.Value) < 0)
            {
                this.NextExecutionDate = this.configuration.DateLimits.Value.StartLimit.Value;
            }
            DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                switch (this.configuration.PeriodType)
                {
                    case OccurrencyPeriodEnum.Daily:
                        this.GetNextExecutionDaily();
                        break;
                    case OccurrencyPeriodEnum.Weekly:
                        this.GetNextExecutionWeekly();
                        break;
                    case OccurrencyPeriodEnum.Monthly:
                        this.GetNextExecutionMonthly();
                        break;
                    case OccurrencyPeriodEnum.Yearly:
                        this.GetNextExecutionYearly();
                        break;
                }
                NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            } 
            this.NextExecutionDate = NewDate;
            ScheduleConfigValidator.ValidateLimits(this.NextExecutionDate, this.configuration.DateLimits, false);

            string Description = EventDescriptionFormatter.GetScheduleRecurrentDesc(this.configuration);
            return new ScheduleEvent()
            {
                ExecutionDate = this.NextExecutionDate,
                ExecutionDescription = Description
            };
            
        }

        private void GetNextExecutionDaily()
        {
            this.NextExecutionDate = this.NextExecutionDate.AddDays(this.configuration.OcurrencyPeriod.Value).Date;
        }

        private void GetNextExecutionWeekly()
        {
            this.NextExecutionDate = this.NextExecutionDate.AddDays(this.configuration.OcurrencyPeriod.Value * 7).Date;
        }


        private void GetNextExecutionMonthly()
        {
            this.NextExecutionDate = this.NextExecutionDate.AddMonths(this.configuration.OcurrencyPeriod.Value).Date;
        }

        private void GetNextExecutionYearly()
        {
            this.NextExecutionDate = this.NextExecutionDate.AddYears(this.configuration.OcurrencyPeriod.Value).Date;
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
            DateTime NewDate = ExecDate;
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
                        NewDate = NewDate.AddHours(this.configuration.DailyFrecuencyPeriod.Value);
                        break;
                    case DailyFrecuencyEnum.Minutes:
                        NewDate = NewDate.AddMinutes(this.configuration.DailyFrecuencyPeriod.Value);
                        break;
                    case DailyFrecuencyEnum.Seconds:
                        NewDate = NewDate.AddSeconds(this.configuration.DailyFrecuencyPeriod.Value);
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

using System;
using System.Globalization;
using System.Linq;

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
                && DateTime.Compare(this.NextExecutionDate, this.configuration.DateLimits.Value.StartLimit.Value) < 0)
            {
                this.NextExecutionDate = this.configuration.DateLimits.Value.StartLimit.Value;
            }

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
            DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate = this.NextExecutionDate.AddDays(this.configuration.OcurrencyPeriod.Value).Date;
            }
            this.NextExecutionDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
        }

        private void GetNextExecutionWeekly()
        {
            if (this.configuration.WeeklyDays != null && this.configuration.WeeklyDays.Length > 0)
            {
                this.NextExecutionDate = this.CalculateWeeklyConfigDay(this.NextExecutionDate);
                DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
                if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
                {
                    this.NextExecutionDate = this.CalculateWeeklyConfigDay(this.NextExecutionDate.AddDays(1).Date);
                }
                this.NextExecutionDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            }
            else
            {
                DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
                if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
                {
                    this.NextExecutionDate = this.NextExecutionDate.AddDays(this.configuration.OcurrencyPeriod.Value * 7).Date;
                }
                this.NextExecutionDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            }
        }

        private void GetNextExecutionMonthly()
        {
            DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate = this.NextExecutionDate.AddMonths(this.configuration.OcurrencyPeriod.Value).Date;
            }
            this.NextExecutionDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
        }

        private void GetNextExecutionYearly()
        {
            DateTime NewDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
            if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate = this.NextExecutionDate.AddYears(this.configuration.OcurrencyPeriod.Value).Date;
            }
            this.NextExecutionDate = this.CalculateDailyConfigHour(this.NextExecutionDate);
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

        private DateTime CalculateWeeklyConfigDay(DateTime ExecDate)
        {
            DateTime NewDate = ExecDate;
            if (this.configuration.WeeklyDays.Contains(ExecDate.DayOfWeek) == false)
            {
                int? OffsetDays = null;
                if (CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
                {
                    try
                    {
                        OffsetDays = (int)this.configuration.WeeklyDays.First(day => (int)day > (int)ExecDate.DayOfWeek);
                    }
                    catch
                    {
                        int DiffFirstDayOfWeek = (int)ExecDate.DayOfWeek - (int)DayOfWeek.Sunday;
                        OffsetDays = (this.configuration.OcurrencyPeriod.Value * 7) - DiffFirstDayOfWeek;
                        OffsetDays += (int)this.configuration.WeeklyDays.Min(day => (int)day);
                    }
                }
                else if (this.NextExecutionDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    try
                    {
                        OffsetDays = (int)this.configuration.WeeklyDays.First(day => (int)day > (int)ExecDate.DayOfWeek);
                    }
                    catch
                    {
                        if (this.configuration.WeeklyDays.Contains(DayOfWeek.Sunday))
                        {
                            OffsetDays = 7 - (int)ExecDate.DayOfWeek;
                        }
                        else
                        {
                            int DiffFirstDayOfWeek = (int)ExecDate.DayOfWeek - (int)DayOfWeek.Monday;
                            OffsetDays = (this.configuration.OcurrencyPeriod.Value * 7) - DiffFirstDayOfWeek;
                            OffsetDays += ((int)this.configuration.WeeklyDays.Min(day => (int)day) - (int)DayOfWeek.Monday);
                        }
                    }
                }
                else
                {
                    OffsetDays = (this.configuration.OcurrencyPeriod.Value * 7) - 6;
                    OffsetDays += ((int)this.configuration.WeeklyDays.Min(day => (int)day) - (int)DayOfWeek.Monday);
                }

                NewDate = this.NextExecutionDate.AddDays(OffsetDays.Value).Date;
            }
            return NewDate;
        }
    }
}

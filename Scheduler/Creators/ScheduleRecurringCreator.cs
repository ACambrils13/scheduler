using System;
using System.Globalization;
using System.Linq;

namespace Scheduler.Creators
{
    internal class ScheduleRecurringCreator : ScheduleEventCreator
    {
        private DateTime NextExecutionDate;

        internal override ScheduleEvent GetNextExecution(SchedulerConfigurator config)
        {
            ScheduleConfigValidator.ValidateRecurringSchedule(config);
            this.NextExecutionDate = config.CurrentDate.Value;
            if (config.DateLimits?.StartLimit != null
                && DateTime.Compare(this.NextExecutionDate, config.DateLimits.Value.StartLimit.Value) < 0)
            {
                this.NextExecutionDate = config.DateLimits.Value.StartLimit.Value;
            }

            switch (config.PeriodType)
            {
                case OccurrencyPeriodEnum.Daily:
                    this.GetNextExecutionDaily(config);
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    this.GetNextExecutionWeekly(config);
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    this.GetNextExecutionMonthly(config);
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    this.GetNextExecutionYearly(config);
                    break;
            }
            ScheduleConfigValidator.ValidateLimits(this.NextExecutionDate, config.DateLimits, false);

            string Description = EventDescriptionFormatter.GetScheduleRecurrentDesc(config);
            return new ScheduleEvent()
            {
                ExecutionDate = this.NextExecutionDate,
                ExecutionDescription = Description
            };

        }

        private void GetNextExecutionDaily(SchedulerConfigurator config)
        {
            DateTime NewDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate = this.NextExecutionDate.AddDays(config.OcurrencyPeriod.Value).Date;
            }
            this.NextExecutionDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
        }

        private void GetNextExecutionWeekly(SchedulerConfigurator config)
        {
            if (config.WeeklyDays != null && config.WeeklyDays.Length > 0)
            {
                this.NextExecutionDate = this.CalculateWeeklyConfigDay(config, this.NextExecutionDate);
                DateTime NewDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
                if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
                {
                    this.NextExecutionDate = this.CalculateWeeklyConfigDay(config, this.NextExecutionDate.AddDays(1).Date);
                }
                this.NextExecutionDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            }
            else
            {
                DateTime NewDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
                if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
                {
                    this.NextExecutionDate = this.NextExecutionDate.AddDays(config.OcurrencyPeriod.Value * 7).Date;
                }
                this.NextExecutionDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            }
        }

        private void GetNextExecutionMonthly(SchedulerConfigurator config)
        {
            DateTime NewDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate = this.NextExecutionDate.AddMonths(config.OcurrencyPeriod.Value).Date;
            }
            this.NextExecutionDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
        }

        private void GetNextExecutionYearly(SchedulerConfigurator config)
        {
            DateTime NewDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            {
                this.NextExecutionDate = this.NextExecutionDate.AddYears(config.OcurrencyPeriod.Value).Date;
            }
            this.NextExecutionDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
        }


        private DateTime CalculateDailyConfigHour(SchedulerConfigurator config, DateTime ExecDate)
        {
            DateTime NewDate;
            if (config.DailyScheduleHour.HasValue)
            {
                NewDate = ExecDate.Date.Add(config.DailyScheduleHour.Value.TimeOfDay);
            }
            else
            {
                NewDate = this.CalculateDailyConfigHourReccurent(config, ExecDate);
            }
            return NewDate;
        }

        private DateTime CalculateDailyConfigHourReccurent(SchedulerConfigurator config, DateTime ExecDate)
        {
            DateTime NewDate = ExecDate;
            DateTime StartLimit = ExecDate.Date;
            DateTime EndLimit = ExecDate.Date.AddDays(1);

            if (config.DailyLimits.HasValue)
            {
                if (config.DailyLimits.Value.StartLimit.HasValue)
                {
                    StartLimit = StartLimit.Add(config.DailyLimits.Value.StartLimit.Value.TimeOfDay);
                }
                if (config.DailyLimits.Value.EndLimit.HasValue)
                {
                    EndLimit = ExecDate.Date.Add(config.DailyLimits.Value.EndLimit.Value.TimeOfDay);
                }
                NewDate = StartLimit;
            }
            while (DateTime.Compare(ExecDate, NewDate) > 0)
            {
                switch (config.DailyFrecuency)
                {
                    case DailyFrecuencyEnum.Hours:
                        NewDate = NewDate.AddHours(config.DailyFrecuencyPeriod.Value);
                        break;
                    case DailyFrecuencyEnum.Minutes:
                        NewDate = NewDate.AddMinutes(config.DailyFrecuencyPeriod.Value);
                        break;
                    case DailyFrecuencyEnum.Seconds:
                        NewDate = NewDate.AddSeconds(config.DailyFrecuencyPeriod.Value);
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

        private DateTime CalculateWeeklyConfigDay(SchedulerConfigurator config, DateTime ExecDate)
        {
            DateTime NewDate = ExecDate;
            if (config.WeeklyDays.Contains(ExecDate.DayOfWeek) == false)
            {
                int? OffsetDays = null;
                if (CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
                {
                    try
                    {
                        OffsetDays = (int)config.WeeklyDays.First(day => (int)day > (int)ExecDate.DayOfWeek);
                    }
                    catch
                    {
                        int DiffFirstDayOfWeek = (int)ExecDate.DayOfWeek - (int)DayOfWeek.Sunday;
                        OffsetDays = (config.OcurrencyPeriod.Value * 7) - DiffFirstDayOfWeek;
                        OffsetDays += (int)config.WeeklyDays.Min(day => (int)day);
                    }
                }
                else if (this.NextExecutionDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    try
                    {
                        OffsetDays = (int)config.WeeklyDays.First(day => (int)day > (int)ExecDate.DayOfWeek);
                    }
                    catch
                    {
                        if (config.WeeklyDays.Contains(DayOfWeek.Sunday))
                        {
                            OffsetDays = 7 - (int)ExecDate.DayOfWeek;
                        }
                        else
                        {
                            int DiffFirstDayOfWeek = (int)ExecDate.DayOfWeek - (int)DayOfWeek.Monday;
                            OffsetDays = (config.OcurrencyPeriod.Value * 7) - DiffFirstDayOfWeek;
                            OffsetDays += ((int)config.WeeklyDays.Min(day => (int)day) - (int)DayOfWeek.Monday);
                        }
                    }
                }
                else
                {
                    OffsetDays = (config.OcurrencyPeriod.Value * 7) - 6;
                    OffsetDays += ((int)config.WeeklyDays.Min(day => (int)day) - (int)DayOfWeek.Monday);
                }

                NewDate = this.NextExecutionDate.AddDays(OffsetDays.Value).Date;
            }
            return NewDate;
        }
    }
}

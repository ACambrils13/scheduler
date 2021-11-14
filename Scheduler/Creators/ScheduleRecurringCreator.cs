using Scheduler.Auxiliary;
using Scheduler.Configuration;
using Scheduler.Validators;
using System;
using System.Globalization;
using System.Linq;

namespace Scheduler.Creators
{
    internal class ScheduleRecurringCreator : ScheduleEventCreator
    {
        internal override ScheduleEvent GetNextExecution(SchedulerConfigurator config)
        {
            ScheduleConfigValidator.ValidateRecurringSchedule(config);
            DateTime NextExecutionDate = config.CurrentDate.Value.CurrentDateOrStartLimit(config.DateLimits?.StartLimit);

            switch (config.PeriodType)
            {
                case OccurrencyPeriodEnum.Daily:
                    NextExecutionDate = GetNextExecutionDaily(config, NextExecutionDate);
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    NextExecutionDate = GetNextExecutionWeekly(config, NextExecutionDate);
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    NextExecutionDate = GetNextExecutionMonthly(config, NextExecutionDate);
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    NextExecutionDate = GetNextExecutionYearly(config, NextExecutionDate);
                    break;
            }
            ScheduleConfigValidator.ValidateLimits(NextExecutionDate, config.DateLimits, false);

            string Description = EventDescriptionFormatter.GetScheduleRecurrentDesc(config);
            return new ScheduleEvent()
            {
                ExecutionDate = NextExecutionDate,
                ExecutionDescription = Description
            };

        }

        private static DateTime GetNextExecutionDaily(SchedulerConfigurator config, DateTime nextExec)
        {
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) > 0)
            {
                nextExec = nextExec.AddDays(config.OcurrencyPeriod.Value).Date;
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        private static DateTime GetNextExecutionWeekly(SchedulerConfigurator config, DateTime nextExec)
        {
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) > 0)
            {
                nextExec = nextExec.AddWeeks(config.OcurrencyPeriod.Value).Date;
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;

            //if (config.WeeklyDays != null && config.WeeklyDays.Length > 0)
            //{
            //    this.NextExecutionDate = this.CalculateWeeklyConfigDay(config, this.NextExecutionDate);
            //    DateTime NewDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            //    if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            //    {
            //        this.NextExecutionDate = this.CalculateWeeklyConfigDay(config, this.NextExecutionDate.AddDays(1).Date);
            //    }
            //    this.NextExecutionDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            //}
            //else
            //{
            //    DateTime NewDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            //    if (DateTime.Compare(this.NextExecutionDate, NewDate) > 0)
            //    {
            //        this.NextExecutionDate = this.NextExecutionDate.AddDays(config.OcurrencyPeriod.Value * 7).Date;
            //    }
            //    this.NextExecutionDate = this.CalculateDailyConfigHour(config, this.NextExecutionDate);
            //}
        }

        private static DateTime GetNextExecutionMonthly(SchedulerConfigurator config, DateTime nextExec)
        {
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) > 0)
            {
                nextExec = nextExec.AddMonths(config.OcurrencyPeriod.Value).Date;
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        private static DateTime GetNextExecutionYearly(SchedulerConfigurator config, DateTime nextExec)
        {
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) > 0)
            {
                nextExec = nextExec.AddYears(config.OcurrencyPeriod.Value).Date;
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        #region Daily
        private static DateTime CalculateDailyConfigHour(SchedulerConfigurator config, DateTime execDate)
        {
            if (config.DailyScheduleHour.HasValue)
            {
                return execDate.Date.Add(config.DailyScheduleHour.Value.TimeOfDay);
            }
            return CalculateDailyConfigHourReccurent(config, execDate);
        }

        private static DateTime CalculateDailyConfigHourReccurent(SchedulerConfigurator config, DateTime execDate)
        {
            DateTime NewDate;
            DateTime StartLimit = execDate.StartDailyLimit(config.DailyLimits?.StartLimit);
            DateTime EndLimit = execDate.EndDailyLimit(config.DailyLimits?.EndLimit);
            NewDate = StartLimit;

            while (DateTime.Compare(execDate, NewDate) > 0)
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
        #endregion

        //private DateTime CalculateWeeklyConfigDay(SchedulerConfigurator config, DateTime ExecDate)
        //{
        //    DateTime NewDate = ExecDate;
        //    if (config.WeeklyDays.Contains(ExecDate.DayOfWeek) == false)
        //    {
        //        int? OffsetDays = null;
        //        if (CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
        //        {
        //            try
        //            {
        //                OffsetDays = (int)config.WeeklyDays.First(day => (int)day > (int)ExecDate.DayOfWeek);
        //            }
        //            catch
        //            {
        //                int DiffFirstDayOfWeek = (int)ExecDate.DayOfWeek - (int)DayOfWeek.Sunday;
        //                OffsetDays = (config.OcurrencyPeriod.Value * 7) - DiffFirstDayOfWeek;
        //                OffsetDays += (int)config.WeeklyDays.Min(day => (int)day);
        //            }
        //        }
        //        else if (this.NextExecutionDate.DayOfWeek != DayOfWeek.Sunday)
        //        {
        //            try
        //            {
        //                OffsetDays = (int)config.WeeklyDays.First(day => (int)day > (int)ExecDate.DayOfWeek);
        //            }
        //            catch
        //            {
        //                if (config.WeeklyDays.Contains(DayOfWeek.Sunday))
        //                {
        //                    OffsetDays = 7 - (int)ExecDate.DayOfWeek;
        //                }
        //                else
        //                {
        //                    int DiffFirstDayOfWeek = (int)ExecDate.DayOfWeek - (int)DayOfWeek.Monday;
        //                    OffsetDays = (config.OcurrencyPeriod.Value * 7) - DiffFirstDayOfWeek;
        //                    OffsetDays += ((int)config.WeeklyDays.Min(day => (int)day) - (int)DayOfWeek.Monday);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            OffsetDays = (config.OcurrencyPeriod.Value * 7) - 6;
        //            OffsetDays += ((int)config.WeeklyDays.Min(day => (int)day) - (int)DayOfWeek.Monday);
        //        }

        //        NewDate = this.NextExecutionDate.AddDays(OffsetDays.Value).Date;
        //    }
        //    return NewDate;
        //}
    }
}

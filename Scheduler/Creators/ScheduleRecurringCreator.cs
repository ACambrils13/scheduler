using Scheduler.Auxiliary;
using Scheduler.Configuration;
using Scheduler.Validators;
using System;
using System.Globalization;

namespace Scheduler.Creators
{
    internal class ScheduleRecurringCreator : ScheduleEventCreator
    {
        internal override ScheduleEvent GetNextExecution(SchedulerConfigurator config)
        {
            ScheduleConfigValidator.ValidateRecurringSchedule(config);
            DateTime NextExecutionDate = GetCurrentDate(config);

            switch (config.PeriodType)
            {
                case OccurrencyPeriodEnum.Daily:
                    NextExecutionDate = GetNextExecutionDaily(config, NextExecutionDate);
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    if (config.WeeklyDays?.Count > 0)
                    {
                        NextExecutionDate = GetNextExecutionWeekly(config, NextExecutionDate);
                    }
                    else
                    {
                        NextExecutionDate = GetNextExecutionWeeklyStandard(config, NextExecutionDate);
                    }
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    NextExecutionDate = GetNextExecutionMonthly(config, NextExecutionDate);
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    NextExecutionDate = GetNextExecutionYearly(config, NextExecutionDate);
                    break;
            }
            ScheduleConfigValidator.ValidateLimits(NextExecutionDate, config.DateLimits, false);
            config.ScheduleDate = NextExecutionDate;

            string Description = EventDescriptionFormatter.GetScheduleRecurrentDesc(config);
            return new ScheduleEvent()
            {
                ExecutionDate = config.ScheduleDate.Value,
                ExecutionDescription = Description
            };

        }

        private static DateTime GetCurrentDate(SchedulerConfigurator config)
        {
            DateTime startDate = config.ScheduleDate ?? config.CurrentDate.Value;
            return startDate.CurrentDateOrStartLimit(config.DateLimits?.StartLimit);
        }

        private static DateTime GetNextExecutionDaily(SchedulerConfigurator config, DateTime nextExec)
        {
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) >= 0)
            {
                nextExec = nextExec.AddDays(config.OcurrencyPeriod.Value).Date;
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        private static DateTime GetNextExecutionWeeklyStandard(SchedulerConfigurator config, DateTime nextExec)
        {
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) >= 0)
            {
                nextExec = nextExec.AddWeeks(config.OcurrencyPeriod.Value).Date;
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        private static DateTime GetNextExecutionWeekly(SchedulerConfigurator config, DateTime nextExec)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            nextExec = NextSelectedDay(config, nextExec, culture, false);
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) >= 0)
            {
                nextExec = NextSelectedDay(config, nextExec, culture, true);
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        private static DateTime NextSelectedDay(SchedulerConfigurator config, DateTime currentDate, CultureInfo culture, bool escapeCurrent)
        {
            if (escapeCurrent)
            {
                currentDate = NextDayToCompare(config, currentDate, culture);
            }
            while (config.WeeklyDays.Contains(currentDate.DayOfWeek) == false)
            {
                currentDate = NextDayToCompare(config, currentDate, culture);
            }
            return currentDate;
        }

        private static DateTime NextDayToCompare(SchedulerConfigurator config, DateTime currentDate, CultureInfo culture)
        {
            int currentWeek = currentDate.GetWeekOfYear(culture);
            DateTime nextDay = currentDate.NextDay();
            if (nextDay.GetWeekOfYear(culture) == currentWeek)
            {
                return nextDay;
            }
            else
            {
                return GetFirstSelectedDayOfWeek(config, currentDate, culture);
            }

        }

        private static DateTime GetFirstSelectedDayOfWeek(SchedulerConfigurator config, DateTime currentDate, CultureInfo culture)
        {
            DateTime nextDay = currentDate.FirstDayOfNextWeek(config.OcurrencyPeriod.Value, culture);
            while (config.WeeklyDays.Contains(nextDay.DayOfWeek) == false)
            {
                nextDay = nextDay.AddDays(1);
            }
            return nextDay;
        }

        private static DateTime GetNextExecutionMonthly(SchedulerConfigurator config, DateTime nextExec)
        {
            ScheduleConfigValidator.ValidateMonthlyConfiguration(config);
            if (config.MonthlyDaySelection.Value == true)
            {
                return GetNextExecutionMonthlyExactDay(config, nextExec);
            }
            else
            {
                return GetNextExecutionMonthlyFrecuency(config, nextExec);
            }




            //DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            //while (DateTime.Compare(nextExec, newDate) >= 0)
            //{
            //    nextExec = nextExec.AddMonths(config.OcurrencyPeriod.Value).Date;
            //    newDate = CalculateDailyConfigHour(config, nextExec);
            //}
            //return newDate;
        }

        private static DateTime GetNextExecutionMonthlyExactDay(SchedulerConfigurator config, DateTime nextExec)
        {
            nextExec = NextExactDayOfMonth(config, nextExec);
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) >= 0)
            {
                nextExec = nextExec.ExactDayOfMonth(config.MonthlyDay.Value, config.OcurrencyPeriod);
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        private static DateTime NextExactDayOfMonth(SchedulerConfigurator config, DateTime currentDate)
        {
            if (currentDate.Day < config.MonthlyDay.Value.DayOrLastDayOfMonth(currentDate.Month, currentDate.Year))
            {
                currentDate = config.MonthlyDay.Value.DayOfMonthOrLastDay(currentDate.Month, currentDate.Year);
            }
            else if (currentDate.Day > config.MonthlyDay)
            {
                currentDate = currentDate.ExactDayOfMonth(config.MonthlyDay.Value, config.OcurrencyPeriod);
            }
            return currentDate;
        }

        private static DateTime GetNextExecutionMonthlyFrecuency(SchedulerConfigurator config, DateTime nextExec)
        {
            return nextExec; //TODO
        }



        private static DateTime GetNextExecutionYearly(SchedulerConfigurator config, DateTime nextExec)
        {
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) >= 0)
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
                return execDate.CalculateSameDayWithHours(config.DailyScheduleHour.Value);
            }
            return CalculateDailyConfigHourReccurent(config, execDate);
        }

        private static DateTime CalculateDailyConfigHourReccurent(SchedulerConfigurator config, DateTime execDate)
        {
            DateTime NewDate;
            DateTime StartLimit = execDate.StartDailyLimit(config.DailyLimits?.StartLimit);
            DateTime EndLimit = execDate.EndDailyLimit(config.DailyLimits?.EndLimit);
            NewDate = StartLimit;

            while (DateTime.Compare(execDate, NewDate) >= 0)
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
    }
}

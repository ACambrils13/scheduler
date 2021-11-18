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
            DateTime NextExecutionDate = config.CurrentDate.Value.CurrentDateOrStartLimit(config.DateLimits?.StartLimit);

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

        private static DateTime GetNextExecutionWeeklyStandard(SchedulerConfigurator config, DateTime nextExec)
        {
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) > 0)
            {
                nextExec = nextExec.AddWeeks(config.OcurrencyPeriod.Value).Date;
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        private static DateTime GetNextExecutionWeekly(SchedulerConfigurator config, DateTime nextExec)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            nextExec = NextSelectedDay(config, nextExec, culture);
            DateTime newDate = CalculateDailyConfigHour(config, nextExec);
            while (DateTime.Compare(nextExec, newDate) > 0)
            {
                nextExec = NextSelectedDay(config, nextExec.NextDay(), culture);
                newDate = CalculateDailyConfigHour(config, nextExec);
            }
            return newDate;
        }

        private static DateTime NextSelectedDay(SchedulerConfigurator config, DateTime currentDate, CultureInfo culture)
        {
            int currentWeek = currentDate.GetWeekOfYear(culture);
            while (config.WeeklyDays.Contains(currentDate.DayOfWeek) == false)
            {
                DateTime nextDay = currentDate.NextDay();
                if (nextDay.GetWeekOfYear(culture) == currentWeek)
                {
                    currentDate = nextDay;
                }
                else
                {
                    currentDate = GetFirstSelectedDayOfWeek(config, currentDate, culture);
                    currentWeek = currentDate.GetWeekOfYear(culture);
                }
            }
            return currentDate;
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
    }
}

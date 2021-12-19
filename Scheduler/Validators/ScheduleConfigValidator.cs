using Scheduler.Auxiliary;
using Scheduler.Configuration;
using Scheduler.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace Scheduler.Validators
{
    internal class ScheduleConfigValidator
    {
        internal static void ValidateBasicProperties(SchedulerConfigurator properties)
        {
            ValidateDateNullable(properties.CurrentDate, nameof(properties.CurrentDate));
            ValidateEnum<ScheduleTypeEnum>(properties.Type, nameof(properties.Type));
        }

        internal static void ValidateOnceSchedule(SchedulerConfigurator properties)
        {
            ValidateDateNullable(properties.ScheduleDate, nameof(properties.ScheduleDate));
            ValidateLimits(properties.ScheduleDate.Value, properties.DateLimits, true);
        }

        internal static void ValidateRecurringSchedule(SchedulerConfigurator properties)
        {
            ValidateLimits(properties.CurrentDate.Value, properties.DateLimits, false);
            ValidateEnum<OccurrencyPeriodEnum>(properties.PeriodType, nameof(properties.PeriodType));
            ValidatePeriod(properties.OcurrencyPeriod, nameof(properties.OcurrencyPeriod));
            ValidateDailySelection(properties);
        }

        internal static void ValidateLimits(DateTime date, DateLimitsConfig? limits, bool validateBefore)
        {
            if (limits.HasValue)
            {
                if (limits.Value.EndLimit.HasValue)
                {
                    if (DateTime.Compare(date, limits.Value.EndLimit.Value) > 0)
                    {
                        throw new ValidationException(FormatConfigExcMessage(TextResources.ExcLimits));
                    }
                }
                if (validateBefore && limits.Value.StartLimit.HasValue)
                {
                    if (DateTime.Compare(date, limits.Value.StartLimit.Value) < 0)
                    {
                        throw new ValidationException(FormatConfigExcMessage(TextResources.ExcLimits));
                    }
                }
            }
        }

        private static void ValidateDailySelection(SchedulerConfigurator config)
        {
            if (config.DailyScheduleHour.HasValue)
            {
                ValidateHourOfDay(config.DailyScheduleHour.Value, nameof(config.DailyScheduleHour));
            }
            else if (config.DailyFrecuency.HasValue && config.DailyFrecuencyPeriod.HasValue)
            {
                ValidateEnum<DailyFrecuencyEnum>(config.DailyFrecuency, nameof(config.DailyFrecuency));
                ValidatePeriod(config.DailyFrecuencyPeriod, nameof(config.DailyFrecuencyPeriod));
            }
            else
            {
                throw new ValidationException(FormatConfigExcMessage(TextResources.ExcDailyConfig));
            }
        }

        internal static void ValidateDateNullable(DateTime? date, string propertyName)
        {
            if (date.HasValue == false)
            {
                throw new ValidationException(FormatConfigExcMessage(string.Format(TextResources.ExcObjectNull, propertyName)));
            }
            ValidateDate(date.Value, propertyName);
        }

        internal static void ValidateDate(DateTime date, string propertyName)
        {

            if (date == DateTime.MaxValue)
            {
                throw new ValidationException(FormatConfigExcMessage(string.Format(TextResources.ExcDateMaxValue, propertyName)));
            }
        }
        internal static void ValidateHourOfDay(TimeSpan hour, string propertyName)
        {
            TimeSpan Zero = new(0, 0, 0);
            TimeSpan MidNigth = new(23, 59, 59);
            if (hour < Zero || hour > MidNigth)
            {
                throw new ValidationException(FormatConfigExcMessage(string.Format(TextResources.ExcHoursValue, propertyName)));
            }
        }

        internal static void ValidateEnum<TEnum>(Enum enumValue, string propertyName)
        {
            if (enumValue == null || Enum.IsDefined(typeof(TEnum), enumValue) == false)
            {
                throw new ValidationException(FormatConfigExcMessage(string.Format(TextResources.ExcEnumError, propertyName)));
            }
        }

        internal static void ValidatePeriod(int? period, string propertyName)
        {
            if (period.HasValue == false || period.Value < 0)
            {
                throw new ValidationException(FormatConfigExcMessage(string.Format(TextResources.ExcPeriod, propertyName)));
            }
        }

        internal static void ValidateDateLimits(DateTime? start, DateTime? end)
        {
            if (start.HasValue == false && end.HasValue)
            {
                throw new ValidationException(FormatConfigExcMessage(TextResources.ExcLimitsEndBeforeStart));
            }
            else if (start.HasValue)
            {
                ValidateDate(start.Value, nameof(start));
                if (end.HasValue)
                {
                    ValidateDate(end.Value, nameof(end));
                    if (DateTime.Compare(start.Value, end.Value) >= 0)
                    {
                        throw new ValidationException(FormatConfigExcMessage(TextResources.ExcLimitsEndBeforeStart));
                    }
                }
            }
        }

        internal static void ValidateHourLimits(TimeSpan? start, TimeSpan? end)
        {
            if (start.HasValue == false && end.HasValue)
            {
                throw new ValidationException(FormatConfigExcMessage(TextResources.ExcLimitsEndBeforeStart));
            }
            else if (start.HasValue)
            {
                ValidateHourOfDay(start.Value, nameof(start));
                if (end.HasValue)
                {
                    ValidateHourOfDay(end.Value, nameof(end));
                    if (TimeSpan.Compare(start.Value, end.Value) >= 0)
                    {
                        throw new ValidationException(FormatConfigExcMessage(TextResources.ExcLimitsEndBeforeStart));
                    }
                }
            }
        }

        internal static void ValidateMonthlyConfiguration(SchedulerConfigurator config)
        {
            if (config.MonthlyDaySelection.HasValue == false)
            {
                throw new ValidationException(FormatConfigExcMessage(TextResources.ExcMonthlyTypeConfig));
            }
            if (config.MonthlyDaySelection.Value == true)
            {
                ValidateDayOfMonth(config.MonthlyDay);
            }
            else
            {
                ValidateMonthlyPeriod(config);
            }
        }

        internal static void ValidateDayOfMonth(int? day)
        {
            if (day.HasValue == false || day.Value < 1 || day.Value > 31)
            {
                throw new ValidationException(FormatConfigExcMessage(TextResources.ExcMonthlyDay));
            }
        }

        internal static void ValidateMonthlyPeriod(SchedulerConfigurator config)
        {
            ValidateEnum<MonthlyFrecuencyEnum>(config.MonthlyFrecuency, nameof(config.MonthlyFrecuency));
            ValidateEnum<MonthlyDayEnum>(config.MonthlyWeekday, nameof(config.MonthlyWeekday));
        }

        internal static string FormatConfigExcMessage(string exc)
        {
            return string.Format(TextResources.ConfError, exc);
        }
    }
}

using Scheduler.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace Scheduler
{
    internal class ScheduleConfigValidator
    {
        internal static void ValidateBasicProperties(Scheduler properties)
        {
            ValidateDateNullable(properties.CurrentDate, nameof(properties.CurrentDate));
            ValidateEnum<ScheduleTypeEnum>(properties.Type, nameof(properties.Type));
        }

        internal static void ValidateOnceSchedule(Scheduler properties)
        {
            ValidateDateNullable(properties.ScheduleDate, nameof(properties.ScheduleDate));
            ValidateLimits(properties.ScheduleDate.Value, properties.DateLimits);
        }

        internal static void ValidateRecurringSchedule(Scheduler properties)
        {
            ValidateEnum<OccurrencyPeriodEnum>(properties.PeriodType, nameof(properties.PeriodType));
            ValidatePeriod(properties.OcurrencyPeriod, nameof(properties.OcurrencyPeriod));
            ValidateDailySelection(properties.DailyScheduleHour, properties.DailyFrecuency, properties.DailyFrecuencyPeriod);
        }

        internal static void ValidateLimits(DateTime scheduleDate, LimitsConfig? limits)
        {
            if (limits.HasValue)
            {
                if (limits.Value.StartLimit.HasValue)
                {
                    if (DateTime.Compare(scheduleDate, limits.Value.StartLimit.Value) < 0)
                    {
                        throw new ValidationException(FormatConfigExcMessage(TextResources.ExcLimits));
                    }
                }
                if (limits.Value.EndLimit.HasValue)
                {
                    if (DateTime.Compare(scheduleDate, limits.Value.EndLimit.Value) > 0)
                    {
                        throw new ValidationException(FormatConfigExcMessage(TextResources.ExcLimits));
                    }
                }
            }
        }

        private static void ValidateDailySelection(DateTime? dailyScheduleHour, DailyFrecuencyEnum? dailyFrecuency, int? dailyFrecuencyPeriod)
        {
            if (dailyScheduleHour.HasValue)
            {
                ValidateDate(dailyScheduleHour.Value, nameof(dailyScheduleHour));
            }
            else if (dailyFrecuency.HasValue && dailyFrecuencyPeriod.HasValue)
            {
                ValidateEnum<DailyFrecuencyEnum>(dailyFrecuency, nameof(dailyFrecuency));
                ValidatePeriod(dailyFrecuencyPeriod, nameof(dailyFrecuencyPeriod));
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

        internal static void ValidateDate (DateTime date, string propertyName)
        {

            if (date == DateTime.MaxValue)
            {
                throw new ValidationException(FormatConfigExcMessage(string.Format(TextResources.ExcDateMaxValue, propertyName)));
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

        internal static void ValidateLimits(DateTime? start, DateTime? end)
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

        internal static string FormatConfigExcMessage(string exc)
        {
            return string.Format(TextResources.ConfError, exc);
        }
    }
}

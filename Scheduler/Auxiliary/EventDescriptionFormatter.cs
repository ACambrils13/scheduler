using Scheduler.Configuration;
using Scheduler.Resources;
using System;
using System.Text;

namespace Scheduler.Auxiliary
{
    internal class EventDescriptionFormatter
    {
        internal static string GetScheduleOnceDesc(DateTime scheduleDate, DateLimitsConfig? dateLimits)
        {
            string date = scheduleDate.ToShortDateString();
            string hour = scheduleDate.ToString("HH:mm");

            StringBuilder description = new(TextResources.EventDescOnce);
            description.Append(string.Concat(" ", string.Format(TextResources.EventDescSchedule, date, hour)));
            description.Append(AddLimitsDesc(dateLimits));
            return description.ToString();
        }

        internal static string GetScheduleRecurrentDesc(SchedulerConfigurator config)
        {
            string periodString = GetTypeName(config.PeriodType.Value);
            StringBuilder description = new(string.Format(TextResources.EventDescRecurring, config.OcurrencyPeriod, periodString));
            description.Append(GetWeeklyDesc(config));
            description.Append(GetDailyDesc(config));
            description.Append(AddLimitsDesc(config.DateLimits));
            return description.ToString();
        }

        private static string GetDailyDesc(SchedulerConfigurator config)
        {
            StringBuilder dailyDesc = new();
            if (config.DailyScheduleHour.HasValue)
            {
                dailyDesc.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringHour, config.DailyScheduleHour.Value.ToString(@"hh\:mm"))));
            }
            else if (config.DailyLimits.HasValue)
            {
                switch (config.DailyFrecuency)
                {
                    case DailyFrecuencyEnum.Hours:
                        dailyDesc.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, config.DailyFrecuencyPeriod, TextResources.Hours)));
                        break;
                    case DailyFrecuencyEnum.Minutes:
                        dailyDesc.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, config.DailyFrecuencyPeriod, TextResources.Minutes)));
                        break;
                    case DailyFrecuencyEnum.Seconds:
                        dailyDesc.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, config.DailyFrecuencyPeriod, TextResources.Seconds)));
                        break;
                }
                if (config.DailyLimits.HasValue)
                {
                    string StartLimit = config.DailyLimits.Value.StartLimit?.ToString(@"hh\:mm") ?? "0:00";
                    string EndLimit = config.DailyLimits.Value.EndLimit?.ToString(@"hh\:mm") ?? "23:59";
                    dailyDesc.Append(String.Concat(" ", string.Format(TextResources.EventDescDailyLimits, StartLimit, EndLimit)));
                }
            }
            return dailyDesc.ToString();
        }

        private static string GetWeeklyDesc(SchedulerConfigurator config)
        {
            string weeklyDesc = string.Empty;
            if (config.PeriodType.Value == OccurrencyPeriodEnum.Weekly && config.WeeklyDays != null && config.WeeklyDays.Count > 0)
            {
                string WeeklyDays = string.Join(", ", config.WeeklyDays);
                WeeklyDays = WeeklyDays.ChangeLastPeriodToAnd();
                weeklyDesc = string.Concat(" ", string.Format(TextResources.EventDescRecurringWeekly, WeeklyDays));
            }
            return weeklyDesc;
        }

        private static string AddLimitsDesc(DateLimitsConfig? dateLimits)
        {
            StringBuilder limitsDesc = new();
            if (dateLimits.HasValue)
            {
                if (dateLimits.Value.StartLimit.HasValue)
                {
                    string StartDate = dateLimits.Value.StartLimit.Value.ToShortDateString();
                    limitsDesc.Append(string.Concat(" ", string.Format(TextResources.EventDescLimitsStart, StartDate)));
                }
                if (dateLimits.Value.EndLimit.HasValue)
                {
                    string EndDate = dateLimits.Value.EndLimit.Value.ToShortDateString();
                    limitsDesc.Append(string.Concat(" ", string.Format(TextResources.EventDescLimitsEnd, EndDate)));
                }
            }
            return limitsDesc.ToString();
        }

        private static string GetTypeName(OccurrencyPeriodEnum type)
        {
            string typeString = string.Empty;
            switch (type)
            {
                case OccurrencyPeriodEnum.Daily:
                    typeString = TextResources.Days;
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    typeString = TextResources.Weeks;
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    typeString = TextResources.Months;
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    typeString = TextResources.Years;
                    break;
            }
            return typeString;
        }

    }
}

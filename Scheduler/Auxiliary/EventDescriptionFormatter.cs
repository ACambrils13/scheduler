using Scheduler.Configuration;
using Scheduler.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.Auxiliary
{
    internal class EventDescriptionFormatter
    {
        internal static string GetScheduleOnceDesc(DateTime scheduleDate, DateLimitsConfig? dateLimits)
        {
            string date = scheduleDate.ToShortDateString();
            string hour = scheduleDate.ToString("HH:mm");

            StringBuilder description = new(LanguageManager.GetStringResource("EventDescOnce"));
            description.Append(string.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescSchedule"), date, hour)));
            description.Append(AddLimitsDesc(dateLimits));
            return description.ToString();
        }

        internal static string GetScheduleRecurrentDesc(SchedulerConfigurator config)
        {
            string periodString = GetTypeName(config.PeriodType.Value);
            StringBuilder description = new(LanguageManager.GetStringResource("EventDescRecurringStart"));
            description.Append(GetMonthlyDesc(config));
            description.Append(string.Format(LanguageManager.GetStringResource("EventDescRecurringEvery"), config.OcurrencyPeriod, periodString));
            description.Append(GetWeeklyDesc(config));
            description.Append(GetDailyDesc(config));
            description.Append(AddLimitsDesc(config.DateLimits));
            return description.ToString();
        }

        private static string GetMonthlyDesc(SchedulerConfigurator config)
        {
            StringBuilder monthlyDesc = new();
            if (config.PeriodType == OccurrencyPeriodEnum.Monthly)
            {
                if (config.MonthlyDaySelection == true)
                {
                    monthlyDesc.Append(string.Format(LanguageManager.GetStringResource("EventDescDayOfMonth"), config.MonthlyDay));
                }
                else
                {
                    monthlyDesc.Append(string.Format(LanguageManager.GetStringResource("EventDescMonthFrecuency"), LanguageManager.GetStringResource(config.MonthlyFrecuency.Value.ToString()), LanguageManager.GetStringResource(config.MonthlyWeekday.Value.ToString())));
                }
            }
            return monthlyDesc.ToString();
        }

        private static string GetDailyDesc(SchedulerConfigurator config)
        {
            StringBuilder dailyDesc = new();
            if (config.DailyScheduleHour.HasValue)
            {
                dailyDesc.Append(string.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescRecurringHour"), config.DailyScheduleHour.Value.ToString(@"hh\:mm"))));
            }
            else if (config.DailyFrecuency.HasValue)
            {
                switch (config.DailyFrecuency)
                {
                    case DailyFrecuencyEnum.Hours:
                        dailyDesc.Append(string.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescRecurringEvery"), config.DailyFrecuencyPeriod, LanguageManager.GetStringResource("Hours"))));
                        break;
                    case DailyFrecuencyEnum.Minutes:
                        dailyDesc.Append(string.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescRecurringEvery"), config.DailyFrecuencyPeriod, LanguageManager.GetStringResource("Minutes"))));
                        break;
                    case DailyFrecuencyEnum.Seconds:
                        dailyDesc.Append(string.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescRecurringEvery"), config.DailyFrecuencyPeriod, LanguageManager.GetStringResource("Seconds"))));
                        break;
                }
                if (config.DailyLimits.HasValue)
                {
                    string StartLimit = config.DailyLimits.Value.StartLimit?.ToString(@"hh\:mm") ?? "0:00";
                    string EndLimit = config.DailyLimits.Value.EndLimit?.ToString(@"hh\:mm") ?? "23:59";
                    dailyDesc.Append(String.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescDailyLimits"), StartLimit, EndLimit)));
                }
            }
            return dailyDesc.ToString();
        }

        private static string GetWeeklyDesc(SchedulerConfigurator config)
        {
            string weeklyDesc = string.Empty;
            if (config.PeriodType.Value == OccurrencyPeriodEnum.Weekly && config.WeeklyDays != null && config.WeeklyDays.Count > 0)
            {
                List<string> weeklyDaysLocalized = LanguageManager.GetStringResourcesList(config.WeeklyDays);
                string WeeklyDays = string.Join(", ", weeklyDaysLocalized);
                WeeklyDays = WeeklyDays.ChangeLastPeriodToAnd();
                weeklyDesc = string.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescRecurringWeekly"), WeeklyDays));
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
                    limitsDesc.Append(string.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescLimitsStart"), StartDate)));
                }
                if (dateLimits.Value.EndLimit.HasValue)
                {
                    string EndDate = dateLimits.Value.EndLimit.Value.ToShortDateString();
                    limitsDesc.Append(string.Concat(" ", string.Format(LanguageManager.GetStringResource("EventDescLimitsEnd"), EndDate)));
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
                    typeString = LanguageManager.GetStringResource("Days");
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    typeString = LanguageManager.GetStringResource("Weeks");
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    typeString = LanguageManager.GetStringResource("Months");
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    typeString = LanguageManager.GetStringResource("Years");
                    break;
            }
            return typeString;
        }

    }
}

using Scheduler.Configuration;
using Scheduler.Resources;
using System;
using System.Text;

namespace Scheduler.Auxiliary
{
    internal class EventDescriptionFormatter
    {
        internal static string GetScheduleOnceDesc(DateTime scheduleDate, LimitsConfig? dateLimits)
        {
            string Date = scheduleDate.ToShortDateString();
            string Hour = scheduleDate.ToString("HH:mm");

            StringBuilder Description = new(TextResources.EventDescOnce);
            Description.Append(string.Concat(" ", string.Format(TextResources.EventDescSchedule, Date, Hour)));
            if (dateLimits.HasValue)
            {
                if (dateLimits.Value.StartLimit.HasValue)
                {
                    string StartDate = dateLimits.Value.StartLimit.Value.ToShortDateString();
                    Description.Append(string.Concat(" ", string.Format(TextResources.EventDescLimitsStart, StartDate)));
                }
                if (dateLimits.Value.EndLimit.HasValue)
                {
                    string EndDate = dateLimits.Value.EndLimit.Value.ToShortDateString();
                    Description.Append(string.Concat(" ", string.Format(TextResources.EventDescLimitsEnd, EndDate)));
                }
            }
            return Description.ToString();
        }

        internal static string GetScheduleRecurrentDesc(SchedulerConfigurator config)
        {
            string PeriodString = string.Empty;
            switch (config.PeriodType)
            {
                case OccurrencyPeriodEnum.Daily:
                    PeriodString = TextResources.Days;
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    PeriodString = TextResources.Weeks;
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    PeriodString = TextResources.Months;
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    PeriodString = TextResources.Years;
                    break;
            }

            StringBuilder Description = new(string.Format(TextResources.EventDescRecurring, config.OcurrencyPeriod, PeriodString));
            if (config.PeriodType.Value == OccurrencyPeriodEnum.Weekly && config.WeeklyDays != null && config.WeeklyDays.Length > 0)
            {
                string WeeklyDays = string.Join(", ", config.WeeklyDays);
                if (WeeklyDays.LastIndexOf(",") >= 0)
                {
                    int Place = WeeklyDays.LastIndexOf(",");
                    WeeklyDays = WeeklyDays.Remove(Place, 1).Insert(Place, string.Concat(" ", TextResources.And));
                }
                Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringWeekly, WeeklyDays)));
            }
            if (config.DailyScheduleHour.HasValue)
            {
                Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringHour, config.DailyScheduleHour.Value.ToShortTimeString())));
            }
            else if (config.DailyLimits.HasValue)
            {
                switch (config.DailyFrecuency)
                {
                    case DailyFrecuencyEnum.Hours:
                        Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, config.DailyFrecuencyPeriod, TextResources.Hours)));
                        break;
                    case DailyFrecuencyEnum.Minutes:
                        Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, config.DailyFrecuencyPeriod, TextResources.Minutes)));
                        break;
                    case DailyFrecuencyEnum.Seconds:
                        Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, config.DailyFrecuencyPeriod, TextResources.Seconds)));
                        break;
                }
                if (config.DailyLimits.HasValue)
                {
                    string StartLimit = config.DailyLimits.Value.StartLimit?.ToShortTimeString() ?? "0:00";
                    string EndLimit = config.DailyLimits.Value.EndLimit?.ToShortTimeString() ?? "23:59";
                    Description.Append(String.Concat(" ", string.Format(TextResources.EventDescDailyLimits, StartLimit, EndLimit)));
                }
            }
            if (config.DateLimits.HasValue)
            {
                if (config.DateLimits.Value.StartLimit.HasValue)
                {
                    string StartDate = config.DateLimits.Value.StartLimit.Value.ToShortDateString();
                    Description.Append(string.Concat(" ", string.Format(TextResources.EventDescLimitsStart, StartDate)));
                }
                if (config.DateLimits.Value.EndLimit.HasValue)
                {
                    string EndDate = config.DateLimits.Value.EndLimit.Value.ToShortDateString();
                    Description.Append(string.Concat(" ", string.Format(TextResources.EventDescLimitsEnd, EndDate)));
                }
            }
            return Description.ToString();
        }
    }
}

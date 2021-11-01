using Scheduler.Resources;
using System;
using System.Text;

namespace Scheduler
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

        internal static string GetScheduleRecurrentDesc(Scheduler configuration)
        {
            string PeriodString = string.Empty;
            switch (configuration.PeriodType)
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

            StringBuilder Description = new(string.Format(TextResources.EventDescRecurring, configuration.OcurrencyPeriod, PeriodString));
            if (configuration.PeriodType.Value == OccurrencyPeriodEnum.Weekly && configuration.WeeklyDays != null && configuration.WeeklyDays.Length > 0)
            {
                string WeeklyDays = string.Join(", ", configuration.WeeklyDays);
                if (WeeklyDays.LastIndexOf(",") >= 0)
                {
                    int Place = WeeklyDays.LastIndexOf(",");
                    WeeklyDays = WeeklyDays.Remove(Place, 1).Insert(Place, string.Concat(" ", TextResources.And));
                }
                Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringWeekly, WeeklyDays)));
            }
            if (configuration.DailyScheduleHour.HasValue)
            {
                Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringHour, configuration.DailyScheduleHour.Value.ToShortTimeString())));
            }
            else if (configuration.DailyLimits.HasValue)
            {
                switch (configuration.DailyFrecuency)
                {
                    case DailyFrecuencyEnum.Hours:
                        Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, configuration.DailyFrecuencyPeriod, TextResources.Hours)));
                        break;
                    case DailyFrecuencyEnum.Minutes:
                        Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, configuration.DailyFrecuencyPeriod, TextResources.Minutes)));
                        break;
                    case DailyFrecuencyEnum.Seconds:
                        Description.Append(string.Concat(" ", string.Format(TextResources.EventDescRecurringEvery, configuration.DailyFrecuencyPeriod, TextResources.Seconds)));
                        break;
                }
                if (configuration.DailyLimits.HasValue)
                {
                    string StartLimit = configuration.DailyLimits.Value.StartLimit?.ToShortTimeString() ?? "0:00";
                    string EndLimit = configuration.DailyLimits.Value.EndLimit?.ToShortTimeString() ?? "23:59";
                    Description.Append(String.Concat(" ", string.Format(TextResources.EventDescDailyLimits, StartLimit, EndLimit)));
                }
            }
            if (configuration.DateLimits.HasValue)
            {
                if (configuration.DateLimits.Value.StartLimit.HasValue)
                {
                    string StartDate = configuration.DateLimits.Value.StartLimit.Value.ToShortDateString();
                    Description.Append(string.Concat(" ", string.Format(TextResources.EventDescLimitsStart, StartDate)));
                }
                if (configuration.DateLimits.Value.EndLimit.HasValue)
                {
                    string EndDate = configuration.DateLimits.Value.EndLimit.Value.ToShortDateString();
                    Description.Append(string.Concat(" ", string.Format(TextResources.EventDescLimitsEnd, EndDate)));
                }
            }
            return Description.ToString();
        }
    }
}

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

            StringBuilder Description = new (TextResources.EventDescOnce);
            Description.Append(string.Concat(" ",string.Format(TextResources.EventDescSchedule, Date, Hour)));
            if (dateLimits.HasValue)
            {
                if (dateLimits.Value.StartLimit.HasValue)
                {
                    string StartDate = dateLimits.Value.StartLimit.Value.ToShortDateString();
                    Description.Append(string.Concat(" ",string.Format(TextResources.EventDescLimitsStart, StartDate)));
                }
                if (dateLimits.Value.EndLimit.HasValue)
                {
                    string EndDate = dateLimits.Value.EndLimit.Value.ToShortDateString();
                    Description.Append(string.Concat(" ",string.Format(TextResources.EventDescLimitsEnd, EndDate)));
                }
            }
            return Description.ToString();
        }

        internal static string GetScheduleRecurrentDesc(Scheduler configuration)
        {
            StringBuilder Description = new(string.Format(TextResources.EventDescRecurring,configuration.OcurrencyPeriod.ToString(),configuration.PeriodType.ToString()));
            if (configuration.PeriodType.Value == OccurrencyPeriodEnum.Weekly)
            {
                // poner los días
            }
            if (configuration.DailyLimits.HasValue)
            {
                string StartLimit = configuration.DailyLimits.Value.StartLimit?.ToShortTimeString() ?? "0:00";
                string EndLimit = configuration.DailyLimits.Value.EndLimit?.ToShortTimeString() ?? "23:59";
                Description.Append(String.Concat(" ", string.Format(TextResources.EventDescDailyLimits, StartLimit, EndLimit)));
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

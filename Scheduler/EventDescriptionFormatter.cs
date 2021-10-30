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

            StringBuilder Description = new StringBuilder(TextResources.EventDescOnce);
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
    }
}

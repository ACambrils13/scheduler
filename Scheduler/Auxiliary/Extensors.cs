using Scheduler.Configuration;
using System;

namespace Scheduler.Auxiliary
{
    public static class Extensors
    {
        public static DateTime CalculateSameDayWithHours(this DateTime currentDate, DateTime hours)
        {
            return currentDate.Date.Add(hours.TimeOfDay);
        }

        public static DateTime AddWeeks(this DateTime currentDate, int weeks)
        {
            return currentDate.AddDays(weeks * 7).Date;
        }

        public static DateTime CurrentDateOrStartLimit(this DateTime currentDate, DateTime? startLimit)
        {
            if (startLimit.HasValue && DateTime.Compare(currentDate, startLimit.Value) < 0)
            {
                return startLimit.Value;
            }
            else
            {
                return currentDate;
            }
        }

        public static DateTime StartDailyLimit (this DateTime currentDate, DateTime? StartDailyLimit)
        {
            if (StartDailyLimit.HasValue)
            {
                return currentDate.CalculateSameDayWithHours(StartDailyLimit.Value);
            }
            return currentDate.Date;
        }

        public static DateTime EndDailyLimit(this DateTime currentDate, DateTime? EndDailyLimit)
        {
            if (EndDailyLimit.HasValue)
            {
                return currentDate.CalculateSameDayWithHours(EndDailyLimit.Value);
            }
            return currentDate.Date.AddDays(1);
        }
    }
}

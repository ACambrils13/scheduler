using Scheduler.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Scheduler.Auxiliary
{
    public static class Extensors
    {
        public static DateTime CalculateSameDayWithHours(this DateTime currentDate, TimeSpan hours)
        {
            return currentDate.Date.Add(hours);
        }

        public static DateTime AddWeeks(this DateTime currentDate, int weeks)
        {
            return currentDate.AddDays(weeks * 7).Date;
        }

        public static DateTime NextDay(this DateTime currentDate)
        {
            return currentDate.AddDays(1).Date;
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

        public static DateTime StartDailyLimit(this DateTime currentDate, TimeSpan? startDailyLimit)
        {
            if (startDailyLimit.HasValue)
            {
                return currentDate.CalculateSameDayWithHours(startDailyLimit.Value);
            }
            return currentDate.Date;
        }

        public static DateTime? EndDailyLimit(this DateTime currentDate, TimeSpan? endDailyLimit)
        {
            if (endDailyLimit.HasValue)
            {
                return currentDate.CalculateSameDayWithHours(endDailyLimit.Value);
            }
            return null;
        }

        public static bool CurrentDateAfterNewDate(this DateTime currentDate, DateTime newDate, DateTime? previousDate)
        {
            if (DateTime.Compare(currentDate, newDate) > 0
                || (DateTime.Compare(currentDate, newDate) == 0 && previousDate.HasValue
                && DateTime.Compare(currentDate, previousDate.Value) == 0))
            {
                return true;
            }
            return false;
        }

        public static int GetWeekOfYear(this DateTime currentDate, CultureInfo culture)
        {
            Calendar calendar = culture.Calendar;
            DayOfWeek firstDayofWeek = culture.DateTimeFormat.FirstDayOfWeek;
            CalendarWeekRule weekRule = culture.DateTimeFormat.CalendarWeekRule;

            return calendar.GetWeekOfYear(currentDate, weekRule, firstDayofWeek);
        }

        public static DateTime FirstDayOfNextWeek(this DateTime currentDate, int numWeeks, CultureInfo culture)
        {
            return currentDate.AddWeeks(numWeeks).FirstDayOfSameWeek(culture);
        }

        public static DateTime FirstDayOfSameWeek(this DateTime currentDate, CultureInfo culture)
        {
            DayOfWeek firstDay = culture.DateTimeFormat.FirstDayOfWeek;
            int week = currentDate.GetWeekOfYear(culture);
            while (currentDate.DayOfWeek != firstDay && currentDate.GetWeekOfYear(culture) == week)
            {
                currentDate = currentDate.AddDays(-1);
            }
            return currentDate;
        }

        public static string ChangeLastPeriodToAnd(this string joinedText)
        {
            int lastPeriod = joinedText.LastIndexOf(",");
            if (lastPeriod >= 0)
            {
                joinedText = joinedText.Remove(lastPeriod, 1).Insert(lastPeriod, string.Concat(" ", LanguageManager.GetStringResource("And")));
            }
            return joinedText;
        }

        public static DateTime ExactDayOfMonth(this DateTime date, int day, int? addMonths)
        {
            if (addMonths.HasValue)
            {
                date = date.AddMonths(addMonths.Value);
            }
            return day.DayOfMonthOrLastDay(date.Month, date.Year);
        }

        public static DateTime DayOfMonthOrLastDay(this int day, int month, int year)
        {
            day = day.DayOrLastDayOfMonth(month, year);
            return new DateTime(year, month, day);
        }

        public static int DayOrLastDayOfMonth(this int day, int month, int year)
        {
            int lastDayOfMonth = DateTime.DaysInMonth(year, month);
            day = day <= lastDayOfMonth ? day : lastDayOfMonth;
            return day;
        }

        public static DateTime FirstDayOfSameMonth(this DateTime date, int? addMonths)
        {
            if (addMonths.HasValue)
            {
                date = date.AddMonths(addMonths.Value);
            }
            return new DateTime(date.Year, date.Month, 1);
        }

        public static bool DateIsValid(this DateTime date, MonthlyDayEnum type)
        {
            List<DayOfWeek> validDaysOfWeek = type switch
            {
                MonthlyDayEnum.Monday => new List<DayOfWeek> { DayOfWeek.Monday },
                MonthlyDayEnum.Tuesday => new List<DayOfWeek> { DayOfWeek.Tuesday },
                MonthlyDayEnum.Wednesday => new List<DayOfWeek> { DayOfWeek.Wednesday },
                MonthlyDayEnum.Thursday => new List<DayOfWeek> { DayOfWeek.Thursday },
                MonthlyDayEnum.Friday => new List<DayOfWeek> { DayOfWeek.Friday },
                MonthlyDayEnum.Saturday => new List<DayOfWeek> { DayOfWeek.Saturday },
                MonthlyDayEnum.Sunday => new List<DayOfWeek> { DayOfWeek.Sunday },
                MonthlyDayEnum.Day => new List<DayOfWeek> {
                        DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday,
                        DayOfWeek.Saturday,
                        DayOfWeek.Sunday
                        },
                MonthlyDayEnum.Weekday => new List<DayOfWeek> {
                        DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday
                        },
                MonthlyDayEnum.WeekendDay => new List<DayOfWeek> {
                        DayOfWeek.Saturday,
                        DayOfWeek.Sunday
                        },
                _ => new List<DayOfWeek>(),
            };

            if (validDaysOfWeek.Contains(date.DayOfWeek))
            {
                return true;
            }
            return false;
        }
    }
}

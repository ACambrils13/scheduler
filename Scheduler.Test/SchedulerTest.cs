using FluentAssertions;
using System;
using Xunit;

namespace Scheduler.Test
{
    public class SchedulerTest
    {
        public static readonly object[][] TestDataOk =
        {
            new object[] {new DateTime(2021,1,4), ScheduleType.Once, new DateTime(2021,1,8), null, null, new DateTime(2021,1,1), null, new DateTime(2021,1,8) },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Daily, 1, new DateTime(2021,1,1), null, new DateTime(2021,1,5) },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Daily, 5, null, null, new DateTime(2021,1,9) },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Weekly, 2, new DateTime(2021, 1, 1), new DateTime(2021, 12, 31), new DateTime(2021,1,18) },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Monthly, 3, new DateTime(2021, 1, 1), new DateTime(2021, 12, 31), new DateTime(2021,4,4) },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Yearly, 1, null, null, new DateTime(2022,1,4) },
        };

        public static readonly object[][] TestDataException =
        {
            new object[] {null, ScheduleType.Once, null, null, null, new DateTime(2021,1,1), null },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Weekly, null, new DateTime(2021, 1, 1), new DateTime(2021, 12, 31) },
        };

        [Theory, MemberData(nameof(TestDataOk))]
        public void Next_Execution_Should_Be_As_Expected(DateTime CurrentDate, ScheduleType Type, DateTime? ExecutionDate, OccurrencyPeriod PeriodType, int? Period, DateTime? Start, DateTime? End, DateTime ExpectedResult)
        {
            Scheduler Scheduler = new();
            ScheduleEvent NextExecution = Scheduler.GetNextExecution(CurrentDate, Type, ExecutionDate, PeriodType, Period, Start, End);
            NextExecution.ExecutionTime.Should().BeSameDateAs(ExpectedResult);
        }

        [Theory, MemberData(nameof(TestDataException))]
        public void Next_Execution_Should_Cause_Exception(DateTime CurrentDate, ScheduleType? Type, DateTime? ExecutionDate, OccurrencyPeriod PeriodType, int? Period, DateTime? Start, DateTime? End)
        {
            Scheduler Scheduler = new();
            Assert.Throws<Exception>(() => Scheduler.GetNextExecution(CurrentDate, Type.Value, ExecutionDate, PeriodType, Period, Start, End));
        }
    }
}

using FluentAssertions;
using System;
using Xunit;

namespace Scheduler.Test
{
    public class SchedulerTest
    {
        public static readonly object[][] TestData =
        {
            new object[] {new DateTime(2021,1,4), ScheduleType.Once, new DateTime(2021,1,8), null, null, new DateTime(2021,1,1), null, new DateTime(2021,1,8) },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Daily, 1, new DateTime(2021,1,1), null, new DateTime(2021,1,5) },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Daily, 5, null, null, new DateTime(2021,1,9) },
            new object[] {new DateTime(2021,1,4), ScheduleType.Recurring, null, OccurrencyPeriod.Weekly, 2, new DateTime(2021, 1, 1), new DateTime(2021, 12, 31), new DateTime(2021,1,18) },
        };

        [Theory, MemberData(nameof(TestData))]
        public void Next_Execution_Should_Be_As_Expected(DateTime CurrentDate, ScheduleType Type, DateTime? ExecutionDate, OccurrencyPeriod PeriodType, int? Period, DateTime? Start, DateTime? End, DateTime ExpectedResult)
        {
            Scheduler Scheduler = new();
            ScheduleEvent NextExecution = Scheduler.GetNextExecution(CurrentDate, Type, ExecutionDate, PeriodType, Period, Start, End);
            NextExecution.ExecutionTime.Should().BeSameDateAs(ExpectedResult);
        }
    }
}

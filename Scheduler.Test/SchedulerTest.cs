using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Scheduler.Test
{
    public class SchedulerTest
    {
        [Theory]
        [InlineData("25/10/2021")]
        [InlineData("25-10-21")]
        [InlineData("25/10/21 10:02:05")]
        public void CurrentDate_Correct_Asignation(string Date)
        {
            Scheduler scheduler = new Scheduler()
            {
                CurrentDate = DateTime.Parse(Date)
            };

            Assert.Equal(scheduler.CurrentDate.ToString(), DateTime.Parse(Date).ToString());
        }

        [Theory]
        [InlineData("2510/2021")]
        [InlineData("error")]
        [InlineData("31/09/21 10:02")]
        public void CurrentDate_Incorrect_Asignation(string Date)
        {
            Scheduler scheduler = new Scheduler();

            Assert.Throws<FormatException>(() =>
                scheduler.CurrentDate = DateTime.Parse(Date)
            );
        }

        [Theory]
        [InlineData(ScheduleTypeEnum.Once)]
        [InlineData(ScheduleTypeEnum.Recurring)]
        [InlineData(null)]
        public void ScheduleTypeEnum_Correct_Asignation(ScheduleTypeEnum? ScheduleType)
        {
            Scheduler scheduler = new Scheduler()
            {
                Type = ScheduleType
            };

            Assert.Equal(scheduler.Type, ScheduleType);
        }
        

        [Theory]
        [InlineData("25/10/2021")]
        [InlineData("25-10-21")]
        [InlineData("25/10/21 10:02:05")]
        public void Limits_StartDate_Correct_Asignation(string Date)
        {
            LimitsConfig limits = new LimitsConfig(DateTime.Parse(Date), null);

            Assert.Equal(limits.StartLimit.ToString(), DateTime.Parse(Date).ToString());
        }



        //public static readonly object[][] TestDataOk =
        //{
        //    new object[] {new DateTime(2021,1,4), ScheduleTypeEnum.Once, new DateTime(2021,1,8), null, null, new DateTime(2021,1,1), null, new DateTime(2021,1,8) },
        //    new object[] {new DateTime(2021,1,4), ScheduleTypeEnum.Recurring, null, OccurrencyPeriodEnum.Daily, 1, new DateTime(2021,1,1), null, new DateTime(2021,1,5) },
        //    new object[] {new DateTime(2021,1,4), ScheduleTypeEnum.Recurring, null, OccurrencyPeriodEnum.Daily, 5, null, null, new DateTime(2021,1,9) },
        //    new object[] {new DateTime(2021,1,4), ScheduleTypeEnum.Recurring, null, OccurrencyPeriodEnum.Weekly, 2, new DateTime(2021, 1, 1), new DateTime(2021, 12, 31), new DateTime(2021,1,18) },
        //    new object[] {new DateTime(2021,1,4), ScheduleTypeEnum.Recurring, null, OccurrencyPeriodEnum.Monthly, 3, new DateTime(2021, 1, 1), new DateTime(2021, 12, 31), new DateTime(2021,4,4) },
        //    new object[] {new DateTime(2021,1,4), ScheduleTypeEnum.Recurring, null, OccurrencyPeriodEnum.Yearly, 1, null, null, new DateTime(2022,1,4) },
        //};

        //public static readonly object[][] TestDataException =
        //{
        //    new object[] {null, ScheduleTypeEnum.Once, null, null, null, new DateTime(2021,1,1), null },
        //    new object[] {new DateTime(2021,1,4), ScheduleTypeEnum.Recurring, null, OccurrencyPeriodEnum.Weekly, null, new DateTime(2021, 1, 1), new DateTime(2021, 12, 31) },
        //};

        //[Theory, MemberData(nameof(TestDataOk))]
        //public void Next_Execution_Should_Be_As_Expected(DateTime CurrentDate, ScheduleTypeEnum Type, DateTime? ExecutionDate, OccurrencyPeriodEnum PeriodType, int? Period, DateTime? Start, DateTime? End, DateTime ExpectedResult)
        //{
        //    Scheduler Scheduler = new();
        //    ScheduleEvent NextExecution = Scheduler.GetNextExecution(CurrentDate, Type, ExecutionDate, PeriodType, Period, Start, End);
        //    NextExecution.ExecutionTime.Should().BeSameDateAs(ExpectedResult);
        //}

        //[Theory, MemberData(nameof(TestDataException))]
        //public void Next_Execution_Should_Cause_Exception(DateTime CurrentDate, ScheduleTypeEnum? Type, DateTime? ExecutionDate, OccurrencyPeriodEnum PeriodType, int? Period, DateTime? Start, DateTime? End)
        //{
        //    Scheduler Scheduler = new();
        //    Assert.Throws<Exception>(() => Scheduler.GetNextExecution(CurrentDate, Type.Value, ExecutionDate, PeriodType, Period, Start, End));
        //}



    }
}

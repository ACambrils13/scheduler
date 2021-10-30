using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Scheduler.Resources;
using System.Text;

namespace Scheduler.Test
{
    public class SchedulerTest
    {
        [Fact]
        public void CurrentDate_Correct_Asignation()
        {
            string Date = DateTime.Now.ToString();
            Scheduler SchedulerObj = new Scheduler()
            {
                CurrentDate = DateTime.Parse(Date)
            };

            Assert.Equal(SchedulerObj.CurrentDate.ToString(), DateTime.Parse(Date).ToString());
        }

        [Theory]
        [InlineData(ScheduleTypeEnum.Once)]
        [InlineData(ScheduleTypeEnum.Recurring)]
        [InlineData(null)]
        public void ScheduleTypeEnum_Correct_Asignation(ScheduleTypeEnum? ScheduleType)
        {
            Scheduler SchedulerObj = new Scheduler()
            {
                Type = ScheduleType
            };

            Assert.Equal(SchedulerObj.Type, ScheduleType);
        }

        [Fact]
        public void Limits_StartDate_Correct_Asignation()
        {
            string Date = DateTime.Now.ToString();
            LimitsConfig limits = new LimitsConfig(DateTime.Parse(Date), null);

            Assert.Equal(limits.StartLimit.ToString(), DateTime.Parse(Date).ToString());
        }

        [Fact]
        public void Limits_StartDate_EndDate_Correct_Asignation()
        {
            string StartDate = DateTime.Now.ToString();
            string EndDate = DateTime.Now.AddDays(1).ToString();
            LimitsConfig limits = new LimitsConfig(DateTime.Parse(StartDate), DateTime.Parse(EndDate));

            Assert.Equal(limits.StartLimit.ToString(), DateTime.Parse(StartDate).ToString());
            Assert.Equal(limits.EndLimit.ToString(), DateTime.Parse(EndDate).ToString());        
        }

        [Fact]
        public void Limits_Only_EndDate_Failed()
        {
            Assert.Throws<ValidationException>(() =>
                new LimitsConfig(null, DateTime.Now));
        }

        [Fact]
        public void Limit_EndDate_Before_StartDate_Failed()
        {
            Assert.Throws<ValidationException>(() =>
                new LimitsConfig(DateTime.Now, DateTime.Now.AddDays(-1)));
        }

        [Fact]
        public void  ScheduleDate_Correct_Asignation()
        {
            string Date = DateTime.Now.ToString();
            Scheduler SchedulerObj = new Scheduler()
            {
                ScheduleDate = DateTime.Parse(Date)
            };

            Assert.Equal(SchedulerObj.ScheduleDate.ToString(), DateTime.Parse(Date).ToString());
        }

        [Theory]
        [InlineData(OccurrencyPeriodEnum.Daily)]
        [InlineData(OccurrencyPeriodEnum.Weekly)]
        [InlineData(OccurrencyPeriodEnum.Monthly)]
        [InlineData(OccurrencyPeriodEnum.Yearly)]
        [InlineData(null)]
        public void OccurrencyPeriodEnum_Correct_Asignation(OccurrencyPeriodEnum? OcurrencyPeriod)
        {
            Scheduler SchedulerObj = new Scheduler()
            {
                PeriodType = OcurrencyPeriod
            };

            Assert.Equal(SchedulerObj.PeriodType, OcurrencyPeriod);
        }

        [Fact]
        public void DailyScheduleHour_Correct_Asignation()
        {
            string Date = DateTime.Now.ToString();
            Scheduler SchedulerObj = new Scheduler()
            {
                DailyScheduleHour = DateTime.Parse(Date)
            };

            Assert.Equal(SchedulerObj.DailyScheduleHour.ToString(), DateTime.Parse(Date).ToString());
        }

        [Fact]
        public void Configuration_Once_CurrentDate_Null_Failed()
        {
            Scheduler SchedulerObj = new Scheduler()
            {
                CurrentDate = null,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = DateTime.Now.AddDays(1)
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Once_CurrentDate_MaxValue_Failed()
        {
            Scheduler SchedulerObj = new Scheduler()
            {
                CurrentDate = DateTime.MaxValue,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = DateTime.Now.AddDays(1)
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }


        [Fact]
        public void Configuration_Once_Type_Null_Failed()
        {
            Scheduler SchedulerObj = new Scheduler()
            {
                CurrentDate = DateTime.Now,
                Type = null,
                ScheduleDate = DateTime.Now.AddDays(1)
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Once_ScheduleDate_Null_Failed()
        {
            Scheduler SchedulerObj = new Scheduler()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = null
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Once_Next_Execution_Without_Limits_Correct()
        {
            string ScheduleDateEx = DateTime.Now.AddDays(1).ToString();
            string ExecScheduleDate = DateTime.Parse(ScheduleDateEx).ToShortDateString();
            string ExecScheduleHour = DateTime.Parse(ScheduleDateEx).ToString("HH:mm");
            string ExecDescription = string.Concat(TextResources.EventDescOnce, " ", string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour));

            Scheduler SchedulerObj = new Scheduler()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = DateTime.Parse(ScheduleDateEx)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Configuration_Once_Next_Execution_Limits_Start_Correct()
        {
            string ScheduleDateEx = DateTime.Now.AddDays(1).ToString();
            string DateLimitsStart = DateTime.Now.AddDays(-10).ToString();
            string ExecScheduleDate = DateTime.Parse(ScheduleDateEx).ToShortDateString();
            string ExecScheduleHour = DateTime.Parse(ScheduleDateEx).ToString("HH:mm");
            string ExecScheduleLimitStart = DateTime.Parse(DateLimitsStart).ToShortDateString();
            StringBuilder ExecDescription = new StringBuilder();
            ExecDescription.AppendJoin(" ", TextResources.EventDescOnce, string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour),
                string.Format(TextResources.EventDescLimitsStart, ExecScheduleLimitStart));

            Scheduler SchedulerObj = new Scheduler()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = DateTime.Parse(ScheduleDateEx),
                DateLimits = new LimitsConfig(DateTime.Parse(ExecScheduleLimitStart),null)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Configuration_Once_Next_Execution_Limits_Correct()
        {
            string ScheduleDateEx = DateTime.Now.AddDays(1).ToString();
            string DateLimitsStart = DateTime.Now.AddDays(-10).ToString();
            string DateLimitsEnd = DateTime.Now.AddDays(10).ToString();
            string ExecScheduleDate = DateTime.Parse(ScheduleDateEx).ToShortDateString();
            string ExecScheduleHour = DateTime.Parse(ScheduleDateEx).ToString("HH:mm");
            string ExecScheduleLimitStart = DateTime.Parse(DateLimitsStart).ToShortDateString();
            string ExecScheduleLimitEnd = DateTime.Parse(DateLimitsEnd).ToShortDateString();
            StringBuilder ExecDescription = new StringBuilder();
            ExecDescription.AppendJoin(" ", TextResources.EventDescOnce, string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour),
                string.Format(TextResources.EventDescLimitsStart, ExecScheduleLimitStart), string.Format(TextResources.EventDescLimitsEnd,ExecScheduleLimitEnd));

            Scheduler SchedulerObj = new Scheduler()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = DateTime.Parse(ScheduleDateEx),
                DateLimits = new LimitsConfig(DateTime.Parse(ExecScheduleLimitStart), DateTime.Parse(ExecScheduleLimitEnd))
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
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

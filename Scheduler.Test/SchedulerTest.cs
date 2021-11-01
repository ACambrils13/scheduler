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
        #region Scheduler
        [Fact]
        public void CurrentDate_Correct_Asignation()
        {
            string Date = DateTime.Now.ToString();
            Scheduler SchedulerObj = new ()
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
            Scheduler SchedulerObj = new ()
            {
                Type = ScheduleType
            };

            Assert.Equal(SchedulerObj.Type, ScheduleType);
        }

        [Fact]
        public void Limits_StartDate_Correct_Asignation()
        {
            string Date = DateTime.Now.ToString();
            LimitsConfig limits = new (DateTime.Parse(Date), null);

            Assert.Equal(limits.StartLimit.ToString(), DateTime.Parse(Date).ToString());
        }

        [Fact]
        public void Limits_StartDate_EndDate_Correct_Asignation()
        {
            string StartDate = DateTime.Now.ToString();
            string EndDate = DateTime.Now.AddDays(1).ToString();
            LimitsConfig limits = new (DateTime.Parse(StartDate), DateTime.Parse(EndDate));

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
        public void ScheduleDate_Correct_Asignation()
        {
            string Date = DateTime.Now.ToString();
            Scheduler SchedulerObj = new ()
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
            Scheduler SchedulerObj = new ()
            {
                PeriodType = OcurrencyPeriod
            };

            Assert.Equal(SchedulerObj.PeriodType, OcurrencyPeriod);
        }

        [Fact]
        public void DailyScheduleHour_Correct_Asignation()
        {
            string Date = DateTime.Now.ToString();
            Scheduler SchedulerObj = new ()
            {
                DailyScheduleHour = DateTime.Parse(Date)
            };

            Assert.Equal(SchedulerObj.DailyScheduleHour.ToString(), DateTime.Parse(Date).ToString());
        }

        [Theory]
        [InlineData(DailyFrecuencyEnum.Hours)]
        [InlineData(DailyFrecuencyEnum.Minutes)]
        [InlineData(DailyFrecuencyEnum.Seconds)]
        [InlineData(null)]
        public void DailyPeriodEnum_Correct_Asignation(DailyFrecuencyEnum? OcurrencyPeriod)
        {
            Scheduler SchedulerObj = new()
            {
                DailyFrecuency = OcurrencyPeriod
            };

            Assert.Equal(SchedulerObj.DailyFrecuency, OcurrencyPeriod);
        }

        [Fact]
        public void Configuration_Type_Null_Failed()
        {
            Scheduler SchedulerObj = new()
            {
                CurrentDate = DateTime.Now,
                Type = null
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }
        #endregion

        #region Once
        [Fact]
        public void Configuration_Once_CurrentDate_Null_Failed()
        {
            Scheduler SchedulerObj = new ()
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
            Scheduler SchedulerObj = new ()
            {
                CurrentDate = DateTime.MaxValue,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = DateTime.Now.AddDays(1)
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Once_ScheduleDate_Null_Failed()
        {
            Scheduler SchedulerObj = new ()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = null
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Once_Next_Execution_Without_Limits_Correct()
        {
            string ScheduleDateEx = DateTime.Now.AddDays(1).ToString();
            string ExecScheduleDate = DateTime.Parse(ScheduleDateEx).ToShortDateString();
            string ExecScheduleHour = DateTime.Parse(ScheduleDateEx).ToString("HH:mm");
            string ExecDescription = string.Concat(TextResources.EventDescOnce, " ", string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour));

            Scheduler SchedulerObj = new ()
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
        public void Once_Next_Execution_Limits_Start_Correct()
        {
            string ScheduleDateEx = DateTime.Now.AddDays(1).ToString();
            string DateLimitsStart = DateTime.Now.AddDays(-10).ToString();
            string ExecScheduleDate = DateTime.Parse(ScheduleDateEx).ToShortDateString();
            string ExecScheduleHour = DateTime.Parse(ScheduleDateEx).ToString("HH:mm");
            string ExecScheduleLimitStart = DateTime.Parse(DateLimitsStart).ToShortDateString();
            StringBuilder ExecDescription = new ();
            ExecDescription.AppendJoin(" ", TextResources.EventDescOnce, string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour),
                string.Format(TextResources.EventDescLimitsStart, ExecScheduleLimitStart));

            Scheduler SchedulerObj = new ()
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
        public void Once_Next_Execution_Limits_Correct()
        {
            string ScheduleDateEx = DateTime.Now.AddDays(1).ToString();
            string DateLimitsStart = DateTime.Now.AddDays(-10).ToString();
            string DateLimitsEnd = DateTime.Now.AddDays(10).ToString();
            string ExecScheduleDate = DateTime.Parse(ScheduleDateEx).ToShortDateString();
            string ExecScheduleHour = DateTime.Parse(ScheduleDateEx).ToString("HH:mm");
            string ExecScheduleLimitStart = DateTime.Parse(DateLimitsStart).ToShortDateString();
            string ExecScheduleLimitEnd = DateTime.Parse(DateLimitsEnd).ToShortDateString();
            StringBuilder ExecDescription = new ();
            ExecDescription.AppendJoin(" ", TextResources.EventDescOnce, string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour),
                string.Format(TextResources.EventDescLimitsStart, ExecScheduleLimitStart), string.Format(TextResources.EventDescLimitsEnd,ExecScheduleLimitEnd));

            Scheduler SchedulerObj = new ()
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

        [Fact]
        public void Once_Schedule_Before_Limit_Failed()
        {
            DateTime ScheduleDateEx = DateTime.Now.AddDays(-5);
            DateTime DateLimitsStart = DateTime.Now;

            Scheduler SchedulerObj = new()
            {
                CurrentDate = DateTime.Now.AddDays(-10),
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = ScheduleDateEx,
                DateLimits = new LimitsConfig(DateLimitsStart, null)
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Once_After_Limit_Failed()
        {
            DateTime ScheduleDateEx = DateTime.Now.AddDays(10);
            DateTime DateLimitsStart = DateTime.Now.AddDays(-10);
            DateTime DateLimitsEnd = DateTime.Now.AddDays(5);

            Scheduler SchedulerObj = new()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = ScheduleDateEx,
                DateLimits = new LimitsConfig(DateLimitsStart, DateLimitsEnd)
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }
        #endregion

        #region Recurring
        [Fact]
        public void Configuration_Recurring_CurrentDate_Null_Failed()
        {
            Scheduler SchedulerObj = new()
            {
                CurrentDate = null,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyScheduleHour = DateTime.Now
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Recurring_CurrentDate_MaxValue_Failed()
        {
            Scheduler SchedulerObj = new()
            {
                CurrentDate = DateTime.MaxValue,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyScheduleHour = DateTime.Now
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Recurring_OcurrencyPeriod_Null_Failed()
        {
            Scheduler SchedulerObj = new()
            {
                CurrentDate = DateTime.MaxValue,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                DailyScheduleHour = DateTime.Now
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Recurring_OcurrencyPeriod_Negative_Failed()
        {
            Scheduler SchedulerObj = new()
            {
                CurrentDate = DateTime.MaxValue,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = -1,
                DailyScheduleHour = DateTime.Now
            };

            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Recurring_After_Limit_Failed()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021 8:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            DateTime ExecScheduleLimitEnd = DateTime.Parse("15/02/2021");

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 2,
                DailyScheduleHour = DateTime.Parse("05:00"),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Recurring_DailyOcurrencyPeriod_Null_Fail()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyFrecuency = DailyFrecuencyEnum.Seconds,
                DailyLimits = new LimitsConfig(DateTime.Parse("04:00"), DateTime.Parse("08:00"))
            };
            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Recurring_DailyOcurrencyPeriod_Negative_Fail()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyFrecuency = DailyFrecuencyEnum.Minutes,
                DailyFrecuencyPeriod = -40,
                DailyLimits = new LimitsConfig(DateTime.Parse("04:00"), DateTime.Parse("08:00"))
            };
            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        [Fact]
        public void Configuration_Recurring_DailyOcurrency_Null_Fail()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyFrecuencyPeriod = -40,
                DailyLimits = new LimitsConfig(DateTime.Parse("04:00"), DateTime.Parse("08:00"))
            };
            Assert.Throws<ValidationException>(() =>
               SchedulerObj.GetNextExecution());
        }

        #region Daily
        [Fact]
        public void Recurring_Daily_Next_Execution_Without_Limits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2021 05:00:00");
            string ExecDescription = "Occurs every 2 days at 5:00";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 2,
                DailyScheduleHour = DateTime.Parse("05:00")
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Daily_Next_Execution_DateLimits_Start_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021 7:00");
            DateTime ScheduleDateEx = DateTime.Parse("03/01/2021 5:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            string ExecDescription = "Occurs every 2 days at 5:00 starting on 01/01/2021";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 2,
                DailyScheduleHour = DateTime.Parse("05:00"),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, null)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Recurring_Daily_Next_Execution_DateLimits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2021 5:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            DateTime ExecScheduleLimitEnd = DateTime.Parse("31/01/2021");
            string ExecDescription = "Occurs every 2 days at 5:00 starting on 01/01/2021 to 31/01/2021";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 2,
                DailyScheduleHour = DateTime.Parse("05:00"),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Recurring_Daily_Next_Execution_DailyLimits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021 6:30");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2021 8:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            DateTime ExecScheduleLimitEnd = DateTime.Parse("31/01/2021");
            string ExecDescription = "Occurs every 2 days every 2 hours between 4:00 and 8:00 starting on 01/01/2021 to 31/01/2021";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 2,
                DailyFrecuency = DailyFrecuencyEnum.Hours,
                DailyFrecuencyPeriod = 2,
                DailyLimits = new LimitsConfig(DateTime.Parse("04:00"), DateTime.Parse("08:00")),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }
        #endregion

        #region Monthly
        [Fact]
        public void Recurring_Monthly_Next_Execution_Without_Limits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2021 05:00:00");
            string ExecDescription = "Occurs every 1 months at 5:00";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = DateTime.Parse("05:00")
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_DateLimits_Start_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021 18:00");
            DateTime ScheduleDateEx = DateTime.Parse("01/02/2021 5:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            string ExecDescription = "Occurs every 1 months at 5:00 starting on 01/01/2021";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = DateTime.Parse("05:00"),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, null)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_DateLimits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2021 5:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            DateTime ExecScheduleLimitEnd = DateTime.Parse("31/12/2021");
            string ExecDescription = "Occurs every 1 months at 5:00 starting on 01/01/2021 to 31/12/2021";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = DateTime.Parse("05:00"),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_DailyLimits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021 23:00");
            DateTime ScheduleDateEx = DateTime.Parse("01/02/2021 4:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            DateTime ExecScheduleLimitEnd = DateTime.Parse("31/12/2021");
            string ExecDescription = "Occurs every 1 months every 30 minutes between 4:00 and 8:00 starting on 01/01/2021 to 31/12/2021";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 1,
                DailyFrecuency = DailyFrecuencyEnum.Minutes,
                DailyFrecuencyPeriod = 30,
                DailyLimits = new LimitsConfig(DateTime.Parse("04:00"), DateTime.Parse("08:00")),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }
        #endregion

        #region Yearly
        [Fact]
        public void Recurring_Yearly_Next_Execution_Without_Limits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2021 05:00:00");
            string ExecDescription = "Occurs every 1 years at 5:00";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = DateTime.Parse("05:00")
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Yearly_Next_Execution_DateLimits_Start_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021 6:00");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2022 5:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            string ExecDescription = "Occurs every 1 years at 5:00 starting on 01/01/2021";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = DateTime.Parse("05:00"),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, null)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Recurring_Yearly_Next_Execution_DateLimits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2021 5:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            DateTime ExecScheduleLimitEnd = DateTime.Parse("31/01/2022");
            string ExecDescription = "Occurs every 1 years at 5:00 starting on 01/01/2021 to 31/01/2022";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = DateTime.Parse("05:00"),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Recurring_Yearly_Next_Execution_DailyLimits_Correct()
        {
            DateTime CurrentDateEx = DateTime.Parse("01/01/2021 12:00");
            DateTime ScheduleDateEx = DateTime.Parse("01/01/2022 4:00");
            DateTime ExecScheduleLimitStart = DateTime.Parse("01/01/2021");
            DateTime ExecScheduleLimitEnd = DateTime.Parse("31/01/2022");
            string ExecDescription = "Occurs every 1 years every 20 seconds between 4:00 and 8:00 starting on 01/01/2021 to 31/01/2022";

            Scheduler SchedulerObj = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyFrecuency = DailyFrecuencyEnum.Seconds,
                DailyFrecuencyPeriod = 20,
                DailyLimits = new LimitsConfig(DateTime.Parse("04:00"), DateTime.Parse("08:00")),
                DateLimits = new LimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = SchedulerObj.GetNextExecution();

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }
        #endregion

        #endregion
    }
}

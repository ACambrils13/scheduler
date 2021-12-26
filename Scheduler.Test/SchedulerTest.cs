using Scheduler.Auxiliary;
using Scheduler.Configuration;
using Scheduler.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Xunit;

namespace Scheduler.Test
{
    public class SchedulerTest
    {
        #region Scheduler
        [Fact]
        public void Limits_Only_EndDate_Failed()
        {
            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcLimitsEndBeforeStart);
            var exception = Assert.Throws<ValidationException>(() =>
                new DateLimitsConfig(null, DateTime.Now));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Limit_EndDate_Before_StartDate_Failed()
        {
            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcLimitsEndBeforeStart);
            var exception = Assert.Throws<ValidationException>(() =>
                new DateLimitsConfig(DateTime.Now, DateTime.Now.AddDays(-1)));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Type_Null_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now,
                Type = null
            };
            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcEnumError, nameof(schedulerConfig.Type)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }
        #endregion

        #region Once
        [Fact]
        public void Configuration_Once_CurrentDate_Null_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = null,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = DateTime.Now.AddDays(1)
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcObjectNull, nameof(schedulerConfig.CurrentDate)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Once_CurrentDate_MaxValue_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.MaxValue,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = DateTime.Now.AddDays(1)
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcDateMaxValue, nameof(schedulerConfig.CurrentDate)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Once_ScheduleDate_Null_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = null
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcObjectNull, nameof(schedulerConfig.ScheduleDate)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Once_Next_Execution_Without_Limits_Correct()
        {
            DateTime ScheduleDateEx = DateTime.Now.AddDays(1);
            string ExecScheduleDate = ScheduleDateEx.ToShortDateString();
            string ExecScheduleHour = ScheduleDateEx.ToString("HH:mm");
            string ExecDescription = string.Concat(TextResources.EventDescOnce, " ", string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour));

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = ScheduleDateEx
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Once_Next_Execution_Limits_Start_Correct()
        {
            DateTime ScheduleDateEx = DateTime.Now.AddDays(1);
            DateTime DateLimitsStart = DateTime.Now.AddDays(-10);
            string ExecScheduleDate = ScheduleDateEx.ToShortDateString();
            string ExecScheduleHour = ScheduleDateEx.ToString("HH:mm");
            string ExecScheduleLimitStart = DateLimitsStart.ToShortDateString();
            StringBuilder ExecDescription = new();
            ExecDescription.AppendJoin(" ", TextResources.EventDescOnce, string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour),
                string.Format(TextResources.EventDescLimitsStart, ExecScheduleLimitStart));

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = ScheduleDateEx,
                DateLimits = new DateLimitsConfig(DateLimitsStart, null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Once_Next_Execution_Limits_Correct()
        {
            DateTime ScheduleDateEx = DateTime.Now.AddDays(1);
            DateTime DateLimitsStart = DateTime.Now.AddDays(-10);
            DateTime DateLimitsEnd = DateTime.Now.AddDays(10);
            string ExecScheduleDate = ScheduleDateEx.ToShortDateString();
            string ExecScheduleHour = ScheduleDateEx.ToString("HH:mm");
            string ExecScheduleLimitStart = DateLimitsStart.ToShortDateString();
            string ExecScheduleLimitEnd = DateLimitsEnd.ToShortDateString();
            StringBuilder ExecDescription = new();
            ExecDescription.AppendJoin(" ", TextResources.EventDescOnce, string.Format(TextResources.EventDescSchedule, ExecScheduleDate, ExecScheduleHour),
                string.Format(TextResources.EventDescLimitsStart, ExecScheduleLimitStart), string.Format(TextResources.EventDescLimitsEnd, ExecScheduleLimitEnd));

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = ScheduleDateEx,
                DateLimits = new DateLimitsConfig(DateLimitsStart, DateLimitsEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription.ToString());
        }

        [Fact]
        public void Once_Schedule_Before_Limit_Failed()
        {
            DateTime ScheduleDateEx = DateTime.Now.AddDays(-5);
            DateTime DateLimitsStart = DateTime.Now;

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now.AddDays(-10),
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = ScheduleDateEx,
                DateLimits = new DateLimitsConfig(DateLimitsStart, null)
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcLimits);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Once_After_Limit_Failed()
        {
            DateTime ScheduleDateEx = DateTime.Now.AddDays(10);
            DateTime DateLimitsStart = DateTime.Now.AddDays(-10);
            DateTime DateLimitsEnd = DateTime.Now.AddDays(5);

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Once,
                ScheduleDate = ScheduleDateEx,
                DateLimits = new DateLimitsConfig(DateLimitsStart, DateLimitsEnd)
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcLimits);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }
        #endregion

        #region Recurring
        [Fact]
        public void Configuration_Recurring_CurrentDate_Null_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = null,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyScheduleHour = new TimeSpan(DateTime.Now.Hour, 0, 0)
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcObjectNull, nameof(schedulerConfig.CurrentDate)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Recurring_CurrentDate_MaxValue_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.MaxValue,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyScheduleHour = new TimeSpan(DateTime.Now.Hour, 0, 0)
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcDateMaxValue, nameof(schedulerConfig.CurrentDate)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Recurring_OcurrencyPeriod_Null_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                DailyScheduleHour = new TimeSpan(DateTime.Now.Hour, 0, 0)
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcPeriod, nameof(schedulerConfig.OcurrencyPeriod)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Recurring_DailyLimits_End_Before_Start_Failed()
        {
            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcLimitsEndBeforeStart);
            var exception = Assert.Throws<ValidationException>(() =>
               new SchedulerConfigurator()
               {
                   DailyLimits = new HourLimitsConfig(new TimeSpan(14, 0, 0), new TimeSpan(10, 0, 0))
               }
               );
            Assert.Equal(expectedExcMsg, exception.Message);

            exception = Assert.Throws<ValidationException>(() =>
               new SchedulerConfigurator()
               {
                   DailyLimits = new HourLimitsConfig(null, new TimeSpan(10, 0, 0))
               }
               );
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Recurring_OcurrencyPeriod_Negative_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = DateTime.Now,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = -1,
                DailyScheduleHour = new TimeSpan(DateTime.Now.Hour, 0, 0)
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcPeriod, nameof(schedulerConfig.OcurrencyPeriod)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_After_Limit_Failed()
        {
            DateTime CurrentDateEx = new(2021, 01, 01, 8, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 01, 01);
            DateTime ExecScheduleLimitEnd = new(2021, 02, 15);

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 2,
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcLimits);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Recurring_DailyOcurrencyPeriod_Null_Fail()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyFrecuency = DailyFrecuencyEnum.Seconds,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0))
            };
            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcDailyConfig);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Recurring_DailyOcurrencyPeriod_Negative_Fail()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyFrecuency = DailyFrecuencyEnum.Minutes,
                DailyFrecuencyPeriod = -40,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0))
            };
            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcPeriod, nameof(schedulerConfig.DailyFrecuencyPeriod)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Recurring_DailyOcurrency_Null_Fail()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyFrecuencyPeriod = -40,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0))
            };
            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcDailyConfig);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Configuration_Recurring_DailyOcurrency_Schedule_Hour_Fail()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 1,
                DailyScheduleHour = new TimeSpan(-5, 0, 0)
            };
            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcHoursValue, nameof(schedulerConfig.DailyScheduleHour)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        #region Daily
        [Fact]
        public void Recurring_Daily_Next_Execution_Without_Limits_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);
            DateTime ScheduleDateEx = new(2021, 1, 1, 5, 0, 0);
            string ExecDescription = "Occurs every 2 days at 05:00";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 2,
                DailyScheduleHour = new TimeSpan(5, 0, 0)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate.ToString(), ScheduleDateEx.ToString());
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 3, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Daily_Next_Execution_DateLimits_Start_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 7, 0, 0);
            DateTime ScheduleDateEx = new(2021, 1, 2, 5, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 2);
            string ExecDescription = "Occurs every 2 days at 05:00 starting on 02/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 2,
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 4, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Daily_Next_Execution_DateLimits_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);
            DateTime ScheduleDateEx = new(2021, 1, 1, 5, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2021, 1, 31);
            string ExecDescription = "Occurs every 2 days at 05:00 starting on 01/01/2021 to 31/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 2,
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Daily_Next_Execution_DailyLimits_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 6, 30, 0);
            DateTime ScheduleDateEx = new(2021, 1, 1, 8, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2021, 1, 31);
            string ExecDescription = "Occurs every 2 days every 2 hours between 04:00 and 08:00 starting on 01/01/2021 to 31/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 2,
                DailyFrecuency = DailyFrecuencyEnum.Hours,
                DailyFrecuencyPeriod = 2,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0)),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 3, 4, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 3, 6, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 3, 8, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Daily_Next_Execution_DailyLimits_Next_Date_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 10, 30, 0);
            DateTime ScheduleDateEx = new(2021, 1, 6, 4, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2021, 1, 31);
            string ExecDescription = "Occurs every 5 days every 1 hours between 04:00 and 08:00 starting on 01/01/2021 to 31/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Daily,
                OcurrencyPeriod = 5,
                DailyFrecuency = DailyFrecuencyEnum.Hours,
                DailyFrecuencyPeriod = 1,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0)),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 6, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 6, 6, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }
        #endregion

        //#region Monthly
        //[Fact]
        //public void Recurring_Monthly_Next_Execution_Without_Limits_Correct()
        //{
        //    DateTime CurrentDateEx = new(2021, 1, 1);
        //    DateTime ScheduleDateEx = new(2021, 1, 1, 5, 0, 0);
        //    string ExecDescription = "Occurs every 1 months at 05:00";

        //    SchedulerConfigurator schedulerConfig = new()
        //    {
        //        CurrentDate = CurrentDateEx,
        //        Type = ScheduleTypeEnum.Recurring,
        //        PeriodType = OccurrencyPeriodEnum.Monthly,
        //        OcurrencyPeriod = 1,
        //        DailyScheduleHour = new TimeSpan(5, 0, 0)
        //    };
        //    ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

        //    NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 2, 1, 5, 0, 0));
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        //}

        //[Fact]
        //public void Recurring_Monthly_Next_Execution_DateLimits_Start_Correct()
        //{
        //    DateTime CurrentDateEx = new(2021, 1, 1, 18, 0, 0);
        //    DateTime ScheduleDateEx = new(2021, 2, 1, 5, 0, 0);
        //    DateTime ExecScheduleLimitStart = new(2021, 1, 1);
        //    string ExecDescription = "Occurs every 1 months at 05:00 starting on 01/01/2021";

        //    SchedulerConfigurator schedulerConfig = new()
        //    {
        //        CurrentDate = CurrentDateEx,
        //        Type = ScheduleTypeEnum.Recurring,
        //        PeriodType = OccurrencyPeriodEnum.Monthly,
        //        OcurrencyPeriod = 1,
        //        DailyScheduleHour = new TimeSpan(5, 0, 0),
        //        DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, null)
        //    };
        //    ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

        //    NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 3, 1, 5, 0, 0));
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        //}

        //[Fact]
        //public void Recurring_Monthly_Next_Execution_DateLimits_Correct()
        //{
        //    DateTime CurrentDateEx = new(2021, 1, 1);
        //    DateTime ScheduleDateEx = new(2021, 1, 1, 5, 0, 0);
        //    DateTime ExecScheduleLimitStart = new(2021, 1, 1);
        //    DateTime ExecScheduleLimitEnd = new(2021, 12, 31);
        //    string ExecDescription = "Occurs every 1 months at 05:00 starting on 01/01/2021 to 31/12/2021";

        //    SchedulerConfigurator schedulerConfig = new()
        //    {
        //        CurrentDate = CurrentDateEx,
        //        Type = ScheduleTypeEnum.Recurring,
        //        PeriodType = OccurrencyPeriodEnum.Monthly,
        //        OcurrencyPeriod = 1,
        //        DailyScheduleHour = new TimeSpan(5, 0, 0),
        //        DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
        //    };
        //    ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        //}

        //[Fact]
        //public void Recurring_Monthly_Next_Execution_DailyLimits_Correct()
        //{
        //    DateTime CurrentDateEx = new(2021, 1, 1, 23, 0, 0);
        //    DateTime ScheduleDateEx = new(2021, 2, 1, 4, 0, 0);
        //    DateTime ExecScheduleLimitStart = new(2021, 1, 1);
        //    DateTime ExecScheduleLimitEnd = new(2021, 12, 31);
        //    string ExecDescription = "Occurs every 1 months every 30 minutes between 04:00 and 05:00 starting on 01/01/2021 to 31/12/2021";

        //    SchedulerConfigurator schedulerConfig = new()
        //    {
        //        CurrentDate = CurrentDateEx,
        //        Type = ScheduleTypeEnum.Recurring,
        //        PeriodType = OccurrencyPeriodEnum.Monthly,
        //        OcurrencyPeriod = 1,
        //        DailyFrecuency = DailyFrecuencyEnum.Minutes,
        //        DailyFrecuencyPeriod = 30,
        //        DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(5, 0, 0)),
        //        DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
        //    };
        //    ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

        //    NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 2, 1, 4, 30, 0));
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

        //    NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 2, 1, 5, 0, 0));
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

        //    NextExec = Scheduler.GetNextExecution(schedulerConfig);

        //    Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 3, 1, 4, 0, 0));
        //    Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        //}
        //#endregion

        #region Yearly
        [Fact]
        public void Recurring_Yearly_Next_Execution_Without_Limits_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);
            DateTime ScheduleDateEx = new(2021, 1, 1, 5, 0, 0);
            string ExecDescription = "Occurs every 1 years at 05:00";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = new TimeSpan(5, 0, 0)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2022, 1, 1, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Yearly_Next_Execution_DateLimits_Start_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 6, 0, 0);
            DateTime ScheduleDateEx = new(2022, 1, 1, 5, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            string ExecDescription = "Occurs every 1 years at 05:00 starting on 01/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2023, 1, 1, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Yearly_Next_Execution_DateLimits_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);
            DateTime ScheduleDateEx = new(2021, 1, 1, 5, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2022, 1, 31);
            string ExecDescription = "Occurs every 1 years at 05:00 starting on 01/01/2021 to 31/01/2022";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2022, 1, 1, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcLimits);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_Yearly_Next_Execution_DailyLimits_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 12, 0, 0);
            DateTime ScheduleDateEx = new(2022, 1, 1, 4, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2022, 1, 31);
            string ExecDescription = "Occurs every 1 years every 20 seconds between 04:00 and 08:00 starting on 01/01/2021 to 31/01/2022";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Yearly,
                OcurrencyPeriod = 1,
                DailyFrecuency = DailyFrecuencyEnum.Seconds,
                DailyFrecuencyPeriod = 20,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0)),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2022, 1, 1, 4, 0, 20));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2022, 1, 1, 4, 0, 40));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }
        #endregion

        #region Weekly
        [Fact]
        public void Reccuring_Weekly_Next_Execution_Without_Limits_Without_WeekDays_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);
            DateTime ScheduleDateEx = new(2021, 1, 1, 5, 0, 0);
            string ExecDescription = "Occurs every 2 weeks at 05:00";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 2,
                DailyScheduleHour = new TimeSpan(5, 0, 0)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 15, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Weekly_Next_Execution_DateLimits_Start_Without_WeekDays_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 18, 0, 0);
            DateTime ScheduleDateEx = new(2021, 1, 22, 5, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            string ExecDescription = "Occurs every 3 weeks at 05:00 starting on 01/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 3,
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 2, 12, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Weekly_Next_Execution_DateLimits_Without_WeekDays_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);
            DateTime ScheduleDateEx = new(2021, 1, 1, 5, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2021, 12, 31);
            string ExecDescription = "Occurs every 1 weeks at 05:00 starting on 01/01/2021 to 31/12/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 1,
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 8, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Weekly_Next_Execution_DailyLimits_Without_WeekDays_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 23, 0, 0);
            DateTime ScheduleDateEx = new(2021, 1, 8, 4, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2021, 12, 31);
            string ExecDescription = "Occurs every 1 weeks every 30 minutes between 04:00 and 08:00 starting on 01/01/2021 to 31/12/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 1,
                DailyFrecuency = DailyFrecuencyEnum.Minutes,
                DailyFrecuencyPeriod = 30,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0)),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 8, 4, 30, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 8, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Reccuring_Weekly_Next_Execution_Without_Limits_WeekDays_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);
            DateTime ScheduleDateEx = new(2021, 1, 12, 5, 0, 0);
            string ExecDescription = "Occurs every 2 weeks on Tuesday and Thursday at 05:00";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 2,
                WeeklyDays = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday },
                DailyScheduleHour = new TimeSpan(5, 0, 0),
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 14, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 26, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Weekly_Next_Execution_DateLimits_Start_WeekDays_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 18, 0, 0);
            DateTime ScheduleDateEx = new(2021, 1, 2, 5, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            string ExecDescription = "Occurs every 3 weeks on Friday, Saturday and Sunday at 05:00 starting on 01/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 3,
                WeeklyDays = new List<DayOfWeek> { DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday },
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 3, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 22, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 23, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Weekly_Next_Execution_DateLimits_WeekDays_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1);
            DateTime ScheduleDateEx = new(2021, 1, 13, 5, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 7);
            DateTime ExecScheduleLimitEnd = new(2021, 12, 31);
            string ExecDescription = "Occurs every 1 weeks on Wednesday at 05:00 starting on 07/01/2021 to 31/12/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 1,
                WeeklyDays = new List<DayOfWeek> { DayOfWeek.Wednesday },
                DailyScheduleHour = new TimeSpan(5, 0, 0),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 20, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

        }

        [Fact]
        public void Recurring_Weekly_Next_Execution_DailyLimits_WeekDays_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 23, 0, 0);
            DateTime ScheduleDateEx = new(2021, 1, 3, 4, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2021, 12, 31);
            string ExecDescription = "Occurs every 1 weeks on Sunday every 30 minutes between 04:00 and 05:00 starting on 01/01/2021 to 31/12/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 1,
                WeeklyDays = new List<DayOfWeek> { DayOfWeek.Sunday },
                DailyFrecuency = DailyFrecuencyEnum.Minutes,
                DailyFrecuencyPeriod = 30,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(5, 0, 0)),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 3, 4, 30, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 3, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 10, 4, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Weekly_Next_Execution_DailyLimits_WeekDays_Sunday_Correct()
        {
            DateTime CurrentDateEx = new(2021, 1, 1, 23, 0, 0);
            DateTime ScheduleDateEx = new(2021, 1, 14, 4, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 3);
            DateTime ExecScheduleLimitEnd = new(2021, 12, 31);
            string ExecDescription = "Occurs every 2 weeks on Thursday every 60 seconds between 04:00 and 05:00 starting on 03/01/2021 to 31/12/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 2,
                WeeklyDays = new List<DayOfWeek> { DayOfWeek.Thursday },
                DailyFrecuency = DailyFrecuencyEnum.Seconds,
                DailyFrecuencyPeriod = 60,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(5, 0, 0)),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 14, 4, 1, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Weekly_Next_Execution_DailyLimits_WeekDays_Culture_GB_Correct()
        {
            CultureInfo.CurrentCulture = new("en-GB");
            DateTime CurrentDateEx = new(2021, 1, 1, 23, 0, 0);
            DateTime ScheduleDateEx = new(2021, 1, 3, 4, 0, 0);
            DateTime ExecScheduleLimitStart = new(2021, 1, 1);
            DateTime ExecScheduleLimitEnd = new(2021, 12, 31);
            string ExecDescription = "Occurs every 1 weeks on Sunday every 30 minutes between 04:00 and 08:00 starting on 01/01/2021 to 31/12/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = CurrentDateEx,
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Weekly,
                OcurrencyPeriod = 1,
                WeeklyDays = new List<DayOfWeek> { DayOfWeek.Sunday },
                DailyFrecuency = DailyFrecuencyEnum.Minutes,
                DailyFrecuencyPeriod = 30,
                DailyLimits = new HourLimitsConfig(new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0)),
                DateLimits = new DateLimitsConfig(ExecScheduleLimitStart, ExecScheduleLimitEnd)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, ScheduleDateEx);
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 3, 4, 30, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }


        #endregion

        #region Monthly pt3
        [Fact]
        public void Recurring_Monthly_DaySelection_Null_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDay = 8,
                DailyScheduleHour = new TimeSpan(12,0,0),
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcMonthlyTypeConfig);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_Monthly_DaySelection_False_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = false,
                MonthlyDay = 8,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcEnumError, nameof(schedulerConfig.MonthlyFrecuency)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_Monthly_DaySelection_True_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = true,
                MonthlyFrecuency = MonthlyFrecuencyEnum.First,
                MonthlyWeekday = MonthlyDayEnum.Day,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcMonthlyDay);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_Monthly_Day_Negative_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = true,
                MonthlyDay = -3,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcMonthlyDay);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_Monthly_Day_UpTo31_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = true,
                MonthlyDay = 45,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, TextResources.ExcMonthlyDay);
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_Monthly_DaySelection_False_Without_Frecuency_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = false,
                MonthlyWeekday= MonthlyDayEnum.Day,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcEnumError, nameof(schedulerConfig.MonthlyFrecuency)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_Monthly_DaySelection_False_Without_Weekday_Failed()
        {
            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = false,
                MonthlyFrecuency = MonthlyFrecuencyEnum.Third,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
            };

            string expectedExcMsg = string.Format(TextResources.ConfError, string.Format(TextResources.ExcEnumError, nameof(schedulerConfig.MonthlyWeekday)));
            var exception = Assert.Throws<ValidationException>(() =>
               Scheduler.GetNextExecution(schedulerConfig));
            Assert.Equal(expectedExcMsg, exception.Message);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_Day_Of_Month_Correct()
        {
            string ExecDescription = "Occurs the day 8 of every 3 months every 2 hours between 10:00 and 12:00 starting on 01/01/2021 to 31/12/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = true,
                MonthlyDay = 8,
                DailyFrecuency = DailyFrecuencyEnum.Hours,
                DailyFrecuencyPeriod = 2,
                DailyLimits = new HourLimitsConfig(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),
                DateLimits = new DateLimitsConfig(new DateTime(2021, 1, 1), new DateTime(2021, 12, 31))
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 8, 10, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 8, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 4, 8, 10, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_Day_Of_Month_31_Correct()
        {
            string ExecDescription = "Occurs the day 31 of every 1 months every 2 hours between 10:00 and 12:00 starting on 01/01/2021 to 31/12/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 1,
                MonthlyDaySelection = true,
                MonthlyDay = 31,
                DailyFrecuency = DailyFrecuencyEnum.Hours,
                DailyFrecuencyPeriod = 2,
                DailyLimits = new HourLimitsConfig(new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),
                DateLimits = new DateLimitsConfig(new DateTime(2021, 1, 1), new DateTime(2021, 12, 31))
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 31, 10, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 31, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 2, 28, 10, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 2, 28, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 3, 31, 10, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 3, 31, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 4, 30, 10, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 4, 30, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_First_Correct()
        {
            string ExecDescription = "Occurs the First Monday of every 2 months every 1 hours between 05:00 and 07:00 starting on 01/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 2,
                MonthlyDaySelection = false,
                MonthlyFrecuency = MonthlyFrecuencyEnum.First,
                MonthlyWeekday = MonthlyDayEnum.Monday,
                DailyFrecuency = DailyFrecuencyEnum.Hours,
                DailyFrecuencyPeriod = 1,
                DailyLimits = new HourLimitsConfig(new TimeSpan(5, 0, 0), new TimeSpan(7, 0, 0)),
                DateLimits = new DateLimitsConfig(new DateTime(2021, 1, 1), null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 4, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 4, 6, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 4, 7, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 3, 1, 5, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_Second_Correct()
        {
            string ExecDescription = "Occurs the Second Weekday of every 1 months at 12:00 starting on 01/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 1,
                MonthlyDaySelection = false,
                MonthlyFrecuency = MonthlyFrecuencyEnum.Second,
                MonthlyWeekday = MonthlyDayEnum.Weekday,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
                DateLimits = new DateLimitsConfig(new DateTime(2021, 1, 1), null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 4, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 2, 2, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 3, 2, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_Third_Correct()
        {
            string ExecDescription = "Occurs the Third Wednesday of every 4 months at 12:00 starting on 01/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 4,
                MonthlyDaySelection = false,
                MonthlyFrecuency = MonthlyFrecuencyEnum.Third,
                MonthlyWeekday = MonthlyDayEnum.Wednesday,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
                DateLimits = new DateLimitsConfig(new DateTime(2021, 1, 1), null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 20, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 5, 19, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 9, 15, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_Fourth_Correct()
        {
            string ExecDescription = "Occurs the Fourth Day of every 6 months at 12:00 starting on 01/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 6,
                MonthlyDaySelection = false,
                MonthlyFrecuency = MonthlyFrecuencyEnum.Fourth,
                MonthlyWeekday = MonthlyDayEnum.Day,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
                DateLimits = new DateLimitsConfig(new DateTime(2021, 1, 1), null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 4, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 7, 4, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2022, 1, 4, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_Last_Correct()
        {
            string ExecDescription = "Occurs the Last Sunday of every 3 months at 12:00 starting on 01/03/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = false,
                MonthlyFrecuency = MonthlyFrecuencyEnum.Last,
                MonthlyWeekday = MonthlyDayEnum.Sunday,
                DailyScheduleHour = new TimeSpan(12, 0, 0),
                DateLimits = new DateLimitsConfig(new DateTime(2021, 3, 1), null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 3, 28, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 6, 27, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 9, 26, 12, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }

        [Fact]
        public void Recurring_Monthly_Next_Execution_WeekendDay_Correct()
        {
            string ExecDescription = "Occurs the Third WeekendDay of every 3 months every 30 minutes between 18:00 and 19:00 starting on 01/01/2021";

            SchedulerConfigurator schedulerConfig = new()
            {
                CurrentDate = new DateTime(2021, 1, 1),
                Type = ScheduleTypeEnum.Recurring,
                PeriodType = OccurrencyPeriodEnum.Monthly,
                OcurrencyPeriod = 3,
                MonthlyDaySelection = false,
                MonthlyFrecuency = MonthlyFrecuencyEnum.Third,
                MonthlyWeekday = MonthlyDayEnum.WeekendDay,
                DailyFrecuency = DailyFrecuencyEnum.Minutes,
                DailyFrecuencyPeriod = 30,
                DailyLimits = new HourLimitsConfig(new TimeSpan(18,0,0), new TimeSpan(19,0,0)),
                DateLimits = new DateLimitsConfig(new DateTime(2021, 1, 1), null)
            };
            ScheduleEvent NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 9, 18, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 9, 18, 30, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 1, 9, 19, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 4, 10, 18, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 4, 10, 18, 30, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 4, 10, 19, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 7, 10, 18, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 7, 10, 18, 30, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);

            NextExec = Scheduler.GetNextExecution(schedulerConfig);

            Assert.Equal(NextExec.ExecutionDate, new DateTime(2021, 7, 10, 19, 0, 0));
            Assert.Equal(NextExec.ExecutionDescription, ExecDescription);
        }
        #endregion

        #endregion
    }
}

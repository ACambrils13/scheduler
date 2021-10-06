using System;

namespace Scheduler
{
    public abstract class ScheduleConfig
    {
        public ScheduleConfig(DateTime CurrentDate, ScheduleType Type, LimitsConfig Limits) 
        {
            this.CurrentDate = CurrentDate;
            this.Type = Type;
            this.Limits = Limits;
        }

        public abstract ScheduleEvent ScheduleNextExecution();

        internal DateTime CurrentDate { get; private set; }
        internal ScheduleType Type { get; private set; }
        internal LimitsConfig Limits { get; private set; }
        internal DateTime ScheduleDate { get; set; }
        internal OccurrencyPeriod PeriodType { get; set; }
        internal int Period { get; set; }
    }

    public class ScheduleConfigOnce : ScheduleConfig
    {

        public ScheduleConfigOnce(DateTime CurrentDate, ScheduleType Type, LimitsConfig Limits, DateTime? ExecutionDate)
            : base(CurrentDate, Type, Limits) 
        {
            Auxiliary.CheckNotNull(new object[] { ExecutionDate });
            this.ScheduleDate = ExecutionDate.Value;
        }

        public override ScheduleEvent ScheduleNextExecution()
        {
            return new ScheduleEvent(this.ScheduleDate, this.Type, this.Limits);
        }
    }

    public class ScheduleConfigRecurring : ScheduleConfig
    {
        public ScheduleConfigRecurring(DateTime CurrentDate, ScheduleType Type, LimitsConfig Limits, OccurrencyPeriod? PeriodType, int? Period)
            : base(CurrentDate, Type, Limits)
        {
            Auxiliary.CheckNotNull(new object[] { Period, PeriodType });
            this.PeriodType = PeriodType.Value;
            this.Period = Period.Value;
        }

        public override ScheduleEvent ScheduleNextExecution()
        {
            switch (this.PeriodType)
            {
                case OccurrencyPeriod.Daily:
                    this.ScheduleDate = this.CurrentDate.AddDays(this.Period);
                    break;
                case OccurrencyPeriod.Monthly:
                    this.ScheduleDate = this.CurrentDate.AddMonths(this.Period);
                    break;
                case OccurrencyPeriod.Weekly:
                    this.ScheduleDate = this.CurrentDate.AddDays(this.Period * 7);
                    break;
                case OccurrencyPeriod.Yearly:
                    this.ScheduleDate = this.CurrentDate.AddYears(this.Period);
                    break;
            }
            return new ScheduleEvent(this.ScheduleDate, this.Type, this.Limits);
        }
    }
}

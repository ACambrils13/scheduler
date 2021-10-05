using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public abstract class ScheduleConfig
    {
        public ScheduleConfig(DateTime CurrentDate, ScheduleType Type, LimitsConfig Limits) 
        {
            Auxiliar.ValidateDate(CurrentDate);
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

        public ScheduleConfigOnce(DateTime CurrentDate, ScheduleType Type, LimitsConfig Limits, DateTime ExecutionDate)
            : base(CurrentDate, Type, Limits) 
        {
            Auxiliar.ValidateDate(ExecutionDate);
            this.ScheduleDate = ExecutionDate;
        }

        public override ScheduleEvent ScheduleNextExecution()
        {
            return new ScheduleEvent(this.ScheduleDate, this.Type, this.Limits);
        }
    }

    public class ScheduleConfigRecurring : ScheduleConfig
    {
        public ScheduleConfigRecurring(DateTime CurrentDate, ScheduleType Type, LimitsConfig Limits, OccurrencyPeriod PeriodType, int Period)
            : base(CurrentDate, Type, Limits)
        {
            this.PeriodType = PeriodType;
            this.Period = Period;
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

    public class Scheduler
    {
        private ScheduleConfig Configurator;

        public Scheduler() { }

        public void SetConfig (DateTime CurrentDate, ScheduleType Type, DateTime ExecutionDate, OccurrencyPeriod PeriodType, int Period, DateTime Start, DateTime End)
        {
            LimitsConfig Limits = new LimitsConfig(Start, End);
            switch (Type)
            {
                case ScheduleType.Once:
                    this.Configurator = new ScheduleConfigOnce(CurrentDate, Type, Limits, ExecutionDate);
                    break;
                case ScheduleType.Recurring:
                    this.Configurator = new ScheduleConfigRecurring(CurrentDate, Type, Limits, PeriodType, Period);
                    break;
            }
        }
    }

    public class LimitsConfig
    {
        public LimitsConfig(DateTime Start, DateTime End)
        {
            this.StartDate = Start;
            this.EndDate = End;
        }

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
    }

    public enum ScheduleType
    {
        Once,
        Recurring
    }

    public enum OccurrencyPeriod
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}

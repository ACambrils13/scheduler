using System;

namespace Scheduler
{
    public class ScheduleConfigurator
    {
        public ScheduleConfigurator() { }

        public DateTime CurrentDate { get; set; }
        public ScheduleTypeEnum Type { get; set; }
        public LimitsConfig DateLimits { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? ScheduleDateHour { get; set; }
        public OccurrencyPeriodEnum? PeriodType { get; set; }
        public int? OcurrencyPeriod { get; set; }
        public Week? WeeklyDays { get; set; }
        public DailyFrecuencyEnum? DailyFrecuency { get; set; }
        public int? DailyFrecuencyPeriod { get; set; }
        public LimitsConfig? DailyLimits { get; set; }
        public DateTime ExecutionDate { get; private set; }
        public string ExecutionDescripcion { get; private set; }

        public 

        private void FormatEventDescription(ScheduleTypeEnum ExecutionType, DateTime? StartLimit)
        {
            string ExecutionDate = this.ExecutionTime.ToString("d");
            string ExecutionHour = this.ExecutionTime.ToString("t");
            string StartDate = StartLimit?.ToString("d") ?? TextResources.NotDefined;

            switch (ExecutionType)
            {
                case ScheduleTypeEnum.Once:
                    this.ExecutionDescription = string.Format(TextResources.EventScheduleDescOnce, ExecutionDate, ExecutionHour, StartDate);
                    break;
                case ScheduleTypeEnum.Recurring:
                    this.ExecutionDescription = string.Format(TextResources.EventScheduleDescRecurring, ExecutionDate, ExecutionHour, StartDate);
                    break;
            }
        }
    }

    public class ScheduleConfigOnce : ScheduleConfig
    {

        public ScheduleConfigOnce(DateTime CurrentDate, ScheduleTypeEnum Type, LimitsConfig Limits, DateTime? ExecutionDate)
            : base(CurrentDate, Type, Limits) 
        {
            Auxiliary.CheckNotNull(new object[] { ExecutionDate });
            this.ScheduleDate = ExecutionDate.Value;
        }

        public override ScheduleEvent ScheduleNextExecution()
        {
            return new ScheduleEvent(this.ScheduleDate, this.Type, this.DateLimits);
        }
    }

    public class ScheduleConfigRecurring : ScheduleConfig
    {
        public ScheduleConfigRecurring(DateTime CurrentDate, ScheduleTypeEnum Type, LimitsConfig Limits, OccurrencyPeriodEnum? PeriodType, int? Period)
            : base(CurrentDate, Type, Limits)
        {
            Auxiliary.CheckNotNull(new object[] { Period, PeriodType });
            this.PeriodType = PeriodType.Value;
            this.OcurrencyPeriod = Period.Value;
        }

        public override ScheduleEvent ScheduleNextExecution()
        {
            switch (this.PeriodType)
            {
                case OccurrencyPeriodEnum.Daily:
                    this.ScheduleDate = this.CurrentDate.AddDays(this.OcurrencyPeriod);
                    break;
                case OccurrencyPeriodEnum.Monthly:
                    this.ScheduleDate = this.CurrentDate.AddMonths(this.OcurrencyPeriod);
                    break;
                case OccurrencyPeriodEnum.Weekly:
                    this.ScheduleDate = this.CurrentDate.AddDays(this.OcurrencyPeriod * 7);
                    break;
                case OccurrencyPeriodEnum.Yearly:
                    this.ScheduleDate = this.CurrentDate.AddYears(this.OcurrencyPeriod);
                    break;
            }
            return new ScheduleEvent(this.ScheduleDate, this.Type, this.DateLimits);
        }
    }
}

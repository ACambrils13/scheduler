using Scheduler.Resources;
using System;

namespace Scheduler
{
    public class ScheduleEvent
    {
        private readonly ScheduleConfigurator configurator;
        public ScheduleEvent(ScheduleConfigurator Configurator)
        {
            this.configurator = Configurator;
        }

        public DateTime ExecutionDate => this.configurator.ExecutionDate;
        public string ExecutionDescription => this.configurator.ExecutionDescripcion;

    }
}

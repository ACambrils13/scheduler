using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Creators
{
    internal abstract class ScheduleEventCreator
    {
        internal readonly Scheduler configuration;

        internal ScheduleEventCreator(Scheduler scheduler)
        {
            this.configuration = scheduler;
        }

        internal abstract ScheduleEvent GetNextExecution();
    }
}

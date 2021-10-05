using Scheduler.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class Auxiliar
    {
        public static void ValidateDate(DateTime Date)
        {
            if (string.IsNullOrWhiteSpace(Date.ToString()))
            {
                throw new Exception(TextResources.ExcDate);
            }
        }
    }
}

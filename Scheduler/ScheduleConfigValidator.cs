using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    public class ScheduleConfigValidator
    {
        public static void ValidateDate (DateTime Date, string PropertyName)
        {
            if (Date == DateTime.MaxValue)
            {
                throw new ValidationException("propertyname maxvalue");
            }
        }

        public static void ValidateDateNullable (DateTime? Date, string PropertyName)
        {
            if (Date.HasValue == false)
            {
                throw new ValidationException("propertyname nulo");
            }
            else
            {
                ScheduleConfigValidator.ValidateDate(Date.Value, PropertyName);
            }
        }

        public static void ValidateLimits(DateTime? Start, DateTime? End)
        {
            if (Start.HasValue && End.HasValue)
            {
                ScheduleConfigValidator.ValidateDate(Start.Value, nameof(Start));
                ScheduleConfigValidator.ValidateDate(End.Value, nameof(End));
                if (DateTime.Compare(Start.Value, End.Value) < 0)
                {
                    throw new ValidationException("End mayor que start");
                }
            } 
            else if (Start.HasValue == false && End.HasValue)
            {
                throw new ValidationException("End sin Start");
            }
        }
    }
}

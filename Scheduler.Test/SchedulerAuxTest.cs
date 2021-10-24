using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scheduler.Test
{
    public class SchedulerAuxTest
    {
        public static IEnumerable<object[]> ScheduleTypeEnumValues()
        { 
            foreach (var code in Enum.GetValues(typeof(ScheduleTypeEnum)))
            {
                yield return new object[] { code };
            }
        }

        [Theory]
        [MemberData(nameof(ScheduleTypeEnumValues))]
        public void ScheduleType_Correct(ScheduleTypeEnum Type)
        {
            Assert.IsType<ScheduleTypeEnum>(Type);
        }
    }
}

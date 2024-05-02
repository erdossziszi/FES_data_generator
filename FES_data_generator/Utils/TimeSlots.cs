using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Utils
{
    internal class TimeSlots
    {
        public TimeSlots(int slotsPerDay, int[] availabiltyTs, int minExamLen, int maxExamLen)
        {
            SlotsPerDayOpt = slotsPerDay;
            AvailabilityTsLenOpt = availabiltyTs;
            minExamLenOpt = minExamLen;
            maxExamLenOpt = maxExamLen;            
        }
        public int SlotsPerDayOpt { get; set; }
        public int[] AvailabilityTsLenOpt { get; set; }
        public int minExamLenOpt { get; set; }
        public int maxExamLenOpt { get; set; }

    }
}

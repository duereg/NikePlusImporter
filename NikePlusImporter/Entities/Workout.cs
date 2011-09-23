using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Import.NikePlus.Entities
{
    public class Workout
    {
        public long ID { get; set; }
        public DateTime ImportedOn { get; set; }
        public String Name { get; set; }
        public DateTime EventDate { get; set; }
        public float Distance { get; set; }
        public int Duration { get; set; }
        public float Calories { get; set; }
        public String Comments { get; set; }

        public short HeartRateAvg { get; set; }
        public short HeartRateMin { get; set; }
        public short HeartRateMax { get; set; }
    }
}

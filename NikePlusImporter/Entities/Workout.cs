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

        public IEnumerable<SnapShot> Snapshots { get; set; }

        public class SnapShot
        {
            //private IList<float> _intervals = new List<float>();
            public int Interval { get; set; }
            public string DataType { get; set; }
            public string IntervalType { get; set; }
            public string IntervalUnit { get; set; }

            public IEnumerable<float> Intervals { get; set; } //{ return _intervals; } }
        }
    }
}

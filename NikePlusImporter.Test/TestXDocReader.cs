using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Import.NikePlus.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TestXDocReader : IXDocReader
    {
        public XDocument WorkoutXml { get; set; }
        public XDocument WorkoutSummaryXml { get; set; }

        /// <summary>
        /// Get a workout based off of the workout ID.
        /// These workouts include detailed interval / mileage /pace information.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public XDocument GetWorkout(long id)
        {
            return WorkoutXml;
        }

        /// <summary>
        /// Retrieve workout summaries.
        /// They have the basic workout information but none of detailed interval / mileage /pace information included.
        /// </summary>
        /// <returns>
        /// A list of workout summaries
        /// </returns>
        public XDocument GetWorkoutSummaries()
        {
            return WorkoutSummaryXml;
        }
    }
}

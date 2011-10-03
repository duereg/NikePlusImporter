using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Import.NikePlus
{
    /// <summary>
    /// 
    /// </summary>
    public interface IXDocReader
    {
        /// <summary>
        /// Get a workout based off of the workout ID. 
        /// These workouts include detailed interval / mileage /pace information.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        XDocument GetWorkout(long id);
        /// <summary>
        /// Retrieve workout summaries. 
        /// They have the basic workout information but none of detailed interval / mileage /pace information included.
        /// </summary>
        /// <returns>A list of workout summaries</returns>
        XDocument GetWorkoutSummaries();
    }
}

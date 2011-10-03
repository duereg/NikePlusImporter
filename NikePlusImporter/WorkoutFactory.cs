using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using Import.NikePlus.Entities;
using Import.NikePlus.Exceptions;

namespace Import.NikePlus
{
    /// <summary>
    /// Convert NikePlus xml into a usable, strongly typed class structure
    /// </summary>
    public class WorkoutFactory 
    {
        private IXDocReader _reader = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkoutFactory"/> class.
        /// </summary>
        /// <param name="reader">The reader to retrieve the XDocs needed by the factory.</param>
        public WorkoutFactory(IXDocReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "The given XDoc reader is invalid.");
            _reader = reader;
        }

        /// <summary>
        /// Gets a list of all the the workout IDs in the system.
        /// </summary>
        /// <returns>
        /// a list of all the workout IDS in the system
        /// </returns>
        public IEnumerable<long> GetIDs()
        {
            return
                from a in GetSummaries()
                select a.ID;
        }

        /// <summary>
        /// Retrieve a list of workouts containing detailed information about the workout.
        /// These workouts include detailed interval / mileage / pace information.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Workout> GetPopulated()
        {
            IEnumerable<long> workoutKeys = GetIDs();
            foreach (var key in workoutKeys)
            {
                yield return GetWorkout(key);
            }
        }

        /// <summary>
        /// Gets a populated workout based off of its ID.
        /// This workout includes detailed interval / mileage / pace information.
        /// </summary>
        /// <param name="id">The ID of the workout.</param>
        /// <returns>
        /// A Populated Workout
        /// </returns>
        public Workout GetWorkout(long id)
        {
            var fileContents = _reader.GetWorkout(id); 
            return CreatePopulatedWorkout(fileContents);
        }

        /// <summary>
        ///Retrieve a collection of workout summaries.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Workout> GetSummaries()
        {
            return CreateSimpleWorkouts(_reader.GetWorkoutSummaries()); 
        }

        private Workout CreatePopulatedWorkout(XDocument nikePlusDoc)
        {
            ValidateStatus(nikePlusDoc);

            Workout work = new Workout{ImportedOn = DateTime.Now};

            var baseElement = nikePlusDoc.Root.Element("sportsData");

            if(baseElement != null){

                work.EventDate = (DateTime)baseElement.Element("startTime");
                work.Comments = baseElement.Element("description").Value;
                work.Name = baseElement.Element("name").Value;

                var run = baseElement.Element("runSummary");
                if(run != null){
                    work.Calories = (float)run.Element("calories");
                    work.Duration = (int)run.Element("duration");
                    work.Distance = (float)run.Element("distance");

                    var heartrate = run.Element("heartrate");

                    if (heartrate != null)
                    {
                        work.HeartRateAvg = (short)heartrate.Element("average");
                        work.HeartRateMax = (short)heartrate.Element("maximum").Element("bpm");
                        work.HeartRateMin = (short)heartrate.Element("minimum").Element("bpm");
                    } 
                }

                var extendedData = baseElement.Element("extendedDataList");
                if (extendedData != null)
                {
                    work.Snapshots = from n in extendedData.Elements("extendedData")
                                     select new Workout.SnapShot
                                     {
                                         DataType = n.Attribute("dataType").Value,
                                         Interval = (int) n.Attribute("intervalValue"),
                                         IntervalType = n.Attribute("intervalType").Value,
                                         IntervalUnit = n.Attribute("intervalUnit").Value,
                                         Intervals = n.Value.Split(',').Select(p => Convert.ToSingle(p.Trim()))
                                     };
                }
            }

            return work;
        }
         
        private  IEnumerable<Workout> CreateSimpleWorkouts(XDocument nikePlusDoc)
        {
            ValidateStatus(nikePlusDoc);

            IList<Workout> workouts = new List<Workout>();

            foreach (var run in nikePlusDoc.Descendants("run"))
            {
                var workout = new Workout{ Calories= (float) run.Element("calories"), 
                                Duration= (int)run.Element("duration"), 
                                Distance= (float) run.Element("distance"),  
                                EventDate = (DateTime) run.Element("startTime"),
                                ID = (long) run.Attribute("id"),
                                ImportedOn = DateTime.Now, 
                                Comments= run.Element("description").Value,
                                Name = run.Element("name").Value};

                var heartrate = run.Element("heartrate");
            
                if(heartrate != null) {
                    workout.HeartRateAvg = (short) heartrate.Element("average");
                    workout.HeartRateMax = (short) heartrate.Element("maximum");
                    workout.HeartRateMin = (short) heartrate.Element("minimum");
                }

                workouts.Add(workout);
            }


            return workouts;
        }

        /// <summary>
        /// Checks the status of the xml document contains "success". Throws exception with given error message if not true. 
        /// </summary>
        /// <param name="xmlDoc">Xml Document to examine</param>
        /// <param name="errorMessage">Error message to throw in case of error</param>
        private void ValidateStatus(XDocument xmlDoc )
        {
            Contract.Requires<ArgumentNullException>(xmlDoc != null, "The given XML document is invalid.");
            Contract.Requires<ArgumentException>(xmlDoc.Root != null , "The given XML document is empty.");
            Contract.Requires<NikePlusException>(!xmlDoc.ToString().Contains("site_outage_plus"), "The Nike+ site is down. Please try to login at a later time.");

            if(!xmlDoc.Root.Element("status").Equals("success")){
                var element = xmlDoc.Root.Element("serviceException");
                if((element != null) && !string.IsNullOrWhiteSpace(element.Value )){
                    throw new NikePlusException(element.Value);
                }
            }
        }
    }
}

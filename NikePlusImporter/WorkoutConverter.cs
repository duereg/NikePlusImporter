using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using Import.NikePlus.Entities;

namespace Import.NikePlus
{
    /// <summary>
    /// Convert NikePlus xml into a usable, strongly typed class structure
    /// </summary>
    public class WorkoutConverter
    {    
        protected string FileContents{ get; set; }
        protected XDocument NikePlusDoc { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileImporter"/> class.
        /// </summary>
        /// <param name="fileContents">The file contents to import into the system.</param>
        public WorkoutConverter(string fileContents)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(fileContents), "The contents of the file are invalid.");
            
            FileContents = fileContents;
            NikePlusDoc = XDocument.Parse(fileContents);
            ValidateStatus(NikePlusDoc);
        }

        public WorkoutConverter(XDocument nikePlusDoc)
        {
            Contract.Assert(nikePlusDoc != null);
            FileContents = nikePlusDoc.ToString();
            NikePlusDoc = nikePlusDoc;
            ValidateStatus(nikePlusDoc);
        }

        public IEnumerable<Workout> Get()
        {
            if (NikePlusDoc.Descendants("run") != null)
            {
                return GetSimple();
            }
            else
            {
                var workouts =new List<Workout>();
                workouts.Add(GetPopulated());
                return workouts;
            }
        }

        public IEnumerable<Workout> GetSimple()
        {
            return CreateSimpleWorkouts(); 
        }

        /// <summary>
        /// Converts the given file contents into a <see cref="Import.NikePlus.Entities.Workout"/> instance.
        /// </summary>
        /// <returns>a <see cref="Import.NikePlus.Entities.Workout"/> instance.</returns>
        public Workout GetPopulated()
        {
            return CreatePopulatedWorkout(); 
        }

        private Workout CreatePopulatedWorkout()
        {
            Workout work = new Workout{ImportedOn = DateTime.Now};

            var baseElement = NikePlusDoc.Root.Element("sportsData");

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
         
        private  IEnumerable<Workout> CreateSimpleWorkouts()
        {
            IList<Workout> workouts = new List<Workout>(); 

            foreach(var run in NikePlusDoc.Descendants("run")) {
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
            Contract.Assert(xmlDoc != null, "The given XML document is invalid");
            Contract.Assert(!xmlDoc.ToString().Contains("site_outage_plus"), "The Nike+ site is down. Please try to login at a later time.");

            if(!xmlDoc.Root.Element("status").Equals("success")){
                var element = xmlDoc.Root.Element("serviceException");
                if((element != null) && !string.IsNullOrWhiteSpace(element.Value )){
                    throw new InvalidOperationException(element.Value);
                }
            }
        }
    }
}

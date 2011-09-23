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
        }

        public WorkoutConverter(XDocument nikePlusDoc)
        {
            Contract.Assert(nikePlusDoc != null);
            FileContents = nikePlusDoc.ToString();
            NikePlusDoc = nikePlusDoc;
        }

        public IEnumerable<Workout> GetSimple()
        {
            return CreateSimpleWorkouts(); 
        }

        /// <summary>
        /// Converts the given file contents into a <see cref="Import.NikePlus.Entities.Workout"/> instance.
        /// </summary>
        /// <returns>a <see cref="Import.NikePlus.Entities.Workout"/> instance.</returns>
        public IEnumerable<Workout> GetPopulated()
        {
            return CreatePopulatedWorkouts(); 
        }

        private IEnumerable<Workout> CreatePopulatedWorkouts() { return null; }


        private  IEnumerable<Workout> CreateSimpleWorkouts()
        {
            IEnumerable<Workout> workouts = null; 

            workouts =
            from n in NikePlusDoc.Descendants("run")
            select new Workout{ Calories= (float) n.Element("calories"), 
                                Duration= (int)n.Element("duration"), 
                                Distance= (float) n.Element("distance"),  
                                EventDate = (DateTime) n.Element("startTime"),
                                ID = (long) n.Attribute("id"),
                                ImportedOn = DateTime.Now, 
                                Comments= n.Element("description").Value,
                                Name = n.Element("name").Value,
                                HeartRateAvg = (short) n.Element("heartrate").Element("average"),
                                HeartRateMax = (short) n.Element("heartrate").Element("maximum"),
                                HeartRateMin = (short) n.Element("heartrate").Element("minimum")
            };

            return workouts;
        }
    }
}

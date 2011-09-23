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
            select new Workout{ Calories= Convert.ToInt16 ( n.Element("calories").Value), 
                                Duration= Convert.ToInt32 ( n.Element("duration").Value), 
                                Distance= Convert.ToSingle( n.Element("distance").Value),  
                                EventDate = Convert.ToDateTime(n.Element("startTime").Value),
                                ID = Convert.ToInt64(n.Attribute("id").Value),
                                ImportedOn = DateTime.Now, 
                                Comments= n.Element("description").Value,
                                Name = n.Element("name").Value,
                                HeartRateAvg = Convert.ToInt16 ( n.Element("heartrate").Element("average").Value),
                                HeartRateMax = Convert.ToInt16 ( n.Element("heartrate").Element("maximum").Value),
                                HeartRateMin = Convert.ToInt16(n.Element("heartrate").Element("minimum").Value)
            };

            return workouts;
        }
    }
}

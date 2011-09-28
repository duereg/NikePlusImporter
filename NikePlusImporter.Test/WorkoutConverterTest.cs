using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Import.NikePlus.Entities;
using Import.NikePlus.Test.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Import.NikePlus.Exceptions;
using System;

namespace Import.NikePlus.Test
{
    /// <summary>
    ///This is a test class for WorkoutConverterTest and is intended
    ///to contain all WorkoutConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WorkoutConverterTest
    { 
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
 
        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Run_Summary_XDoc_WHEN_Creating_WorkoutConverter_THEN_No_Exception()
        {
            XDocument nikePlusDoc = XDocument.Parse(Settings.Default.GoodRunList); 
            WorkoutConverter target = new WorkoutConverter(nikePlusDoc); 
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Run_XDoc_WHEN_Creating_WorkoutConverter_THEN_No_Exception()
        {
            XDocument nikePlusDoc = XDocument.Parse(Settings.Default.GoodWorkoutXml);
            WorkoutConverter target = new WorkoutConverter(nikePlusDoc);
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Run_Summary_String_WHEN_Creating_WorkoutConverter_THEN_No_Exception()
        { 
            WorkoutConverter target = new WorkoutConverter(Settings.Default.GoodRunList);
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Run_String_WHEN_Creating_WorkoutConverter_THEN_No_Exception()
        { 
            WorkoutConverter target = new WorkoutConverter(Settings.Default.GoodWorkoutXml);
        }
         
        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GIVEN_Empty_String_WHEN_Creating_WorkoutConverter_THEN_Exception()
        {
            string fileContents = string.Empty; 
            WorkoutConverter target = new WorkoutConverter(fileContents); 
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GIVEN_Null_String_WHEN_Creating_WorkoutConverter_THEN_Exception()
        {
            string fileContents = null;
            WorkoutConverter target = new WorkoutConverter(fileContents);
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.Xml.XmlException))]
        public void GIVEN_Invalid_Xml_String_WHEN_Creating_WorkoutConverter_THEN_Exception()
        {
            string fileContents = "<someXML><that><is><not><valid>THIS_IS_NOT_VALID</valid></not></that>";
            WorkoutConverter target = new WorkoutConverter(fileContents);
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Valid_But_Incorrect_Run_String_WHEN_Creating_WorkoutConverter_THEN_Exception()
        {
            string fileContents = Settings.Default.BadWorkoutXml;
            WorkoutConverter target = new WorkoutConverter(fileContents);
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Valid_But_Incorrect_Summary_XDoc_WHEN_Creating_WorkoutConverter_THEN_Exception()
        {
            string fileContents = Settings.Default.BadRunList;
            WorkoutConverter target = new WorkoutConverter(XDocument.Parse(fileContents));
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Valid_But_Incorrect_Run_XDoc_WHEN_Creating_WorkoutConverter_THEN_Exception()
        {
            string fileContents = Settings.Default.BadWorkoutXml;
            WorkoutConverter target = new WorkoutConverter(XDocument.Parse(fileContents));
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Valid_But_Incorrect_Summary_String_WHEN_Creating_WorkoutConverter_THEN_Exception()
        {
            string fileContents = Settings.Default.BadRunList;
            WorkoutConverter target = new WorkoutConverter(fileContents);
        }

        /// <summary>
        ///A test for WorkoutConverter Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GIVEN_Invalid_XDoc_WHEN_Creating_WorkoutConverter_THEN_Exception()
        { 
            WorkoutConverter target = new WorkoutConverter(new XDocument());
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Run_XDoc_WHEN_Retrieving_Workouts_Via_Get_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            XDocument nikePlusDoc = XDocument.Parse(Settings.Default.GoodRunList);
            WorkoutConverter target = new WorkoutConverter(nikePlusDoc); 
             
            IEnumerable<Workout> actual = target.Get();

            Assert.IsNotNull(actual, "Invalid Workout List Returned");
            Assert.AreEqual(actual.Count(), 1, "No Workouts Returned");        
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Summary_XDoc_WHEN_Retrieving_Workouts_Via_Get_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            XDocument nikePlusDoc = XDocument.Parse(Settings.Default.GoodWorkoutXml);
            WorkoutConverter target = new WorkoutConverter(nikePlusDoc);

            IEnumerable<Workout> actual = target.Get();

            Assert.IsNotNull(actual, "Invalid Workout List Returned");
            Assert.AreEqual(actual.Count(), 1, "No Workouts Returned");
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Run_String_WHEN_Retrieving_Workouts_Via_Get_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            WorkoutConverter target = new WorkoutConverter(Settings.Default.GoodRunList);

            IEnumerable<Workout> actual = target.Get();

            Assert.IsNotNull(actual, "Invalid Workout List Returned");
            Assert.AreEqual(actual.Count(), 1, "No Workouts Returned");
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Summary_String_WHEN_Retrieving_Workouts_Via_Get_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            WorkoutConverter target = new WorkoutConverter(Settings.Default.GoodWorkoutXml);

            IEnumerable<Workout> actual = target.Get();

            Assert.IsNotNull(actual, "Invalid Workout List Returned");
            Assert.AreEqual(actual.Count(), 1, "No Workouts Returned");
        }

        /// <summary>
        ///A test for GetPopulated
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Workout_String_WHEN_Retrieving_Workouts_Via_GetPopulated_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            WorkoutConverter target = new WorkoutConverter(Settings.Default.GoodWorkoutXml); 
            Workout actual = target.GetPopulated();
            Assert.IsNotNull(actual, "GetPopulated() returned null."); 
        }

        /// <summary>
        ///A test for GetSimple
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Summary_String_WHEN_Retrieving_Workouts_Via_GetSummaries_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            WorkoutConverter target = new WorkoutConverter(Settings.Default.GoodRunList);
            IEnumerable<Workout> actual = target.GetSummaries();
            Assert.IsNotNull(actual);
            Assert.AreNotEqual(actual.Count(), 0);
        }

        /// <summary>
        ///A test for GetPopulated
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Workout_XDoc_WHEN_Retrieving_Workouts_Via_GetPopulated_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            WorkoutConverter target = new WorkoutConverter(XDocument.Parse(Settings.Default.GoodWorkoutXml));
            Workout actual = target.GetPopulated();
            Assert.IsNotNull(actual, "GetPopulated() returned null.");
        }

        /// <summary>
        ///A test for GetSimple
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Summary_XDoc_WHEN_Retrieving_Workouts_Via_GetSummaries_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            WorkoutConverter target = new WorkoutConverter(XDocument.Parse(Settings.Default.GoodRunList));
            IEnumerable<Workout> actual = target.GetSummaries();
            Assert.IsNotNull(actual);
            Assert.AreNotEqual(actual.Count(), 0);
        } 
    }
}

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
    ///This is a test class for WorkoutFactoryTest and is intended
    ///to contain all WorkoutFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WorkoutFactoryTest
    {
        private IXDocReader _validReader = null;
        private IXDocReader _invalidReader = null;
        private IXDocReader _emptyXDocReader = null;
        private IXDocReader _nullXDocReader = null; 

        [TestInitialize]
        public void Setup()
        {
            _validReader = new TestXDocReader();
            _invalidReader = new TestXDocReader();
            _emptyXDocReader = new TestXDocReader();
            _nullXDocReader = new TestXDocReader();

            ((TestXDocReader)_validReader).WorkoutXml = XDocument.Parse(Settings.Default.GoodWorkoutXml);
            ((TestXDocReader)_validReader).WorkoutSummaryXml = XDocument.Parse(Settings.Default.GoodRunList);

            ((TestXDocReader)_invalidReader).WorkoutXml = XDocument.Parse(Settings.Default.BadWorkoutXml);
            ((TestXDocReader)_invalidReader).WorkoutSummaryXml = XDocument.Parse(Settings.Default.BadRunList);

            ((TestXDocReader)_emptyXDocReader).WorkoutXml = new XDocument();
            ((TestXDocReader)_emptyXDocReader).WorkoutSummaryXml = XDocument.Parse(Settings.Default.EmptyRunList);
        }

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


        [TestMethod] 
        public void GIVEN_List_Of_IDs_WHEN_Retrieving_By_ID_THEN_Populated_Workout_Retrieved()
        {
            //Arrange
            var converter = new WorkoutFactory(_validReader);
            //Act
            var ids = converter.GetIDs();
            foreach (var id in ids)
            {
                var w = converter.GetWorkout(id);
                Assert.IsNotNull(w);
                Console.Out.WriteLine("{0} [{1}] Calories: {2} Distance: {3} Time: {4}", w.ID, w.Name, w.Calories, w.Distance, new TimeSpan(w.Duration));
                if (w.Snapshots != null)
                {
                    foreach (var ss in w.Snapshots)
                    {
                        Console.Out.WriteLine("{0} {1} {2} {3}", ss.DataType, ss.Interval, ss.IntervalType, ss.IntervalUnit);
                        if (ss.Intervals != null)
                        {
                            Console.Out.WriteLine("Num of Intervals: {0}", ss.Intervals.Count());
                        }
                    }
                }
            }

            //Assert
            Assert.IsNotNull(ids);
            Assert.AreNotEqual(ids.Count(), 0);
        }
        /// <summary>
        ///A test for WorkoutFactory Constructor
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Reader_WHEN_Creating_WorkoutFactory_THEN_No_Exception()
        {
            var target = new WorkoutFactory(_validReader); 
        }

        /// <summary>
        ///A test for WorkoutFactory Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GIVEN_Null_Reader_WHEN_Creating_WorkoutFactory_THEN_Exception()
        {
            var target = new WorkoutFactory(null);
        }

        /// <summary>
        ///A test for WorkoutFactory Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Reader_With_Incorrect_Run_WHEN_Retrieving_Workout_By_ID_THEN_Exception()
        {
            var goodFactory = new WorkoutFactory(_validReader);
            var badFactory = new WorkoutFactory(_invalidReader);

            foreach (var id in goodFactory.GetIDs())
            {
                badFactory.GetWorkout(id);
            }
        }

        /// <summary>
        ///A test for WorkoutFactory Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Valid_But_Incorrect_Summary_XDoc_WHEN_Retrieving_Summaries_THEN_Exception()
        {
            var badFactory = new WorkoutFactory(_invalidReader);
            badFactory.GetSummaries();
        }

        /// <summary>
        ///A test for WorkoutFactory Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GIVEN_Empty_Run_XDoc_WHEN_Retrieving_Workout_By_ID_THEN_Exception()
        {
            var badFactory = new WorkoutFactory(_emptyXDocReader);
            var goodFactory = new WorkoutFactory(_validReader);

            foreach (var id in goodFactory.GetIDs())
            {
                badFactory.GetWorkout(id);
            }
        }

        /// <summary>
        ///A test for WorkoutFactory Constructor
        ///</summary>
        [TestMethod()] 
        public void GIVEN_Valid_But_Empty_Summaries_WHEN_Retrieving_Workouts_THEN_No_Exception()
        {
            var badFactory = new WorkoutFactory(_emptyXDocReader);
            badFactory.GetSummaries();
        }

        [TestMethod] 
        public void GIVEN_Valid_Reader_WHEN_Retrieving_Workout_IDs_THEN_IDS_Retrieved()
        {
            //Arrange
            var goodFactory = new WorkoutFactory(_validReader); 
            //Act
            var ids = goodFactory.GetIDs();
            //Assert
            Assert.IsNotNull(ids);
            Assert.AreNotEqual(ids.Count(), 0);
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Run_XDoc_WHEN_Retrieving_Workouts_Via_Get_THEN_Workout_Returned()
        {
            var goodFactory = new WorkoutFactory(_validReader);

            var actual = goodFactory.GetIDs();
            foreach (var workoutID in actual)
            {
                var workCopy = goodFactory.GetWorkout(workoutID);

                //TODO: Validate values in returned entity
                Assert.IsNotNull(workCopy, "Invalid Workout Returned"); 
            }      
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        public void GIVEN_Valid_Summary_XDoc_WHEN_Retrieving_Workouts_Via_Get_THEN_Workout_Returned()
        {
            //TODO: Validate values in returned entity
            var goodFactory = new WorkoutFactory(_validReader);

            IEnumerable<Workout> actual = goodFactory.GetSummaries();

            Assert.IsNotNull(actual, "Invalid Workout List Returned");
            Assert.AreEqual(actual.Count(), 1, "No Workouts Returned");
        }

        [TestMethod]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Bad_Reader_WHEN_Retrieving_Workout_IDs_THEN_Exception()
        {
            //Arrange
            var badFactory = new WorkoutFactory(_invalidReader);
            //Act
            var ids = badFactory.GetIDs(); 
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Bad_Reader_WHEN_Retrieving_Workouts_Via_Get_THEN_Exception()
        {
            //Arrange
            var badFactory = new WorkoutFactory(_invalidReader);
            //Act
            var workCopy = badFactory.GetWorkout(1); 
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NikePlusException))]
        public void GIVEN_Bad_Reader_WHEN_Retrieving_Workouts_Summaries_THEN_Exception()
        {
            //Arrange
            var badFactory = new WorkoutFactory(_invalidReader);
            //Act
            IEnumerable<Workout> actual = badFactory.GetSummaries();
        } 

        [TestMethod]
        public void GIVEN_Empty_Reader_WHEN_Retrieving_Workout_IDs_THEN_No_IDS_Retrieved()
        {
            //Arrange
            var emptyFactory = new WorkoutFactory(_emptyXDocReader);
            //Act
            var ids = emptyFactory.GetIDs();
            //Assert
            Assert.IsNotNull(ids);
            Assert.AreEqual(ids.Count(), 0);
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GIVEN_Empty_Reader_WHEN_Retrieving_Workouts_Via_Get_THEN_Exception()
        {
            //Arrange
            var emptyFactory = new WorkoutFactory(_emptyXDocReader);

            //Act
            var workCopy = emptyFactory.GetWorkout(1);
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()] 
        public void GIVEN_Empty_Reader_WHEN_Retrieving_Workout_Summaries_THEN_No_Exception()
        {
            //Arrange
            var emptyFactory = new WorkoutFactory(_emptyXDocReader);
            //Act
            IEnumerable<Workout> actual = emptyFactory.GetSummaries();
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GIVEN_Null_Reader_WHEN_Retrieving_Workout_IDs_THEN_Exception()
        {
            //Arrange
            var nullFactory = new WorkoutFactory(_nullXDocReader);
            //Act
            var ids = nullFactory.GetIDs();
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GIVEN_Null_Reader_WHEN_Retrieving_Workouts_Via_Get_THEN_Exception()
        {
            var nullFactory = new WorkoutFactory(_nullXDocReader);

            var workCopy = nullFactory.GetWorkout(1);
        }

        /// <summary>
        ///A test for Get
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GIVEN_Null_Reader_WHEN_Retrieving_Workouts_Summaries_THEN_Exception()
        {
            //TODO: Validate values in returned entity
            var nullFactory = new WorkoutFactory(_nullXDocReader);

            IEnumerable<Workout> actual = nullFactory.GetSummaries();
        } 
    }
}

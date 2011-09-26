using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Import.NikePlus;
using Import.NikePlus.Test.Properties;
using System.Diagnostics.Contracts;
using NUnit.Framework; 

namespace Import.NikePlus.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass] 
    [TestFixture]
    public class WebConverterTest
    {
        public WebConverterTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public Microsoft.VisualStudio.TestTools.UnitTesting.TestContext TestContext
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [Test]
        public void GIVEN_Valid_Username_AND_Password_WHEN_Creating_Web_Converter_THEN_No_Exception()
        {
            var converter = new WebConverter(Settings.Default.Username, Settings.Default.Password);
        }

        [TestMethod]
        [Test] 
        [NUnit.Framework.Ignore]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Ignore]
        [NUnit.Framework.ExpectedException(typeof(InvalidOperationException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(InvalidOperationException))]
        public void GIVEN_Invalid_Username_AND_Password_WHEN_Creating_Web_Converter_THEN_Exception()
        {
            var converter = new WebConverter("FAKE_USER_NAME", "EVEN_FAKER_PASSWORD");
        }

        [TestMethod]
        [Test] 
        public void GIVEN_Valid_WebConverter_WHEN_Retrieving_Workout_IDs_THEN_IDS_Retrieved()
        {
            //Arrange
            var converter = new WebConverter(Settings.Default.Username, Settings.Default.Password);
            //Act
            var ids = converter.GetWorkoutIDs();
            //Assert
            Contract.Assert(ids != null);
            Contract.Assert(ids.Count() > 0);
        }

        [TestMethod]
        [Test] 
        public void GIVEN_Valid_WebConverter_WHEN_Retrieving_Workout_Summaries_THEN_Valid_Summaries_Retrieved()
        {
            //Arrange
            var converter = new WebConverter(Settings.Default.Username, Settings.Default.Password);
            //Act
            var workouts = converter.GetWorkoutSummaries();
            //Assert
            Contract.Assert(workouts != null);
            Contract.Assert(workouts.Count() > 0);
            foreach (var w in workouts)
            { 
                Console.Out.WriteLine("{0} [{1}] Calories: {2} Distance: {3} Time: {4}", w.ID, w.Name, w.Calories, w.Distance, new TimeSpan(w.Duration));
            }
        }

        [TestMethod]
        [Test] 
        public void GIVEN_List_Of_IDs_WHEN_Retrieving_By_ID_THEN_Populated_Workout_Retrieved()
        {
            //Arrange
            var converter = new WebConverter(Settings.Default.Username, Settings.Default.Password);
            //Act
            var ids = converter.GetWorkoutIDs();
            foreach(var id in ids){
                var w = converter.GetWorkout(id);
                Contract.Assert(w != null);
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
            Contract.Assert(ids != null);
            Contract.Assert(ids.Count() > 0);
        }

    }
}

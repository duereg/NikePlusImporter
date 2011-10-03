using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Import.NikePlus;
using Import.NikePlus.Test.Properties;
using System.Diagnostics.Contracts;
using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Import.NikePlus.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass] 
    [TestFixture]
    public class WebXDocReaderTest
    {
        public WebXDocReaderTest()
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
        [TestCategory("Integration")]
        [Category("Integration")]
        [Test]
        public void GIVEN_Valid_Username_AND_Password_WHEN_Creating_Web_Reader_THEN_No_Exception()
        {
            var converter = new WebXDocReader(Settings.Default.Username, Settings.Default.Password);
        }

        [TestMethod]
        [Test]
        [TestCategory("Integration")]
        [Category("Integration")]
        [NUnit.Framework.Ignore]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Ignore]
        [NUnit.Framework.ExpectedException(typeof(InvalidOperationException))]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(InvalidOperationException))]
        public void GIVEN_Invalid_Username_AND_Password_WHEN_Creating_Web_Reader_THEN_Exception()
        {
            var converter = new WebXDocReader("FAKE_USER_NAME", "EVEN_FAKER_PASSWORD");
        }

        [TestMethod]
        [Test]
        [TestCategory("Integration")]
        [Category("Integration")]
        public void GIVEN_Valid_Reader_WHEN_Retrieving_Workout_Summaries_THEN_Valid_Summaries_Retrieved()
        {
            //Arrange
            var converter = new WebXDocReader(Settings.Default.Username, Settings.Default.Password);
            //Act
            var workouts = converter.GetWorkoutSummaries();
            //Assert
            Assert.IsNotNull(workouts);
            Assert.IsNotNull(workouts.Elements());
            Assert.AreNotEqual(workouts.Elements(), 0);
        }

        [TestMethod]
        [Test]
        [TestCategory("Integration")]
        [Category("Integration")]
        public void GIVEN_Valid_Reader_WHEN_Retrieving_Workout_THEN_Valid_Workout_Retrieved()
        {
            //Arrange
            var converter = new WebXDocReader(Settings.Default.Username, Settings.Default.Password);
            //Act
            var workouts = converter.GetWorkoutSummaries();

            foreach (var run in workouts.Descendants("run"))
            {
                var workout = converter.GetWorkout((long)run.Attribute("id"));

                //Assert
                Assert.IsNotNull(workouts);
                Assert.IsNotNull(workouts.Elements());
                Assert.AreNotEqual(workouts.Elements(), 0);
            }
        }
    }
}

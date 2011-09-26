using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Import.NikePlus.Entities;
using Import.NikePlus.Properties;

namespace Import.NikePlus
{
    /// <summary>
    ///  
    /// </summary>
    public class WebConverter
    {
        private CookieCollection Cookies { get; set;}
        private static Settings Configuration = (Settings) Settings.Synchronized(Settings.Default);
        /// <summary>
        /// Initializes a new instance of the <see cref="WebConverter"/> class.
        /// </summary>
        public WebConverter(string username, string password)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(username), "username is invalid");
            Contract.Assert(!string.IsNullOrWhiteSpace(password), "password is invalid");
            Contract.Assert(!string.IsNullOrWhiteSpace(Configuration.AUTHENTICATION_URL), "url is invalid");

            Cookies = Authenticate(username, password);
        }

        /// <summary>
        /// Converts the specified information into a list of workouts.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public IEnumerable<Workout> GetPopulatedWorkouts()
        {
            IEnumerable<long> workoutKeys = GetWorkoutIDs();
            foreach (var key in workoutKeys)
            {
                yield return GetWorkout(key);
            }
        }

        /// <summary>
        /// Gets a populated workout based off of its ID.
        /// </summary>
        /// <param name="id">The ID of the workout.</param>
        /// <returns>A Populated Workout</returns>
        public Workout GetWorkout(long id)
        {
            var fileContents = GetWorkoutXml(id);
            var xmlConverter = new WorkoutConverter(fileContents);
            return xmlConverter.GetPopulated();
        }

        /// <summary>
        /// Gets the workout summaries. The have the basic workout information but none of detailed interval / mileage /pace information included.
        /// </summary>
        /// <returns>A list of workout summaries</returns>
        public IEnumerable<Workout> GetWorkoutSummaries()
        {
            var xmlDoc = GetWorkouts();
            var xmlConverter = new WorkoutConverter(xmlDoc);
            return xmlConverter.GetSimple();
        }

        public IEnumerable<long> GetWorkoutIDs()
        {
            Contract.Assert(Cookies != null, "Cookie Collection is invalid");

            var xmlDoc = GetWorkouts();
            var xmlConverter = new WorkoutConverter(xmlDoc);

            return
                from a in xmlConverter.GetSimple()
                select a.ID;
        }

        private XDocument GetWorkouts()
        { 
            var request = WebRequest.Create(Configuration.GET_WORKOUTS_LIST_URL) as HttpWebRequest;
            var container = new CookieContainer();
            container.Add(Cookies);
            request.CookieContainer = container;
            return GetXDocument(request);
        }

        private CookieCollection Authenticate(string userName, string password)
        {
            CookieCollection retCookies = null;

            var url = String.Format(Configuration.AUTHENTICATION_URL, userName, password);
            var request = WebRequest.Create(url) as HttpWebRequest;

            request.CookieContainer = new CookieContainer(); 

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                ValidateResponse(response);
                retCookies = response.Cookies;
            }

            return retCookies;
        }

        private XDocument GetWorkoutXml(long key)
        {
            var url = string.Format(Configuration.GET_WORKOUT_DETAILS_URL, key);
            var request = WebRequest.Create(url) as HttpWebRequest;
            var container = new CookieContainer();
            container.Add(Cookies);
            request.CookieContainer = container;
            return GetXDocument(request);
        }
         
        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private XDocument GetXDocument(HttpWebResponse response)
        {
            XDocument xmlDoc = null;

            using (Stream receiveStream = response.GetResponseStream())
            {
                xmlDoc = XDocument.Load(receiveStream);
            }

            return xmlDoc;
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private XDocument GetXDocument(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return GetXDocument(response);
            }
        }

        ///<summary>Analyze the answer sent by the Nike+ website</summary> 
        ///<example>
        /// The XML answer has the following format:
        ///   <plusService>
        ///     <status>failure</status>
        ///     <serviceException errorCode="AuthenticationError">login failed</serviceException>
        ///   </plusService>
        ///</example>  
        /// <param name="response">The Response from the Nike+ website</param>
        private void ValidateResponse(HttpWebResponse response)
        {
            var xmlDoc = GetXDocument(response); 
            Contract.Assert(response.Cookies.Count > 0, "Could not login to Nike+ website with the given credentials");
        }
    }
}

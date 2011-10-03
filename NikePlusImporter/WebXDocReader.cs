using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Import.NikePlus.Entities;
using Import.NikePlus.Properties;
using Import.NikePlus.Exceptions;

namespace Import.NikePlus
{
    /// <summary>
    ///  
    /// </summary>
    public class WebXDocReader : IXDocReader
    {
        private CookieCollection Cookies { get; set;}
        internal static Settings Configuration = (Settings) Settings.Synchronized(Settings.Default);

        /// <summary>
        /// Initializes a new instance of the <see cref="WebXDocReader"/> class.
        /// </summary>
        public WebXDocReader(string username, string password)
        { 
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(username), "username is invalid");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(password), "password is invalid");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(Settings.Default.AUTHENTICATION_URL), "authentication url is invalid");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(Settings.Default.GET_WORKOUT_DETAILS_URL), "workout details url is invalid");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(Settings.Default.GET_WORKOUTS_LIST_URL), "workout list url is invalid");

            Cookies = Authenticate(username, password);
        }

        /// <summary>
        /// Retrieve workout summaries. 
        /// They have the basic workout information but none of detailed interval / mileage /pace information included.
        /// </summary>
        /// <returns>A list of workout summaries</returns>
        public XDocument GetWorkoutSummaries()
        { 
            var request = WebRequest.Create(Configuration.GET_WORKOUTS_LIST_URL) as HttpWebRequest;
            var container = new CookieContainer();
            container.Add(Cookies);
            request.CookieContainer = container;
            return GetXDocument(request);
        }

        /// <summary>
        /// Get a workout based off of the workout ID. 
        /// These workouts include detailed interval / mileage /pace information.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public XDocument GetWorkout(long key)
        {
            var url = string.Format(Configuration.GET_WORKOUT_DETAILS_URL, key);
            var request = WebRequest.Create(url) as HttpWebRequest;
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
            Contract.Requires<NikePlusException>(response.Cookies.Count > 0, "Could not login to Nike+ website with the given credentials");
        }
    }
}

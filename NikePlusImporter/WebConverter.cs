using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Import.NikePlus.Entities;
using System.Net;
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml;

namespace Import.NikePlus
{
    /// <summary>
    /// 
    /// </summary>
    public class WebConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebConverter"/> class.
        /// </summary>
        public WebConverter()
        { 

        }

        /// <summary>
        /// Converts the specified information into a list of workouts.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public IEnumerable<Workout> Convert(string username, string password)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(username), "username is invalid");
            Contract.Assert(!string.IsNullOrWhiteSpace(username), "password is invalid");
            Contract.Assert(!string.IsNullOrWhiteSpace(username), "url is invalid");

            var cookies = Authenticate(username, password);
            IEnumerable<long> workoutKeys = GetWorkouts(cookies);
            foreach (var key in workoutKeys)
            {
                var fileContents = GetWorkoutXml(cookies, key);
                var xmlConverter = new FileConverter(fileContents);
                yield return xmlConverter.Convert();
            }
        }

        private CookieCollection Authenticate(string userName, string password)
        {
            CookieCollection retCookies = null;

            var url = String.Format(GetUrl(Configuration.AUTHENTICATION_URL), userName, password);
            var request = WebRequest.Create(url) as HttpWebRequest;

            // Set some reasonable limits on resources used by this request
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            request.Credentials = CredentialCache.DefaultCredentials;

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                ValidateResponse(response);
                retCookies = response.Cookies;
            }

            return retCookies;
        }

        private IEnumerable<long> GetWorkouts(CookieCollection cookies)
        {
            Contract.Assert(cookies != null, "Cookie Collection is invalid");

            var request = WebRequest.Create(GetUrl(Configuration.GET_WORKOUTS_LIST_URL)) as HttpWebRequest ;
            var container = new CookieContainer();
            container.Add(cookies);
            request.CookieContainer = container;
            var xmlDoc = GetXmlDocument(request);

            return ProcessWorkouts(xmlDoc);
        }

        /// <summary>
        /// Return the workout ID's from the given xml document
        /// </summary>
        /// <param name="xmlDoc">The xml document with the workout ID's</param>
        /// <returns>a list of workout ID's</returns>
        /// <example>
        /// Analyze the answer sent by the NikePlus website
        /// The XML answer has the following format:
        ///   <plusService>
        ///     <status>success</status>
        ///     <runList>
        ///       <run id="753633197">
        ///         <startTime>2000-01-01T09:27:46-08:00</startTime>
        ///         <distance>8.7664</distance>
        ///         <duration>3318888</duration>
        ///         <syncTime>2007-10-28T16:52:40+00:00</syncTime>
        ///         <calories>568</calories>
        ///         <name><![CDATA[]]></name>
        ///         <description><![CDATA[]]></description>
        ///       </run>
        ///       <run>...</run>
        ///     </runList>
        ///   </plusService>
        /// </example>
        private IEnumerable<long> ProcessWorkouts(XmlDocument workouts)
        {
            ValidateStatus(workouts, "Could Not Retrieve A List of Workouts.");

            var nodes = workouts.SelectNodes("/plusService/runList/run");

            foreach (XmlNode node in nodes)
            {
                yield return long.Parse(node.Attributes["id"].Value);
            }
        }

        private string GetWorkoutXml(CookieCollection cookies, long key)
        {
            var url = GetUrl(string.Format(Configuration.GET_WORKOUT_DETAILS_URL, key));
            var request = WebRequest.Create(url) as HttpWebRequest;
            var container = new CookieContainer();
            container.Add(cookies);
            request.CookieContainer = container;
            var xmlDoc = GetXmlDocument(request);
            return xmlDoc.ToString();
        }
         
        private string GetUrl(string origUrl)
        {
            string url;
            if (origUrl.Contains('?'))
            {
                url = origUrl + "&";
            }
            else
            {
                url = origUrl + "?";
            }
            return url;
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private XmlDocument GetXmlDocument(HttpWebResponse response)
        {
            var xmlDoc = new XmlDocument();
            using (Stream receiveStream = response.GetResponseStream())
            {
                xmlDoc.Load(receiveStream);
            }
            return xmlDoc;
        }

        /// <summary>
        /// Gets the XML document.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private XmlDocument GetXmlDocument(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return GetXmlDocument(response);
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
            var xmlDoc = GetXmlDocument(response);
            ValidateStatus(xmlDoc, "Could not login to Nike+ website with the given credentials");
        }

        /// <summary>
        /// Checks the status of the xml document contains "success". Throws exception with given error message if not true. 
        /// </summary>
        /// <param name="xmlDoc">Xml Document to examine</param>
        /// <param name="errorMessage">Error message to throw in case of error</param>
        private void ValidateStatus(XmlDocument xmlDoc, string errorMessage){
            Validate(xmlDoc, "/plusService/status", "success", errorMessage);
        }

        private void Validate(XmlDocument xmlDoc, string xpath, string toCompare, string errorMessage)
        {
            var node = xmlDoc.SelectSingleNode(xpath); 
            if (node != null)
            {
                var value = node.Value;
                if (string.IsNullOrWhiteSpace(value) || !value.Equals(toCompare, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

    }
}

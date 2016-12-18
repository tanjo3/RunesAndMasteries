using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace RunesAndMasteries {

    /// <summary>
    /// Class used to make calls to a REST API.</summary>
    public class API {

        /// <summary>
        /// The REST API we're using.</summary>
        private const string API_URL = "http://api.champion.gg";

        /// <summary>
        /// Creates and sends a API call and returns the response as JSON.</summary>
        /// <param name="request">The GET request to send to the API.</param>
        /// <returns>The response as a JSON object.</returns>
        public static JToken MakeRequest(string request) {
            // create the web request
            HttpWebRequest httpRequest = WebRequest.Create(API_URL + request) as HttpWebRequest;

            // sends the request to the API
            using (HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse) {
                // on error, throw an exception
                if (response.StatusCode != HttpStatusCode.OK) {
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                }

                // attempt to read the response as a JSON object
                using (JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(response.GetResponseStream()))) {
                    return JToken.ReadFrom(jsonTextReader);
                }
            }
        }
    }
}

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
        /// The name of the API to use.</summary>
        public enum API_NAME {

            /// <summary>
            /// Champion.GG's API.</summary>
            CHAMPION_GG,

            /// <summary>
            /// Riot Games's API.</summary>
            RIOT_GAMES
        };

        /// <summary>
        /// The REST API we're using for Champion.GG.</summary>
        private const string CHAMPION_GG_API_URL = "http://api.champion.gg";

        /// <summary>
        /// The REST API we're using for Riot Games.</summary>
        private const string RIOT_GAMES_API_URL = "https://na1.api.riotgames.com";

        /// <summary>
        /// Creates and sends a API call and returns the response as JSON.</summary>
        /// <param name="api">The API to query.</param>
        /// <param name="request">The GET request to send to the API.</param>
        /// <returns>The response as a JSON object.</returns>
        public static JToken MakeRequest(API_NAME api, string request) {
            // create the web request
            HttpWebRequest httpRequest = null;
            switch (api) {
            case API_NAME.CHAMPION_GG:
                httpRequest = WebRequest.Create(CHAMPION_GG_API_URL + request) as HttpWebRequest;
                break;
            case API_NAME.RIOT_GAMES:
                httpRequest = WebRequest.Create(RIOT_GAMES_API_URL + request) as HttpWebRequest;
                break;
            }

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

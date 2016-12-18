using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RunesAndMasteries {

    public static class Program {

        public static bool QueryFrequent { get; set; } = true;

        public static bool QueryWinRate { get; set; } = false;

        /// <summary>
        /// The main entry point for the application.</summary>
        [STAThread]
        public static void Main() {

            // a list of all champion names mapped to their API key values
            SortedDictionary<string, string> allChampions;

            // setup application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try {
                // ensure API key is present
                string apiKey = Properties.Resources.ResourceManager.GetString("API_KEY");
                if (apiKey == null) {
                    throw new Exception("Error Locating API Key");
                }

                // generate a list of champion names
                allChampions = new SortedDictionary<string, string>();
                JToken response = API.MakeRequest("/champion?api_key=" + apiKey);
                foreach (JObject champion in (JArray) response) {
                    allChampions.Add(champion["name"].ToString(), champion["key"].ToString());
                }

                // run the rest of the program until user quits
                while (MainRunLoop(apiKey, allChampions)) { }
            } catch (Exception e) {
                MessageBox.Show("Error: " + e.Message, "An error has occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("An error occurred: '{0}'", e.Message);
            }
        }

        /// <summary>
        /// Runs the main execution loop for this program.</summary>
        /// <remarks>Asks the user which champions to look up and then processes and displays the result.</remarks>
        /// <param name="apiKey">The API key to make calls with.</param>
        /// <param name="championList">A map of champion name to champion key for all champions.</param>
        /// <returns>Whether the user has requested to run a new query.</returns>
        private static bool MainRunLoop(string apiKey, SortedDictionary<string, string> championList) {
            // get the user to select which champions to look up
            ChampionFilterForm filterForm = new ChampionFilterForm(championList.Keys);
            Application.Run(filterForm);

            // collect rune and mastery information
            IDictionary<string, List<string>> runesCount = new Dictionary<string, List<string>>();
            IDictionary<string, List<string>> masteriesCount = new Dictionary<string, List<string>>();

            // iterate through user selection
            foreach (var selection in filterForm.CheckedChampions) {
                if (selection.Value) {
                    // perform a query for that champion
                    QueryChampion(apiKey, championList[selection.Key], selection.Key, runesCount, masteriesCount);
                }
            }

            // sort the runes and masteries by the number of times they appear
            SortedDictionary<ListCount, JArray> sortedRunes = SortResultsByCount(runesCount);
            SortedDictionary<ListCount, JArray> sortedMasteries = SortResultsByCount(masteriesCount);

            // show the runes and masteries
            RunesAndMasteriesForm outputForm = new RunesAndMasteriesForm(sortedRunes, sortedMasteries);
            Application.Run(outputForm);

            // return whether a new query should be run
            return outputForm.NewQuery;
        }

        /// <summary>
        /// Queries the API for rune and mastery information on a champion.</summary>
        /// <param name="apiKey">The API key to use when querying.</param>
        /// <param name="key">The key used to identify the champion in the API.</param>
        /// <param name="name">The name of the champion.</param>
        /// <param name="runes">A mapping of a rune pages to a champions list.</param>
        /// <param name="masteries">A mapping of a mastery pages to a champions list.</param>
        private static void QueryChampion(string apiKey, string key, string name,
            IDictionary<string, List<string>> runes, IDictionary<string, List<string>> masteries) {
            // make the API call
            JToken response = API.MakeRequest("/champion/" + key + "?api_key=" + apiKey);

            // for each role, get their runes and masteries
            foreach (JObject jRole in (JArray) response) {
                string role = name + " " + jRole["role"];

                // most frequent runes and masteries
                if (QueryFrequent) {
                    // get their most frequent runes 
                    JArray runePage = (JArray) jRole["runes"]["mostGames"]["runes"];
                    if (runePage.Count > 3) {
                        List<string> runesList;
                        if (runes.TryGetValue(runePage.ToString(), out runesList)) {
                            runesList.Add(role);
                        } else {
                            runes.Add(runePage.ToString(), new List<string>() { role });
                        }
                    }

                    // get their most frequent masteries 
                    JArray masteryPage = (JArray) jRole["masteries"]["mostGames"]["masteries"];
                    if (masteryPage.Count == 3) {
                        List<string> masteriesList;
                        if (masteries.TryGetValue(masteryPage.ToString(), out masteriesList)) {
                            masteriesList.Add(role);
                        } else {
                            masteries.Add(masteryPage.ToString(), new List<string>() { role });
                        }
                    }
                }

                // highest winrate runes and masteries
                if (QueryWinRate) {
                    // get their highest winrate runes
                    JArray runePage = (JArray) jRole["runes"]["highestWinPercent"]["runes"];
                    if (runePage.Count > 3) {
                        List<string> runesList;
                        if (runes.TryGetValue(runePage.ToString(), out runesList)) {
                            runesList.Add(role);
                        } else {
                            runes.Add(runePage.ToString(), new List<string>() { role });
                        }
                    }

                    // get their highest winrate masteries 
                    JArray masteryPage = (JArray) jRole["masteries"]["highestWinPercent"]["masteries"];
                    if (masteryPage.Count == 3) {
                        List<string> masteriesList;
                        if (masteries.TryGetValue(masteryPage.ToString(), out masteriesList)) {
                            masteriesList.Add(role);
                        } else {
                            masteries.Add(masteryPage.ToString(), new List<string>() { role });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sorts the results of a champion query by the counts of their occurences.</summary>
        /// <param name="collectedResults">The results of the query. It is a mapping of a rune/mastery page to the champions that use them and their role.</param>
        /// <returns>A sorted dictionary of rune/mastery pages by their counts.</returns>
        private static SortedDictionary<ListCount, JArray> SortResultsByCount(IDictionary<string, List<string>> collectedResults) {
            SortedDictionary<ListCount, JArray> sortedResults = new SortedDictionary<ListCount, JArray>();
            foreach (var page in collectedResults) {
                // concatenate all the champion roles into a string
                string countLabel = "[" + string.Join(", ", page.Value) + "]";

                // add the rune page to the sorted dictionary
                sortedResults.Add(new ListCount() { Count = page.Value.Count, List = countLabel }, JArray.Parse(page.Key));
            }

            return sortedResults;
        }
    }
}

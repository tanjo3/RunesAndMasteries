using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RunesAndMasteries {

    public static class Program {

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
                // ensure Champion.GG API key is present
                string ggApiKey = Properties.Resources.ResourceManager.GetString("CHAMPION_GG_API_KEY");
                if (ggApiKey == null) {
                    throw new Exception("Error Locating Champion.GG API Key");
                }

                // ensure Riot Games API key is present
                string riotApiKey = Properties.Resources.ResourceManager.GetString("RIOT_GAMES_API_KEY");
                if (riotApiKey == null) {
                    throw new Exception("Error Locating Riot Games API Key");
                }

                // generate a list of champion names
                allChampions = new SortedDictionary<string, string>();
                JToken response = API.MakeRequest(API.API_NAME.RIOT_GAMES, $"/lol/static-data/v3/champions?api_key={riotApiKey}");
                foreach (KeyValuePair<string, JToken> champion in (JObject) response["data"]) {
                    allChampions.Add(champion.Value["name"].ToString(), champion.Value["key"].ToString());
                }

                // set up champion filter form
                ChampionFilterForm filterForm = new ChampionFilterForm(allChampions);
                filterForm.ApiKey = ggApiKey;

                // set up the main output form
                RunesAndMasteriesForm outputForm = new RunesAndMasteriesForm(filterForm);
                outputForm.ApiKey = riotApiKey;

                // run the application
                Application.Run(outputForm);
            } catch (Exception e) {
                MessageBox.Show("Error: " + e.Message, "An error has occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("ERROR: An error occurred: '{0}'", e.Message);
            }
        }
    }
}

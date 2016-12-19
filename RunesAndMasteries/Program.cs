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

                // set up champion filter form
                ChampionFilterForm filterForm = new ChampionFilterForm(allChampions);
                filterForm.ApiKey = apiKey;

                // set up the main output form
                RunesAndMasteriesForm outputForm = new RunesAndMasteriesForm(filterForm);

                // run the application
                Application.Run(outputForm);
            } catch (Exception e) {
                MessageBox.Show("Error: " + e.Message, "An error has occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("An error occurred: '{0}'", e.Message);
            }
        }
    }
}

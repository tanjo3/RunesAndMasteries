using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RunesAndMasteries {

    public partial class ChampionFilterForm : Form {

        /// <summary>
        /// Whether to add frequent runes and masteries to data.</summary>
        public static readonly bool QueryFrequent = true;

        /// <summary>
        /// Whether to add highest win rate runes and masteries to data.</summary>
        public static readonly bool QueryWinRate = false;

        /// <summary>
        /// API for use during calls.</summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Runes for the champions the user selects.</summary>
        public IDictionary<ListCount, JArray> RunesSelected { get; set; }

        /// <summary>
        /// Masteries for the champions the user selects.</summary>
        public IDictionary<ListCount, JArray> MasteriesSelected { get; set; }

        /// <summary>
        /// A mapping of champion names to their API identifers.</summary>
        private IDictionary<string, string> allChampions;

        /// <summary>
        /// A list of champion names and whether they have been selected.</summary>
        private IDictionary<string, bool> checkedChampions;

        /// <summary>
        /// References to the champion check boxes.</summary>
        private List<CheckBox> championCheckBoxes;

        /// <summary>
        /// Creates a new <c>ChampionFilterForm</c> form.</summary>
        /// <param name="champions">A list of champion names, mapped to their API identifiers.</param>
        public ChampionFilterForm(IDictionary<string, string> champions) {
            InitializeComponent();

            // add champions to form
            allChampions = champions;
            checkedChampions = new Dictionary<string, bool>();
            championCheckBoxes = new List<CheckBox>();
            PopulateWithChampions(champions.Keys);

            // start the form in an initial state
            ResetForm();
        }

        /// <summary>
        /// Adds a check box in the champions panel for each champion.</summary>
        /// <param name="champions">A list of champion names.</param>
        private void PopulateWithChampions(ICollection<string> champions) {
            foreach (string champion in champions) {
                // create a new check box for the champion
                CheckBox cb = new CheckBox() { Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Text = champion };
                cb.CheckedChanged += RecordChange;
                cb.CheckedChanged += UnselectAllOption;

                // keep references to the check box
                checkedChampions.Add(champion, false);
                championCheckBoxes.Add(cb);
            }

            // add check boxes to the view
            championsPanel.Controls.AddRange(championCheckBoxes.ToArray());
        }

        /// <summary>
        /// Resets the form to an initial state.</summary>
        public void ResetForm() {
            // reset option buttons
            selectSpecific.Checked = true;
            selectAll.Checked = false;

            // reset filter text box
            filterText.Text = string.Empty;

            // uncheck all champions
            foreach (CheckBox box in championCheckBoxes) {
                box.Checked = false;
            }

            // reset variables
            RunesSelected = null;
            MasteriesSelected = null;
        }

        /// <summary>
        /// Set all champion check boxes to be selected.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void SelectAllChampions(object sender, EventArgs e) {
            if (sender is RadioButton && ((RadioButton) sender).Checked == true) {
                foreach (CheckBox box in championCheckBoxes) {
                    box.Checked = true;
                }
            }
        }

        /// <summary>
        /// Records a change in a champion's check box checked state.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void RecordChange(object sender, EventArgs e) {
            if (sender is CheckBox) {
                CheckBox cb = (CheckBox) sender;
                checkedChampions[cb.Text] = cb.Checked;
            }
        }

        /// <summary>
        /// Switch the current option from "Select All" to "Select Specific".</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void UnselectAllOption(object sender, EventArgs e) {
            if (sender is CheckBox && ((CheckBox) sender).Checked == false) {
                selectAll.Checked = false;
                selectSpecific.Checked = true;
            }
        }

        /// <summary>
        /// Executed when the OK button is clicked.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OnFilterConfirm(object sender, EventArgs e) {
            // collect rune and mastery information
            IDictionary<string, List<string>> runesCount = new Dictionary<string, List<string>>();
            IDictionary<string, List<string>> masteriesCount = new Dictionary<string, List<string>>();

            // iterate through user selection
            foreach (var selection in checkedChampions) {
                if (selection.Value) {
                    // perform a query for that champion
                    QueryChampion(ApiKey, allChampions[selection.Key], selection.Key, runesCount, masteriesCount);
                }
            }

            // sort the runes and masteries by the number of times they appear
            RunesSelected = SortResultsByCount(runesCount);
            MasteriesSelected = SortResultsByCount(masteriesCount);

            // hide the form from view
            Hide();
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
            //JToken response = API.MakeRequest(API.API_NAME.CHAMPION_GG, $"/champion/{key}?api_key={apiKey}");

            // list the various champions roles
            string[] champRoles = { "Top", "Jungle", "Middle", "ADC", "Support" };

            // create a web client to read Champion.GG pages
            WebClient client = new WebClient();

            // try to read champion data for all roles
            foreach (string champRole in champRoles) {
                string role = $"{name} {champRole}";

                try {
                    // get the HTML as a string for the champion in the specified role
                    string htmlCode = client.DownloadString($"http://champion.gg/champion/{key}/{champRole}");

                    // ensure we're looking at the right role and weren't redirected
                    Match check = Regex.Match(htmlCode, "matchupData.champion = ({.*})");
                    if (check.Success && check.Groups.Count == 2) {
                        JObject checkRole = JObject.Parse(check.Groups[1].ToString());

                        if (checkRole["roleTitle"].ToString() != champRole) {
                            continue;
                        }
                    }

                    // search HTML for champion data
                    Match matchupData = Regex.Match(htmlCode, "matchupData.championData = ({.*})");
                    if (matchupData.Success && matchupData.Groups.Count == 2) {
                        // parse championData value as JSON
                        JObject championData = JObject.Parse(matchupData.Groups[1].ToString());

                        // most frequent runes and masteries
                        if (QueryFrequent) {
                            // get their most frequent runes
                            if (((JObject) championData["runes"]).HasValues) {
                                JArray runePage = (JArray) championData["runes"]["mostGames"]["runes"];
                                if (runePage.Count > 3) {
                                    List<string> runesList;
                                    if (runes.TryGetValue(runePage.ToString(), out runesList)) {
                                        runesList.Add(role);
                                    } else {
                                        runes.Add(runePage.ToString(), new List<string>() { role });
                                    }
                                }
                            } else {
                                Console.WriteLine("INFO: Unable to retrieve runes for {0}", role);
                            }

                            // get their most frequent masteries
                            if (((JObject) championData["masteries"]).HasValues) {
                                JArray masteryPage = (JArray) championData["masteries"]["mostGames"]["masteries"];
                                if (masteryPage.Count == 3) {
                                    List<string> masteriesList;
                                    if (masteries.TryGetValue(masteryPage.ToString(), out masteriesList)) {
                                        masteriesList.Add(role);
                                    } else {
                                        masteries.Add(masteryPage.ToString(), new List<string>() { role });
                                    }
                                }
                            } else {
                                Console.WriteLine("INFO: Unable to retrieve masteries for {0}", role);
                            }
                        }

                        // highest winrate runes and masteries
                        if (QueryWinRate) {
                            // get their highest winrate runes
                            JArray runePage = (JArray) championData["runes"]["highestWinPercent"]["runes"];
                            if (runePage.Count > 3) {
                                List<string> runesList;
                                if (runes.TryGetValue(runePage.ToString(), out runesList)) {
                                    runesList.Add(role);
                                } else {
                                    runes.Add(runePage.ToString(), new List<string>() { role });
                                }
                            }

                            // get their highest winrate masteries
                            JArray masteryPage = (JArray) championData["masteries"]["highestWinPercent"]["masteries"];
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
                } catch (WebException e) {
                    Console.WriteLine("ERROR: Unable to lookup {0}: '{1}'", role, e.Message);
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
                string countLabel = $"[{string.Join(", ", page.Value)}]";

                // add the rune page to the sorted dictionary
                sortedResults.Add(new ListCount() { Count = page.Value.Count, List = countLabel }, JArray.Parse(page.Key));
            }

            return sortedResults;
        }

        /// <summary>
        /// Executed when the filter text box is updated.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OnFilterUpdate(object sender, EventArgs e) {
            if (sender is TextBox) {
                TextBox tb = sender as TextBox;

                if (string.IsNullOrEmpty(tb.Text)) {
                    // enable all check boxes
                    foreach (CheckBox cb in championCheckBoxes) {
                        cb.Enabled = true;
                    }
                } else {
                    // convert search to lower case for easy matching
                    string search = tb.Text.ToLower();

                    // only enable check boxes which match the search term
                    foreach (CheckBox cb in championCheckBoxes) {
                        if (cb.Text.ToLower().Contains(search)) {
                            cb.Enabled = true;
                        } else {
                            cb.Enabled = false;
                        }
                    }
                }
            }
        }
    }
}

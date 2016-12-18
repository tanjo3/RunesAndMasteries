using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RunesAndMasteries {

    public partial class ChampionFilterForm : Form {

        /// <summary>
        /// A list of champion names and whether they have been selected.</summary>
        public IDictionary<string, bool> CheckedChampions { get; }

        /// <summary>
        /// References to the champion check boxes.</summary>
        private List<CheckBox> championCheckBoxes;

        /// <summary>
        /// Creates a new <c>ChampionFilterForm</c> form.</summary>
        /// <param name="champions">A list of champion names to be put in the selection panel.</param>
        public ChampionFilterForm(ICollection<string> champions) {
            InitializeComponent();

            CheckedChampions = new Dictionary<string, bool>();
            championCheckBoxes = new List<CheckBox>();
            PopulateWithChampions(champions);
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
                CheckedChampions.Add(champion, false);
                championCheckBoxes.Add(cb);
            }

            // add check boxes to the view
            championsPanel.Controls.AddRange(championCheckBoxes.ToArray());
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
                CheckedChampions[cb.Text] = cb.Checked;
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
            // close the form
            Close();
        }
    }
}

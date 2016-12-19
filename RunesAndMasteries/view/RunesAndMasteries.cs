using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace RunesAndMasteries {

    public partial class RunesAndMasteriesForm : Form {

        /// <summary>
        /// The form to let the user choose which champions to look up.</summary>
        private ChampionFilterForm filterForm;

        /// <summary>
        /// References to the mastery pictures for the ferocity mastery tree.</summary>
        private Dictionary<string, PictureBox> ferocityPictures = new Dictionary<string, PictureBox>();

        /// <summary>
        /// References to the mastery pictures for the cunning mastery tree.</summary>
        private Dictionary<string, PictureBox> cunningPictures = new Dictionary<string, PictureBox>();

        /// <summary>
        /// References to the mastery pictures for the resolve mastery tree.</summary>
        private Dictionary<string, PictureBox> resolvePictures = new Dictionary<string, PictureBox>();

        /// <summary>
        /// References to the mastery points labels for the ferocity mastery tree.</summary>
        private Dictionary<string, Label> ferocityLabels = new Dictionary<string, Label>();

        /// <summary>
        /// References to the mastery points labels for the cunning mastery tree.</summary>
        private Dictionary<string, Label> cunningLabels = new Dictionary<string, Label>();

        /// <summary>
        /// References to the mastery points labels for the resolve mastery tree.</summary>
        private Dictionary<string, Label> resolveLabels = new Dictionary<string, Label>();

        /// <summary>
        /// Rune pages referenced in this instance organized by their order in the list.</summary>
        private JArray[] runesByIndex;

        /// <summary>
        /// Previous index of selected rune page.</summary>
        private int previousRuneIndex = -1;

        /// <summary>
        /// Mastery pages referenced in this instance organized by their order in the list.</summary>
        private JArray[] masteriesByIndex;

        /// <summary>
        /// Previous index of selected mastery page.</summary>
        private int previousMasteryIndex = -1;

        /// <summary>
        /// Creates a new <c>RunesAndMasteriesForm</c> form.</summary>
        public RunesAndMasteriesForm(ChampionFilterForm filterForm) {
            InitializeComponent();
            this.filterForm = filterForm;

            // create references to the Controls in the ferocity mastery tree
            foreach (Control c in ferocityMasteries.Controls) {
                if (c is PictureBox) {
                    ferocityPictures.Add(c.Tag.ToString(), c as PictureBox);
                } else if (c is Label) {
                    ferocityLabels.Add(c.Tag.ToString(), c as Label);
                }
            }

            // create references to the Controls in the cunning mastery tree
            foreach (Control c in cunningMasteries.Controls) {
                if (c is PictureBox) {
                    cunningPictures.Add(c.Tag.ToString(), c as PictureBox);
                } else if (c is Label) {
                    cunningLabels.Add(c.Tag.ToString(), c as Label);
                }
            }

            // create references to the Controls in the resolve mastery tree
            foreach (Control c in resolveMasteries.Controls) {
                if (c is PictureBox) {
                    resolvePictures.Add(c.Tag.ToString(), c as PictureBox);
                } else if (c is Label) {
                    resolveLabels.Add(c.Tag.ToString(), c as Label);
                }
            }

            // Start form in an initial state
            ResetForm();
        }

        /// <summary>
        /// Sets up the form with the specified rune and mastery information.</summary>
        /// <param name="runes">The runes information.</param>
        /// <param name="masteries">The masteries information.</param>
        public void SetUpForm(IDictionary<ListCount, JArray> runes, IDictionary<ListCount, JArray> masteries) {
            // reset the form before updating
            ResetForm();

            // add runes to the list
            runesByIndex = new JArray[runes.Count];
            runesList.BeginUpdate();
            foreach (var runePage in runes) {
                int index = runesList.Items.Add(runePage.Key.Count + ": " + runePage.Key.List);
                runesByIndex[index] = runePage.Value;
            }
            runesList.EndUpdate();

            // add masteries to the list
            masteriesByIndex = new JArray[masteries.Count];
            masteriesList.BeginUpdate();
            foreach (var masteryPage in masteries) {
                int index = masteriesList.Items.Add(masteryPage.Key.Count + ": " + masteryPage.Key.List);
                masteriesByIndex[index] = masteryPage.Value;
            }
            masteriesList.EndUpdate();
        }

        /// <summary>
        /// Converts an image to grayscale.</summary>
        /// <remarks>Reference: code.msdn.microsoft.com/windowsapps/Convert-an-Image-to-5115a5ac</remarks>
        /// <param name="original">The original image.</param>
        /// <returns>The grayscaled image.</returns>
        private Image ConvertToGrayscale(Image original) {
            Bitmap source = new Bitmap(original);
            Bitmap grayscaled = new Bitmap(source.Width, source.Height);

            ColorMatrix colorMatrix = new ColorMatrix(new float[][] {
                new float[] { 0.299F, 0.299F, 0.299F, 0, 0 },
                new float[] { 0.587F, 0.587F, 0.587F, 0, 0 },
                new float[] { 0.114F, 0.114F, 0.114F, 0, 0 },
                new float[] { 0,      0,      0,      1, 0 },
                new float[] { 0,      0,      0,      0, 1 }
            });

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            Graphics graphics = Graphics.FromImage(grayscaled);
            graphics.DrawImage(source, new Rectangle(0, 0, source.Width,
                source.Height), 0, 0, source.Width,
                source.Height, GraphicsUnit.Pixel, attributes);
            graphics.Dispose();

            return grayscaled as Image;
        }

        /// <summary>
        /// This function is called when a mastery object is enabled/disabled.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OnMasteryEnable(object sender, EventArgs e) {
            if (sender is PictureBox) {
                PictureBox mastery = sender as PictureBox;

                if (mastery.Enabled) {
                    // restore original mastery image when enabled
                    object original = Properties.Resources.ResourceManager.GetObject("_" + mastery.Tag.ToString());

                    if (original != null && original is Image) {
                        mastery.Image = original as Image;
                    } else {
                        mastery.Image = mastery.ErrorImage;
                    }
                } else {
                    // set mastery image to gray when disabled
                    mastery.Image = ConvertToGrayscale(mastery.Image);
                }
            }
        }

        /// <summary>
        /// Reset all runes lists to empty.</summary>
        private void ResetRunes() {
            marksList.Items.Clear();
            sealsList.Items.Clear();
            glyphsList.Items.Clear();
            quintsList.Items.Clear();
        }

        /// <summary>
        /// Disables all mastery images and sets the points to zero.</summary>
        /// <remarks>Should be called prior to updating masteries.</remarks>
        private void ResetMasteries() {
            // reset ferocity mastery tree
            foreach (string mastery in ferocityPictures.Keys) {
                ferocityPictures[mastery].Enabled = false;
                ferocityLabels[mastery].Text = "0";
            }

            // reset cunning mastery tree
            foreach (string mastery in cunningPictures.Keys) {
                cunningPictures[mastery].Enabled = false;
                cunningLabels[mastery].Text = "0";
            }

            // reset resolve mastery tree
            foreach (string mastery in resolvePictures.Keys) {
                resolvePictures[mastery].Enabled = false;
                resolveLabels[mastery].Text = "0";
            }
        }

        /// <summary>
        /// Resets the form to an initial state.</summary>
        public void ResetForm() {
            // reset UI controls
            runesList.Items.Clear();
            ResetRunes();
            masteriesList.Items.Clear();
            ResetMasteries();

            // reset instance variables
            previousRuneIndex = -1;
            previousMasteryIndex = -1;
        }

        /// <summary>
        /// Executed when this form is first shown.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OnFirstShown(object sender, EventArgs e) {
            // show the champion filter form modally
            filterForm.ShowDialog();

            // set up form with user selection
            if (filterForm.RunesSelected != null && filterForm.MasteriesSelected != null) {
                SetUpForm(filterForm.RunesSelected, filterForm.MasteriesSelected);
            } else {
                filterForm.Dispose();
                Close();
            }
        }

        /// <summary>
        /// Executed when the "New Query" button is clicked.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void StartNewQuery(object sender, EventArgs e) {
            // show the champion select dialog
            filterForm.ResetForm();
            filterForm.ShowDialog();

            // set up form with user selection
            if (filterForm.RunesSelected != null && filterForm.MasteriesSelected != null) {
                SetUpForm(filterForm.RunesSelected, filterForm.MasteriesSelected);
            } else {
                filterForm.Dispose();
                Close();
            }
        }

        /// <summary>
        /// Update the runes display with the new selection.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OnRunesChanged(object sender, EventArgs e) {
            if (sender is ListBox) {
                // get the selected rune page index
                int selectedIndex = (sender as ListBox).SelectedIndex;

                // update the runes display
                if (selectedIndex != previousRuneIndex) {
                    previousRuneIndex = selectedIndex;
                    SetAsRunes(runesByIndex[selectedIndex]);
                }
            }
        }

        /// <summary>
        /// Update the masteries display with the new selection.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OnMasteriesChanged(object sender, EventArgs e) {
            if (sender is ListBox) {
                // get the selected mastery page index
                int selectedIndex = (sender as ListBox).SelectedIndex;

                // update the masteries display
                if (selectedIndex != previousMasteryIndex) {
                    previousMasteryIndex = selectedIndex;
                    SetAsMasteries(masteriesByIndex[selectedIndex]);
                }
            }
        }

        /// <summary>
        /// Update the runes displayed with the rune page.</summary>
        /// <param name="runes">The rune page to display.</param>
        private void SetAsRunes(JArray runes) {
            // reset the runes display
            ResetRunes();

            // add each rune to their respective lists
            foreach (JToken rune in runes) {
                // create the string to be added to the list
                string name = rune["name"].ToString();
                string label = rune["number"] + "x " + name + " (" + rune["description"] + ")";

                // add rune to the appropriate list
                if (name.Contains("Mark")) {
                    marksList.Items.Add(label);
                } else if (name.Contains("Seal")) {
                    sealsList.Items.Add(label);
                } else if (name.Contains("Glyph")) {
                    glyphsList.Items.Add(label);
                } else {
                    quintsList.Items.Add(label);
                }
            }
        }

        /// <summary>
        /// Update the masteries displayed with the mastery page.</summary>
        /// <param name="runes">The mastery page to display.</param>
        private void SetAsMasteries(JArray masteries) {
            // reset the masteries display
            ResetMasteries();

            Dictionary<string, PictureBox>[] allPictureBoxes = { ferocityPictures, cunningPictures, resolvePictures };
            Dictionary<string, Label>[] allPointsLabels = { ferocityLabels, cunningLabels, resolveLabels };

            // update each mastery tree
            int i = 0;
            foreach (JToken masteryTree in masteries) {
                Dictionary<string, PictureBox> pictureBoxes = allPictureBoxes[i];
                Dictionary<string, Label> pointsLabels = allPointsLabels[i];

                // go through each mastery in the tree
                foreach (JToken mastery in (JArray) masteryTree["data"]) {
                    string id = mastery["mastery"].ToString();
                    string points = mastery["points"].ToString();

                    // update UI if points are put into this mastery
                    if (points != "0") {
                        pictureBoxes[id].Enabled = true;
                        pointsLabels[id].Text = points;
                    }
                }

                i++;
            }
        }
    }
}

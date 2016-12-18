namespace RunesAndMasteries {
    partial class ChampionFilterForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChampionFilterForm));
            this.selectSpecific = new System.Windows.Forms.RadioButton();
            this.selectAll = new System.Windows.Forms.RadioButton();
            this.championSelect = new System.Windows.Forms.GroupBox();
            this.championsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.confirmSelection = new System.Windows.Forms.Button();
            this.championSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectSpecific
            // 
            this.selectSpecific.Font = new System.Drawing.Font("Fantasque Sans Mono", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectSpecific.Location = new System.Drawing.Point(12, 12);
            this.selectSpecific.Name = "selectSpecific";
            this.selectSpecific.Size = new System.Drawing.Size(318, 30);
            this.selectSpecific.TabIndex = 0;
            this.selectSpecific.Text = "Query Specific Champions";
            // 
            // selectAll
            // 
            this.selectAll.Font = new System.Drawing.Font("Fantasque Sans Mono", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectAll.Location = new System.Drawing.Point(336, 12);
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(270, 30);
            this.selectAll.TabIndex = 1;
            this.selectAll.Text = "Select All Champions";
            this.selectAll.CheckedChanged += new System.EventHandler(this.SelectAllChampions);
            // 
            // championSelect
            // 
            this.championSelect.Controls.Add(this.championsPanel);
            this.championSelect.Font = new System.Drawing.Font("Fantasque Sans Mono", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.championSelect.Location = new System.Drawing.Point(12, 48);
            this.championSelect.Name = "championSelect";
            this.championSelect.Size = new System.Drawing.Size(1240, 575);
            this.championSelect.TabIndex = 2;
            this.championSelect.TabStop = false;
            this.championSelect.Text = "Champions";
            // 
            // championsPanel
            // 
            this.championsPanel.AutoScroll = true;
            this.championsPanel.ColumnCount = 8;
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.championsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.championsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.championsPanel.Font = new System.Drawing.Font("Fantasque Sans Mono", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.championsPanel.Location = new System.Drawing.Point(3, 29);
            this.championsPanel.Name = "championsPanel";
            this.championsPanel.RowCount = 1;
            this.championsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.championsPanel.Size = new System.Drawing.Size(1234, 543);
            this.championsPanel.TabIndex = 0;
            // 
            // confirmSelection
            // 
            this.confirmSelection.Font = new System.Drawing.Font("Fantasque Sans Mono", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.confirmSelection.Location = new System.Drawing.Point(1102, 629);
            this.confirmSelection.Name = "confirmSelection";
            this.confirmSelection.Size = new System.Drawing.Size(150, 40);
            this.confirmSelection.TabIndex = 3;
            this.confirmSelection.Text = "OK";
            this.confirmSelection.Click += new System.EventHandler(this.OnFilterConfirm);
            // 
            // ChampionFilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.selectSpecific);
            this.Controls.Add(this.selectAll);
            this.Controls.Add(this.championSelect);
            this.Controls.Add(this.confirmSelection);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChampionFilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Champion Filter";
            this.championSelect.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton selectSpecific;
        private System.Windows.Forms.RadioButton selectAll;
        private System.Windows.Forms.GroupBox championSelect;
        private System.Windows.Forms.TableLayoutPanel championsPanel;
        private System.Windows.Forms.Button confirmSelection;
    }
}
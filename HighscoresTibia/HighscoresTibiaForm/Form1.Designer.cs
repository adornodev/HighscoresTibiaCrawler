using System.Windows.Forms;

namespace HighscoresTibiaForm
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainToolStrip = new System.Windows.Forms.ToolStrip();
            this.SendDataToolStrip = new System.Windows.Forms.ToolStripButton();
            this.MainGroupBox = new System.Windows.Forms.GroupBox();
            this.mainCheckBoxList = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numberPlayerExport = new System.Windows.Forms.NumericUpDown();
            this.consoleView = new System.Windows.Forms.ListView();
            this.firstColumnConsole = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MainToolStrip.SuspendLayout();
            this.MainGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numberPlayerExport)).BeginInit();
            this.SuspendLayout();
            // 
            // MainToolStrip
            // 
            this.MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SendDataToolStrip});
            this.MainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.MainToolStrip.Name = "MainToolStrip";
            this.MainToolStrip.Size = new System.Drawing.Size(523, 25);
            this.MainToolStrip.TabIndex = 0;
            this.MainToolStrip.Text = "MainToolStrip";
            // 
            // SendDataToolStrip
            // 
            this.SendDataToolStrip.Image = global::HighscoresTibiaForm.Properties.Resources.ic_send;
            this.SendDataToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SendDataToolStrip.Name = "SendDataToolStrip";
            this.SendDataToolStrip.Size = new System.Drawing.Size(56, 22);
            this.SendDataToolStrip.Text = "SEND";
            this.SendDataToolStrip.Click += new System.EventHandler(this.SendDataToolStrip_Click);
            // 
            // MainGroupBox
            // 
            this.MainGroupBox.Controls.Add(this.mainCheckBoxList);
            this.MainGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.MainGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainGroupBox.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.MainGroupBox.Location = new System.Drawing.Point(12, 38);
            this.MainGroupBox.Name = "MainGroupBox";
            this.MainGroupBox.Size = new System.Drawing.Size(314, 68);
            this.MainGroupBox.TabIndex = 1;
            this.MainGroupBox.TabStop = false;
            this.MainGroupBox.Text = "Vocations";
            // 
            // mainCheckBoxList
            // 
            this.mainCheckBoxList.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.mainCheckBoxList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mainCheckBoxList.CheckOnClick = true;
            this.mainCheckBoxList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainCheckBoxList.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.mainCheckBoxList.FormattingEnabled = true;
            this.mainCheckBoxList.Items.AddRange(new object[] {
            "Knight",
            "Paladin",
            "Druid",
            "Sorcerer"});
            this.mainCheckBoxList.Location = new System.Drawing.Point(24, 22);
            this.mainCheckBoxList.MultiColumn = true;
            this.mainCheckBoxList.Name = "mainCheckBoxList";
            this.mainCheckBoxList.Size = new System.Drawing.Size(280, 32);
            this.mainCheckBoxList.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(336, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "Players Number";
            // 
            // numberPlayerExport
            // 
            this.numberPlayerExport.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.numberPlayerExport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numberPlayerExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberPlayerExport.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.numberPlayerExport.Location = new System.Drawing.Point(378, 65);
            this.numberPlayerExport.Maximum = new decimal(new int[] {
            19800,
            0,
            0,
            0});
            this.numberPlayerExport.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numberPlayerExport.Name = "numberPlayerExport";
            this.numberPlayerExport.Size = new System.Drawing.Size(70, 27);
            this.numberPlayerExport.TabIndex = 3;
            this.numberPlayerExport.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numberPlayerExport.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // consoleView
            // 
            this.consoleView.BackColor = System.Drawing.SystemColors.MenuText;
            this.consoleView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consoleView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.firstColumnConsole});
            this.consoleView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.consoleView.ForeColor = System.Drawing.Color.Lime;
            this.consoleView.Location = new System.Drawing.Point(13, 141);
            this.consoleView.MultiSelect = false;
            this.consoleView.Name = "consoleView";
            this.consoleView.Size = new System.Drawing.Size(498, 146);
            this.consoleView.TabIndex = 4;
            this.consoleView.UseCompatibleStateImageBehavior = false;
            // 
            // firstColumnConsole
            // 
            this.firstColumnConsole.Text = "";
            this.firstColumnConsole.Width = 140;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ClientSize = new System.Drawing.Size(523, 299);
            this.Controls.Add(this.consoleView);
            this.Controls.Add(this.numberPlayerExport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MainGroupBox);
            this.Controls.Add(this.MainToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "HighscoresTibia";
            this.MainToolStrip.ResumeLayout(false);
            this.MainToolStrip.PerformLayout();
            this.MainGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numberPlayerExport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip MainToolStrip;
        private System.Windows.Forms.ToolStripButton SendDataToolStrip;
        private System.Windows.Forms.GroupBox MainGroupBox;
        public System.Windows.Forms.CheckedListBox mainCheckBoxList;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown numberPlayerExport;
        public System.Windows.Forms.ListView consoleView;
        public ColumnHeader firstColumnConsole;
    }
}


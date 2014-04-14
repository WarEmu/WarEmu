﻿namespace ServerLauncher
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.B_start = new System.Windows.Forms.Button();
            this.B_stop = new System.Windows.Forms.Button();
            this.StartAccountCheckBox = new System.Windows.Forms.CheckBox();
            this.StartLauncherCheckBox = new System.Windows.Forms.CheckBox();
            this.StartLobbyCheckBox = new System.Windows.Forms.CheckBox();
            this.StartWorldCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog_gamePath = new System.Windows.Forms.FolderBrowserDialog();
            this.button_browse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // B_start
            // 
            this.B_start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_start.Location = new System.Drawing.Point(65, 249);
            this.B_start.Name = "B_start";
            this.B_start.Size = new System.Drawing.Size(91, 23);
            this.B_start.TabIndex = 1;
            this.B_start.Text = "Start Selected";
            this.B_start.UseVisualStyleBackColor = true;
            this.B_start.Click += new System.EventHandler(this.B_start_Click);
            // 
            // B_stop
            // 
            this.B_stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.B_stop.Location = new System.Drawing.Point(162, 249);
            this.B_stop.Name = "B_stop";
            this.B_stop.Size = new System.Drawing.Size(91, 23);
            this.B_stop.TabIndex = 2;
            this.B_stop.Text = "Stop All";
            this.B_stop.UseVisualStyleBackColor = true;
            this.B_stop.Click += new System.EventHandler(this.B_stop_Click);
            // 
            // StartAccountCheckBox
            // 
            this.StartAccountCheckBox.AutoSize = true;
            this.StartAccountCheckBox.Checked = true;
            this.StartAccountCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartAccountCheckBox.Location = new System.Drawing.Point(12, 12);
            this.StartAccountCheckBox.Name = "StartAccountCheckBox";
            this.StartAccountCheckBox.Size = new System.Drawing.Size(125, 17);
            this.StartAccountCheckBox.TabIndex = 3;
            this.StartAccountCheckBox.Text = "Start AccountCacher";
            this.StartAccountCheckBox.UseVisualStyleBackColor = true;
            // 
            // StartLauncherCheckBox
            // 
            this.StartLauncherCheckBox.AutoSize = true;
            this.StartLauncherCheckBox.Checked = true;
            this.StartLauncherCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartLauncherCheckBox.Location = new System.Drawing.Point(12, 35);
            this.StartLauncherCheckBox.Name = "StartLauncherCheckBox";
            this.StartLauncherCheckBox.Size = new System.Drawing.Size(127, 17);
            this.StartLauncherCheckBox.TabIndex = 4;
            this.StartLauncherCheckBox.Text = "Start LauncherServer";
            this.StartLauncherCheckBox.UseVisualStyleBackColor = true;
            // 
            // StartLobbyCheckBox
            // 
            this.StartLobbyCheckBox.AutoSize = true;
            this.StartLobbyCheckBox.Checked = true;
            this.StartLobbyCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartLobbyCheckBox.Location = new System.Drawing.Point(12, 58);
            this.StartLobbyCheckBox.Name = "StartLobbyCheckBox";
            this.StartLobbyCheckBox.Size = new System.Drawing.Size(111, 17);
            this.StartLobbyCheckBox.TabIndex = 5;
            this.StartLobbyCheckBox.Text = "Start LobbyServer";
            this.StartLobbyCheckBox.UseVisualStyleBackColor = true;
            // 
            // StartWorldCheckBox
            // 
            this.StartWorldCheckBox.AutoSize = true;
            this.StartWorldCheckBox.Checked = true;
            this.StartWorldCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartWorldCheckBox.Location = new System.Drawing.Point(12, 81);
            this.StartWorldCheckBox.Name = "StartWorldCheckBox";
            this.StartWorldCheckBox.Size = new System.Drawing.Size(110, 17);
            this.StartWorldCheckBox.TabIndex = 6;
            this.StartWorldCheckBox.Text = "Start WorldServer";
            this.StartWorldCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Path to the game client";
            // 
            // textBox_path
            // 
            this.textBox_path.Location = new System.Drawing.Point(12, 153);
            this.textBox_path.Name = "textBox_path";
            this.textBox_path.Size = new System.Drawing.Size(241, 20);
            this.textBox_path.TabIndex = 8;
            // 
            // folderBrowserDialog_gamePath
            // 
            this.folderBrowserDialog_gamePath.ShowNewFolderButton = false;
            // 
            // button_browse
            // 
            this.button_browse.Location = new System.Drawing.Point(12, 179);
            this.button_browse.Name = "button_browse";
            this.button_browse.Size = new System.Drawing.Size(241, 23);
            this.button_browse.TabIndex = 10;
            this.button_browse.Text = "Browse for game path";
            this.button_browse.UseVisualStyleBackColor = true;
            this.button_browse.Click += new System.EventHandler(this.button_browse_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 284);
            this.Controls.Add(this.button_browse);
            this.Controls.Add(this.textBox_path);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StartWorldCheckBox);
            this.Controls.Add(this.StartLobbyCheckBox);
            this.Controls.Add(this.StartLauncherCheckBox);
            this.Controls.Add(this.StartAccountCheckBox);
            this.Controls.Add(this.B_stop);
            this.Controls.Add(this.B_start);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ServerLauncher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_start;
        private System.Windows.Forms.Button B_stop;
        private System.Windows.Forms.CheckBox StartAccountCheckBox;
        private System.Windows.Forms.CheckBox StartLauncherCheckBox;
        private System.Windows.Forms.CheckBox StartLobbyCheckBox;
        private System.Windows.Forms.CheckBox StartWorldCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog_gamePath;
        private System.Windows.Forms.Button button_browse;
    }
}


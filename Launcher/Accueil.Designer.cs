namespace Launcher
{
    partial class Accueil
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
            this.T_username = new System.Windows.Forms.TextBox();
            this.T_password = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.B_start = new System.Windows.Forms.Button();
            this.T_Log = new System.Windows.Forms.TextBox();
            this.L_selection = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Realms = new System.Windows.Forms.DataGridView();
            this.RealmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Online = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Players = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Destruction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Order = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.b_realms = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Realms)).BeginInit();
            this.SuspendLayout();
            // 
            // T_username
            // 
            this.T_username.Location = new System.Drawing.Point(13, 22);
            this.T_username.Name = "T_username";
            this.T_username.Size = new System.Drawing.Size(100, 20);
            this.T_username.TabIndex = 0;
            // 
            // T_password
            // 
            this.T_password.Location = new System.Drawing.Point(13, 49);
            this.T_password.Name = "T_password";
            this.T_password.Size = new System.Drawing.Size(100, 20);
            this.T_password.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Account";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // B_start
            // 
            this.B_start.Location = new System.Drawing.Point(13, 76);
            this.B_start.Name = "B_start";
            this.B_start.Size = new System.Drawing.Size(75, 23);
            this.B_start.TabIndex = 4;
            this.B_start.Text = "Connect";
            this.B_start.UseVisualStyleBackColor = true;
            this.B_start.Click += new System.EventHandler(this.B_start_Click);
            // 
            // T_Log
            // 
            this.T_Log.Location = new System.Drawing.Point(12, 254);
            this.T_Log.Multiline = true;
            this.T_Log.Name = "T_Log";
            this.T_Log.ReadOnly = true;
            this.T_Log.Size = new System.Drawing.Size(430, 72);
            this.T_Log.TabIndex = 5;
            // 
            // L_selection
            // 
            this.L_selection.FormattingEnabled = true;
            this.L_selection.Items.AddRange(new object[] {
            "French",
            "English",
            "Deutsch",
            "Italian",
            "Spanish",
            "Korean",
            "Chinese",
            "Japanese",
            "Russian"});
            this.L_selection.Location = new System.Drawing.Point(259, 48);
            this.L_selection.Name = "L_selection";
            this.L_selection.Size = new System.Drawing.Size(121, 21);
            this.L_selection.TabIndex = 6;
            this.L_selection.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(259, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Language";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // Realms
            // 
            this.Realms.AllowUserToOrderColumns = true;
            this.Realms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Realms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RealmName,
            this.Online,
            this.Players,
            this.Destruction,
            this.Order});
            this.Realms.Location = new System.Drawing.Point(12, 157);
            this.Realms.Name = "Realms";
            this.Realms.Size = new System.Drawing.Size(429, 91);
            this.Realms.TabIndex = 8;
            // 
            // RealmName
            // 
            this.RealmName.HeaderText = "Realm Name";
            this.RealmName.Name = "RealmName";
            this.RealmName.ReadOnly = true;
            // 
            // Online
            // 
            this.Online.HeaderText = "Online";
            this.Online.Name = "Online";
            this.Online.ReadOnly = true;
            // 
            // Players
            // 
            this.Players.HeaderText = "Players";
            this.Players.Name = "Players";
            this.Players.ReadOnly = true;
            this.Players.Width = 50;
            // 
            // Destruction
            // 
            this.Destruction.HeaderText = "Destruction";
            this.Destruction.Name = "Destruction";
            this.Destruction.ReadOnly = true;
            this.Destruction.Width = 50;
            // 
            // Order
            // 
            this.Order.HeaderText = "Order";
            this.Order.Name = "Order";
            this.Order.ReadOnly = true;
            this.Order.Width = 50;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Realms :";
            // 
            // b_realms
            // 
            this.b_realms.Location = new System.Drawing.Point(150, 128);
            this.b_realms.Name = "b_realms";
            this.b_realms.Size = new System.Drawing.Size(128, 23);
            this.b_realms.TabIndex = 10;
            this.b_realms.Text = "Update Realms";
            this.b_realms.UseVisualStyleBackColor = true;
            this.b_realms.Click += new System.EventHandler(this.b_realms_Click);
            // 
            // Accueil
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 338);
            this.Controls.Add(this.b_realms);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Realms);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.L_selection);
            this.Controls.Add(this.T_Log);
            this.Controls.Add(this.B_start);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.T_password);
            this.Controls.Add(this.T_username);
            this.Name = "Accueil";
            this.Text = "War Launcher";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Disconnect);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Realms)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox T_username;
        private System.Windows.Forms.MaskedTextBox T_password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button B_start;
        private System.Windows.Forms.TextBox T_Log;
        private System.Windows.Forms.ComboBox L_selection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView Realms;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button b_realms;
        private System.Windows.Forms.DataGridViewTextBoxColumn RealmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Online;
        private System.Windows.Forms.DataGridViewTextBoxColumn Players;
        private System.Windows.Forms.DataGridViewTextBoxColumn Destruction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Order;
    }
}


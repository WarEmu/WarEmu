namespace ServerLauncher
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
            this.SuspendLayout();
            // 
            // B_start
            // 
            this.B_start.Location = new System.Drawing.Point(12, 12);
            this.B_start.Name = "B_start";
            this.B_start.Size = new System.Drawing.Size(75, 23);
            this.B_start.TabIndex = 1;
            this.B_start.Text = "Lancer";
            this.B_start.UseVisualStyleBackColor = true;
            this.B_start.Click += new System.EventHandler(this.B_start_Click);
            // 
            // B_stop
            // 
            this.B_stop.Location = new System.Drawing.Point(94, 12);
            this.B_stop.Name = "B_stop";
            this.B_stop.Size = new System.Drawing.Size(75, 23);
            this.B_stop.TabIndex = 2;
            this.B_stop.Text = "Arretter";
            this.B_stop.UseVisualStyleBackColor = true;
            this.B_stop.Click += new System.EventHandler(this.B_stop_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 42);
            this.Controls.Add(this.B_stop);
            this.Controls.Add(this.B_start);
            this.Name = "Form1";
            this.Text = "ServerLauncher";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button B_start;
        private System.Windows.Forms.Button B_stop;
    }
}


namespace OnlineTask
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.reseiveClientControl = new NetManager.Client.ReseiveClientControl();
            this.button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // reseiveClientControl
            // 
            this.reseiveClientControl.IPServer = ((System.Net.IPAddress)(resources.GetObject("reseiveClientControl.IPServer")));
            this.reseiveClientControl.IsSyncronized = true;
            this.reseiveClientControl.Location = new System.Drawing.Point(12, 12);
            this.reseiveClientControl.Name = "reseiveClientControl";
            this.reseiveClientControl.Size = new System.Drawing.Size(174, 98);
            this.reseiveClientControl.TabIndex = 4;
            // 
            // button
            // 
            this.button.Location = new System.Drawing.Point(55, 116);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(75, 23);
            this.button.TabIndex = 5;
            this.button.Text = "Старт";
            this.button.UseVisualStyleBackColor = true;
            this.button.Click += new System.EventHandler(this.Button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button);
            this.Controls.Add(this.reseiveClientControl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private NetManager.Client.ReseiveClientControl reseiveClientControl;
        private System.Windows.Forms.Button button;
    }
}


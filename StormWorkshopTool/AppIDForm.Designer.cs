namespace StormWorkshopTool
{
    partial class AppIDForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppIDForm));
            this.SelectLabel = new System.Windows.Forms.Label();
            this.CLFullButton = new System.Windows.Forms.Button();
            this.CLSandboxButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SelectLabel
            // 
            this.SelectLabel.AutoSize = true;
            this.SelectLabel.Location = new System.Drawing.Point(12, 9);
            this.SelectLabel.Name = "SelectLabel";
            this.SelectLabel.Size = new System.Drawing.Size(87, 13);
            this.SelectLabel.TabIndex = 0;
            this.SelectLabel.Text = "Select the game:";
            // 
            // CLFullButton
            // 
            this.CLFullButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CLFullButton.Location = new System.Drawing.Point(15, 25);
            this.CLFullButton.Name = "CLFullButton";
            this.CLFullButton.Size = new System.Drawing.Size(219, 23);
            this.CLFullButton.TabIndex = 1;
            this.CLFullButton.Text = "Caribbean Legend (Full)";
            this.CLFullButton.UseVisualStyleBackColor = true;
            this.CLFullButton.Click += new System.EventHandler(this.CLFullButton_Click);
            // 
            // CLSandboxButton
            // 
            this.CLSandboxButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CLSandboxButton.Location = new System.Drawing.Point(15, 54);
            this.CLSandboxButton.Name = "CLSandboxButton";
            this.CLSandboxButton.Size = new System.Drawing.Size(219, 23);
            this.CLSandboxButton.TabIndex = 2;
            this.CLSandboxButton.Text = "Caribbean Legend: Sandbox";
            this.CLSandboxButton.UseVisualStyleBackColor = true;
            this.CLSandboxButton.Click += new System.EventHandler(this.CLSandboxButton_Click);
            // 
            // AppIDForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 96);
            this.Controls.Add(this.CLSandboxButton);
            this.Controls.Add(this.CLFullButton);
            this.Controls.Add(this.SelectLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(262, 135);
            this.Name = "AppIDForm";
            this.Text = "App ID";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SelectLabel;
        private System.Windows.Forms.Button CLFullButton;
        private System.Windows.Forms.Button CLSandboxButton;
    }
}
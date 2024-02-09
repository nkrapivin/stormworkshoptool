namespace StormWorkshopTool
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SteamTimer = new System.Windows.Forms.Timer(this.components);
            this.AvatarPictureBox = new System.Windows.Forms.PictureBox();
            this.PersonaNameLabel = new System.Windows.Forms.Label();
            this.SteamIDLabel = new System.Windows.Forms.Label();
            this.MyItemsListView = new System.Windows.Forms.ListView();
            this.CreateNewItemButton = new System.Windows.Forms.Button();
            this.GameWorkshopLinkLabel = new System.Windows.Forms.LinkLabel();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.RefreshItemsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.AvatarPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SteamTimer
            // 
            this.SteamTimer.Interval = 1;
            this.SteamTimer.Tick += new System.EventHandler(this.SteamTimer_Tick);
            // 
            // AvatarPictureBox
            // 
            this.AvatarPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AvatarPictureBox.Location = new System.Drawing.Point(12, 12);
            this.AvatarPictureBox.Name = "AvatarPictureBox";
            this.AvatarPictureBox.Size = new System.Drawing.Size(64, 64);
            this.AvatarPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.AvatarPictureBox.TabIndex = 0;
            this.AvatarPictureBox.TabStop = false;
            // 
            // PersonaNameLabel
            // 
            this.PersonaNameLabel.AutoSize = true;
            this.PersonaNameLabel.Location = new System.Drawing.Point(82, 12);
            this.PersonaNameLabel.Name = "PersonaNameLabel";
            this.PersonaNameLabel.Size = new System.Drawing.Size(93, 13);
            this.PersonaNameLabel.TabIndex = 0;
            this.PersonaNameLabel.Text = "Profile Name Here";
            // 
            // SteamIDLabel
            // 
            this.SteamIDLabel.AutoSize = true;
            this.SteamIDLabel.Location = new System.Drawing.Point(82, 25);
            this.SteamIDLabel.Name = "SteamIDLabel";
            this.SteamIDLabel.Size = new System.Drawing.Size(67, 13);
            this.SteamIDLabel.TabIndex = 1;
            this.SteamIDLabel.Text = "1234567890";
            // 
            // MyItemsListView
            // 
            this.MyItemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MyItemsListView.HideSelection = false;
            this.MyItemsListView.Location = new System.Drawing.Point(12, 82);
            this.MyItemsListView.Name = "MyItemsListView";
            this.MyItemsListView.Size = new System.Drawing.Size(776, 356);
            this.MyItemsListView.TabIndex = 5;
            this.MyItemsListView.UseCompatibleStateImageBehavior = false;
            this.MyItemsListView.ItemActivate += new System.EventHandler(this.MyItemsListView_ItemActivate);
            // 
            // CreateNewItemButton
            // 
            this.CreateNewItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateNewItemButton.Location = new System.Drawing.Point(765, 53);
            this.CreateNewItemButton.Name = "CreateNewItemButton";
            this.CreateNewItemButton.Size = new System.Drawing.Size(23, 23);
            this.CreateNewItemButton.TabIndex = 4;
            this.CreateNewItemButton.Text = "+";
            this.CreateNewItemButton.UseVisualStyleBackColor = true;
            this.CreateNewItemButton.Click += new System.EventHandler(this.CreateNewItemButton_Click);
            // 
            // GameWorkshopLinkLabel
            // 
            this.GameWorkshopLinkLabel.AutoSize = true;
            this.GameWorkshopLinkLabel.Location = new System.Drawing.Point(82, 38);
            this.GameWorkshopLinkLabel.Name = "GameWorkshopLinkLabel";
            this.GameWorkshopLinkLabel.Size = new System.Drawing.Size(173, 13);
            this.GameWorkshopLinkLabel.TabIndex = 2;
            this.GameWorkshopLinkLabel.TabStop = true;
            this.GameWorkshopLinkLabel.Text = "Browse this game\'s workshop page";
            this.GameWorkshopLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GameWorkshopLinkLabel_LinkClicked);
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Location = new System.Drawing.Point(82, 51);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(0, 13);
            this.ProgressLabel.TabIndex = 6;
            // 
            // RefreshItemsButton
            // 
            this.RefreshItemsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RefreshItemsButton.Location = new System.Drawing.Point(736, 53);
            this.RefreshItemsButton.Name = "RefreshItemsButton";
            this.RefreshItemsButton.Size = new System.Drawing.Size(23, 23);
            this.RefreshItemsButton.TabIndex = 3;
            this.RefreshItemsButton.Text = "⟳";
            this.RefreshItemsButton.UseVisualStyleBackColor = true;
            this.RefreshItemsButton.Click += new System.EventHandler(this.RefreshItemsButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.RefreshItemsButton);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.GameWorkshopLinkLabel);
            this.Controls.Add(this.CreateNewItemButton);
            this.Controls.Add(this.MyItemsListView);
            this.Controls.Add(this.SteamIDLabel);
            this.Controls.Add(this.PersonaNameLabel);
            this.Controls.Add(this.AvatarPictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(810, 480);
            this.Name = "MainForm";
            this.Text = "Storm Workshop Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AvatarPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer SteamTimer;
        private System.Windows.Forms.PictureBox AvatarPictureBox;
        private System.Windows.Forms.Label PersonaNameLabel;
        private System.Windows.Forms.Label SteamIDLabel;
        private System.Windows.Forms.ListView MyItemsListView;
        private System.Windows.Forms.Button CreateNewItemButton;
        private System.Windows.Forms.LinkLabel GameWorkshopLinkLabel;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.Button RefreshItemsButton;
    }
}


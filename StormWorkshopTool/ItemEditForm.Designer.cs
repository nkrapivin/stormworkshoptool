namespace StormWorkshopTool
{
    partial class ItemEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemEditForm));
            this.TitleTextBox = new System.Windows.Forms.TextBox();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.DescriptionRichTextBox = new System.Windows.Forms.RichTextBox();
            this.VisibilityLabel = new System.Windows.Forms.Label();
            this.VisibilityComboBox = new System.Windows.Forms.ComboBox();
            this.PreviewImageLabel = new System.Windows.Forms.Label();
            this.PreviewImagePictureBox = new System.Windows.Forms.PictureBox();
            this.PublishButton = new System.Windows.Forms.Button();
            this.PreviewImageOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ContentsFolderLabel = new System.Windows.Forms.Label();
            this.ContentsFolderTextBox = new System.Windows.Forms.TextBox();
            this.ContentsFolderButton = new System.Windows.Forms.Button();
            this.ChangelogLabel = new System.Windows.Forms.Label();
            this.ChangelogRichTextBox = new System.Windows.Forms.RichTextBox();
            this.AgreementPrefixLabel = new System.Windows.Forms.Label();
            this.AgreementLinkLabel = new System.Windows.Forms.LinkLabel();
            this.IDLabel = new System.Windows.Forms.Label();
            this.IDTextBox = new System.Windows.Forms.TextBox();
            this.EditWebsiteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.EditDescriptionCheckBox = new System.Windows.Forms.CheckBox();
            this.EditTitleCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TitleTextBox
            // 
            this.TitleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TitleTextBox.Location = new System.Drawing.Point(15, 70);
            this.TitleTextBox.MaxLength = 120;
            this.TitleTextBox.Name = "TitleTextBox";
            this.TitleTextBox.Size = new System.Drawing.Size(256, 20);
            this.TitleTextBox.TabIndex = 4;
            this.TitleTextBox.Text = "Super Awesome Mod";
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Location = new System.Drawing.Point(12, 54);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(33, 13);
            this.TitleLabel.TabIndex = 2;
            this.TitleLabel.Text = "Title: ";
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Location = new System.Drawing.Point(277, 15);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(66, 13);
            this.DescriptionLabel.TabIndex = 11;
            this.DescriptionLabel.Text = "Description: ";
            // 
            // DescriptionRichTextBox
            // 
            this.DescriptionRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DescriptionRichTextBox.Location = new System.Drawing.Point(280, 31);
            this.DescriptionRichTextBox.MaxLength = 7992;
            this.DescriptionRichTextBox.Name = "DescriptionRichTextBox";
            this.DescriptionRichTextBox.Size = new System.Drawing.Size(529, 168);
            this.DescriptionRichTextBox.TabIndex = 13;
            this.DescriptionRichTextBox.Text = "Super Awesome Description\nCan have multiple lines!\nAwesome!\n\n\n=^-^=";
            // 
            // VisibilityLabel
            // 
            this.VisibilityLabel.AutoSize = true;
            this.VisibilityLabel.Location = new System.Drawing.Point(12, 93);
            this.VisibilityLabel.Name = "VisibilityLabel";
            this.VisibilityLabel.Size = new System.Drawing.Size(49, 13);
            this.VisibilityLabel.TabIndex = 5;
            this.VisibilityLabel.Text = "Visibility: ";
            // 
            // VisibilityComboBox
            // 
            this.VisibilityComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VisibilityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VisibilityComboBox.FormattingEnabled = true;
            this.VisibilityComboBox.Items.AddRange(new object[] {
            "Public",
            "Friends Only",
            "Private",
            "Unlisted"});
            this.VisibilityComboBox.Location = new System.Drawing.Point(15, 109);
            this.VisibilityComboBox.Name = "VisibilityComboBox";
            this.VisibilityComboBox.Size = new System.Drawing.Size(256, 21);
            this.VisibilityComboBox.TabIndex = 6;
            // 
            // PreviewImageLabel
            // 
            this.PreviewImageLabel.AutoSize = true;
            this.PreviewImageLabel.Location = new System.Drawing.Point(12, 172);
            this.PreviewImageLabel.Name = "PreviewImageLabel";
            this.PreviewImageLabel.Size = new System.Drawing.Size(225, 13);
            this.PreviewImageLabel.TabIndex = 10;
            this.PreviewImageLabel.Text = "Preview Image (Click or drag and drop to set): ";
            // 
            // PreviewImagePictureBox
            // 
            this.PreviewImagePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PreviewImagePictureBox.Location = new System.Drawing.Point(15, 188);
            this.PreviewImagePictureBox.Name = "PreviewImagePictureBox";
            this.PreviewImagePictureBox.Size = new System.Drawing.Size(256, 314);
            this.PreviewImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PreviewImagePictureBox.TabIndex = 7;
            this.PreviewImagePictureBox.TabStop = false;
            this.PreviewImagePictureBox.Click += new System.EventHandler(this.PreviewImagePictureBox_Click);
            this.PreviewImagePictureBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.PreviewImagePictureBox_DragDrop);
            this.PreviewImagePictureBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.PreviewImagePictureBox_DragEnter);
            // 
            // PublishButton
            // 
            this.PublishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PublishButton.Location = new System.Drawing.Point(734, 508);
            this.PublishButton.Name = "PublishButton";
            this.PublishButton.Size = new System.Drawing.Size(75, 23);
            this.PublishButton.TabIndex = 19;
            this.PublishButton.Text = "Publish";
            this.PublishButton.UseVisualStyleBackColor = true;
            this.PublishButton.Click += new System.EventHandler(this.PublishButton_Click);
            // 
            // PreviewImageOpenFileDialog
            // 
            this.PreviewImageOpenFileDialog.Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp|All files|*.*";
            this.PreviewImageOpenFileDialog.Title = "Choose a preview image";
            this.PreviewImageOpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.PreviewImageOpenFileDialog_FileOk);
            // 
            // ContentsFolderLabel
            // 
            this.ContentsFolderLabel.AutoSize = true;
            this.ContentsFolderLabel.Location = new System.Drawing.Point(12, 133);
            this.ContentsFolderLabel.Name = "ContentsFolderLabel";
            this.ContentsFolderLabel.Size = new System.Drawing.Size(81, 13);
            this.ContentsFolderLabel.TabIndex = 7;
            this.ContentsFolderLabel.Text = "Contents folder:";
            // 
            // ContentsFolderTextBox
            // 
            this.ContentsFolderTextBox.AllowDrop = true;
            this.ContentsFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContentsFolderTextBox.Location = new System.Drawing.Point(15, 149);
            this.ContentsFolderTextBox.Name = "ContentsFolderTextBox";
            this.ContentsFolderTextBox.Size = new System.Drawing.Size(218, 20);
            this.ContentsFolderTextBox.TabIndex = 8;
            this.ContentsFolderTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.ContentsFolderTextBox_DragDrop);
            this.ContentsFolderTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.ContentsFolderTextBox_DragEnter);
            // 
            // ContentsFolderButton
            // 
            this.ContentsFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ContentsFolderButton.Location = new System.Drawing.Point(239, 149);
            this.ContentsFolderButton.Name = "ContentsFolderButton";
            this.ContentsFolderButton.Size = new System.Drawing.Size(32, 20);
            this.ContentsFolderButton.TabIndex = 9;
            this.ContentsFolderButton.Text = "...";
            this.ContentsFolderButton.UseVisualStyleBackColor = true;
            this.ContentsFolderButton.Click += new System.EventHandler(this.ContentsFolderButton_Click);
            // 
            // ChangelogLabel
            // 
            this.ChangelogLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChangelogLabel.AutoSize = true;
            this.ChangelogLabel.Location = new System.Drawing.Point(277, 202);
            this.ChangelogLabel.Name = "ChangelogLabel";
            this.ChangelogLabel.Size = new System.Drawing.Size(61, 13);
            this.ChangelogLabel.TabIndex = 14;
            this.ChangelogLabel.Text = "Changelog:";
            // 
            // ChangelogRichTextBox
            // 
            this.ChangelogRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChangelogRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ChangelogRichTextBox.Location = new System.Drawing.Point(280, 218);
            this.ChangelogRichTextBox.Name = "ChangelogRichTextBox";
            this.ChangelogRichTextBox.Size = new System.Drawing.Size(529, 284);
            this.ChangelogRichTextBox.TabIndex = 15;
            this.ChangelogRichTextBox.Text = resources.GetString("ChangelogRichTextBox.Text");
            // 
            // AgreementPrefixLabel
            // 
            this.AgreementPrefixLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AgreementPrefixLabel.AutoSize = true;
            this.AgreementPrefixLabel.Location = new System.Drawing.Point(12, 518);
            this.AgreementPrefixLabel.Name = "AgreementPrefixLabel";
            this.AgreementPrefixLabel.Size = new System.Drawing.Size(193, 13);
            this.AgreementPrefixLabel.TabIndex = 17;
            this.AgreementPrefixLabel.Text = "By submitting this item, you agree to the";
            // 
            // AgreementLinkLabel
            // 
            this.AgreementLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AgreementLinkLabel.AutoSize = true;
            this.AgreementLinkLabel.Location = new System.Drawing.Point(211, 518);
            this.AgreementLinkLabel.Name = "AgreementLinkLabel";
            this.AgreementLinkLabel.Size = new System.Drawing.Size(163, 13);
            this.AgreementLinkLabel.TabIndex = 18;
            this.AgreementLinkLabel.TabStop = true;
            this.AgreementLinkLabel.Text = "Steam workshop terms of service";
            this.AgreementLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AgreementLinkLabel_LinkClicked);
            // 
            // IDLabel
            // 
            this.IDLabel.AutoSize = true;
            this.IDLabel.Location = new System.Drawing.Point(12, 15);
            this.IDLabel.Name = "IDLabel";
            this.IDLabel.Size = new System.Drawing.Size(21, 13);
            this.IDLabel.TabIndex = 0;
            this.IDLabel.Text = "ID:";
            // 
            // IDTextBox
            // 
            this.IDTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IDTextBox.Location = new System.Drawing.Point(15, 31);
            this.IDTextBox.Name = "IDTextBox";
            this.IDTextBox.ReadOnly = true;
            this.IDTextBox.Size = new System.Drawing.Size(256, 20);
            this.IDTextBox.TabIndex = 1;
            this.IDTextBox.Text = "None";
            // 
            // EditWebsiteLinkLabel
            // 
            this.EditWebsiteLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EditWebsiteLinkLabel.AutoSize = true;
            this.EditWebsiteLinkLabel.Location = new System.Drawing.Point(12, 505);
            this.EditWebsiteLinkLabel.Name = "EditWebsiteLinkLabel";
            this.EditWebsiteLinkLabel.Size = new System.Drawing.Size(223, 13);
            this.EditWebsiteLinkLabel.TabIndex = 16;
            this.EditWebsiteLinkLabel.TabStop = true;
            this.EditWebsiteLinkLabel.Text = "Edit this item on the Steam Workshop website";
            this.EditWebsiteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.EditWebsiteLinkLabel_LinkClicked);
            // 
            // EditDescriptionCheckBox
            // 
            this.EditDescriptionCheckBox.AutoSize = true;
            this.EditDescriptionCheckBox.Location = new System.Drawing.Point(349, 14);
            this.EditDescriptionCheckBox.Name = "EditDescriptionCheckBox";
            this.EditDescriptionCheckBox.Size = new System.Drawing.Size(139, 17);
            this.EditDescriptionCheckBox.TabIndex = 12;
            this.EditDescriptionCheckBox.Text = "Update the description?";
            this.EditDescriptionCheckBox.UseVisualStyleBackColor = true;
            this.EditDescriptionCheckBox.CheckedChanged += new System.EventHandler(this.EditDescriptionCheckBox_CheckedChanged);
            // 
            // EditTitleCheckBox
            // 
            this.EditTitleCheckBox.AutoSize = true;
            this.EditTitleCheckBox.Location = new System.Drawing.Point(51, 53);
            this.EditTitleCheckBox.Name = "EditTitleCheckBox";
            this.EditTitleCheckBox.Size = new System.Drawing.Size(104, 17);
            this.EditTitleCheckBox.TabIndex = 3;
            this.EditTitleCheckBox.Text = "Update the title?";
            this.EditTitleCheckBox.UseVisualStyleBackColor = true;
            this.EditTitleCheckBox.CheckedChanged += new System.EventHandler(this.EditTitleCheckBox_CheckedChanged);
            // 
            // ItemEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 552);
            this.Controls.Add(this.EditTitleCheckBox);
            this.Controls.Add(this.EditDescriptionCheckBox);
            this.Controls.Add(this.EditWebsiteLinkLabel);
            this.Controls.Add(this.IDTextBox);
            this.Controls.Add(this.IDLabel);
            this.Controls.Add(this.AgreementLinkLabel);
            this.Controls.Add(this.AgreementPrefixLabel);
            this.Controls.Add(this.ChangelogRichTextBox);
            this.Controls.Add(this.ChangelogLabel);
            this.Controls.Add(this.ContentsFolderButton);
            this.Controls.Add(this.ContentsFolderTextBox);
            this.Controls.Add(this.ContentsFolderLabel);
            this.Controls.Add(this.PublishButton);
            this.Controls.Add(this.PreviewImagePictureBox);
            this.Controls.Add(this.PreviewImageLabel);
            this.Controls.Add(this.VisibilityComboBox);
            this.Controls.Add(this.VisibilityLabel);
            this.Controls.Add(this.DescriptionRichTextBox);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.TitleTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(830, 590);
            this.Name = "ItemEditForm";
            this.Text = "Item Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ItemEditForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImagePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TitleTextBox;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.RichTextBox DescriptionRichTextBox;
        private System.Windows.Forms.Label VisibilityLabel;
        private System.Windows.Forms.ComboBox VisibilityComboBox;
        private System.Windows.Forms.Label PreviewImageLabel;
        private System.Windows.Forms.PictureBox PreviewImagePictureBox;
        private System.Windows.Forms.Button PublishButton;
        private System.Windows.Forms.OpenFileDialog PreviewImageOpenFileDialog;
        private System.Windows.Forms.Label ContentsFolderLabel;
        private System.Windows.Forms.TextBox ContentsFolderTextBox;
        private System.Windows.Forms.Button ContentsFolderButton;
        private System.Windows.Forms.Label ChangelogLabel;
        private System.Windows.Forms.RichTextBox ChangelogRichTextBox;
        private System.Windows.Forms.Label AgreementPrefixLabel;
        private System.Windows.Forms.LinkLabel AgreementLinkLabel;
        private System.Windows.Forms.Label IDLabel;
        private System.Windows.Forms.TextBox IDTextBox;
        private System.Windows.Forms.LinkLabel EditWebsiteLinkLabel;
        private System.Windows.Forms.CheckBox EditDescriptionCheckBox;
        private System.Windows.Forms.CheckBox EditTitleCheckBox;
    }
}
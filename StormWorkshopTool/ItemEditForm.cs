using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Steamworks;

namespace StormWorkshopTool
{
    public partial class ItemEditForm : Form
    {
        private readonly CommonOpenFileDialog ContentsFolderCommonOpenFileDialog = new CommonOpenFileDialog();
        internal UGCItem CurrentUGCItem;
        private bool IsPreviewDirty, IsExisting;

        public ItemEditForm()
        {
            InitializeComponent();
            Text = Localize.Tr(Text, "ItemEditForm.Text");
            IDLabel.Text = Localize.Tr(IDLabel.Text, "ItemEditForm.IDLabel.Text");
            TitleLabel.Text = Localize.Tr(TitleLabel.Text, "ItemEditForm.TitleLabel.Text");
            EditTitleCheckBox.Text = Localize.Tr(EditTitleCheckBox.Text, "ItemEditForm.EditTitleCheckBox.Text");
            VisibilityLabel.Text = Localize.Tr(VisibilityLabel.Text, "ItemEditForm.VisibilityLabel.Text");
            VisibilityComboBox.Items[0] = Localize.Tr((string)VisibilityComboBox.Items[0], "ItemEditForm.VisibilityComboBox.Items.0.Text");
            VisibilityComboBox.Items[1] = Localize.Tr((string)VisibilityComboBox.Items[1], "ItemEditForm.VisibilityComboBox.Items.1.Text");
            VisibilityComboBox.Items[2] = Localize.Tr((string)VisibilityComboBox.Items[2], "ItemEditForm.VisibilityComboBox.Items.2.Text");
            VisibilityComboBox.Items[3] = Localize.Tr((string)VisibilityComboBox.Items[3], "ItemEditForm.VisibilityComboBox.Items.3.Text");
            ContentsFolderLabel.Text = Localize.Tr(ContentsFolderLabel.Text, "ItemEditForm.ContentsFolderLabel.Text");
            PreviewImageLabel.Text = Localize.Tr(PreviewImageLabel.Text, "ItemEditForm.PreviewImageLabel.Text");
            DescriptionLabel.Text = Localize.Tr(DescriptionLabel.Text, "ItemEditForm.DescriptionLabel.Text");
            EditDescriptionCheckBox.Text = Localize.Tr(EditDescriptionCheckBox.Text, "ItemEditForm.EditDescriptionCheckBox.Text");
            ChangelogLabel.Text = Localize.Tr(ChangelogLabel.Text, "ItemEditForm.ChangelogLabel.Text");
            ChangelogRichTextBox.Text = Localize.Tr(ChangelogRichTextBox.Text, "ItemEditForm.ChangelogRichTextBox.Text");
            EditWebsiteLinkLabel.Text = Localize.Tr(EditWebsiteLinkLabel.Text, "ItemEditForm.EditWebsiteLinkLabel.Text");
            AgreementPrefixLabel.Text = Localize.Tr(AgreementPrefixLabel.Text, "ItemEditForm.AgreementPrefixLabel.Text");
            AgreementLinkLabel.Text = Localize.Tr(AgreementLinkLabel.Text, "ItemEditForm.AgreementLinkLabel.Text");
            PublishButton.Text = Localize.Tr(PublishButton.Text, "ItemEditForm.PublishButton.Text");
            // non-browsable fields: >:(((
            VisibilityComboBox.SelectedIndex = 0;
            PreviewImagePictureBox.AllowDrop = true;
            ContentsFolderCommonOpenFileDialog.IsFolderPicker = true;
            ContentsFolderCommonOpenFileDialog.Title = "Choose your mod folder";
            ContentsFolderCommonOpenFileDialog.EnsurePathExists = true;
            ContentsFolderCommonOpenFileDialog.EnsureValidNames = true;
            // disable this label, will be re-enabled in SetFromExisting
            EditWebsiteLinkLabel.Visible = false;
            EditDescriptionCheckBox.Visible = false;
            EditTitleCheckBox.Visible = false;

            ContentsFolderCommonOpenFileDialog.Title = Localize.Tr(ContentsFolderCommonOpenFileDialog.Title, "ItemEditForm.ContentsFolderCommonOpenFileDialog.Text");
        }

        private bool IsValidDirectory(string path)
        {
            // only return true for valid non-empty directories
            // (it makes no sense to upload an empty dir)
            try
            {
                return Directory.EnumerateFileSystemEntries(path).Any();
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateItem()
        {
            if (!IsValidDirectory(ContentsFolderTextBox.Text))
            {
                if (IsExisting) // update request?
                {
                    // allow modmakers to push info-only edits without doing content updates
                    var res = MessageBox.Show(this,
                        Localize.Tr("You are about to publish a mod update without any content changes. Are you sure?", "ItemEditForm.QuestionNoContent"),
                        Localize.Tr("Question", "CommonQuestion"),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (res != DialogResult.Yes)
                        return false; // did not explicitly confirm...
                    ContentsFolderTextBox.Text = "";
                }
                else
                {
                    // always reject, 1.0.0 mod submissions must be with content
                    MessageBox.Show(this,
                        Localize.Tr("Contents folder is empty or invalid. For new mods you MUST specify a content folder.", "ItemEditForm.NoFolder"),
                        Localize.Tr("Error", "CommonError"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        internal void SetFromExisting(UGCItem item)
        {
            // this is only called if we have some item already
            CurrentUGCItem = item;

            // apply current item state to UI
            IDTextBox.Text = CurrentUGCItem.Id.ToString();
            TitleTextBox.Text = CurrentUGCItem.Title;
            DescriptionRichTextBox.Text = CurrentUGCItem.Description;
            ChangelogRichTextBox.Text = Localize.Tr("Generic update.", "ItemEditForm.DummyChangelog");
            VisibilityComboBox.SelectedIndex = (int)CurrentUGCItem.Visibility;
            ContentsFolderTextBox.Text = Localize.Tr("<please your folder again!>", "ItemEditForm.SelectSameFolder");
            if (item.Preview != null)
            {
                try
                {
                    using (var ms = new MemoryStream(item.Preview, false))
                    {
                        var img = Image.FromStream(ms);
                        PreviewImagePictureBox.Image?.Dispose();
                        PreviewImagePictureBox.Image = img;
                    }
                }
                catch { }
            }

            EditWebsiteLinkLabel.Visible = true;
            EditDescriptionCheckBox.Visible = true;
            EditTitleCheckBox.Visible = true;
            DescriptionRichTextBox.Enabled = false;
            TitleTextBox.Enabled = false;
            IsExisting = true;
        }

        private void PreviewImagePictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void PreviewImagePictureBox_DragDrop(object sender, DragEventArgs e)
        {
            var that = (PictureBox)sender;
            var imgs = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            if (imgs == null || imgs.Length <= 0)
                return;

            using (var img = Image.FromFile(imgs[0]))
            {
                var bitmap = new Bitmap(img);
                that.Image?.Dispose();
                that.Image = bitmap;
                IsPreviewDirty = true;
            }
        }

        private void PreviewImagePictureBox_Click(object sender, EventArgs e)
        {
            PreviewImageOpenFileDialog.ShowDialog();
        }

        private void PreviewImageOpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            var that = (OpenFileDialog)sender;
            using (var img = Image.FromFile(that.FileName))
            {
                var bitmap = new Bitmap(img);
                PreviewImagePictureBox.Image?.Dispose();
                PreviewImagePictureBox.Image = bitmap;
                IsPreviewDirty = true;
            }
        }

        private void ContentsFolderButton_Click(object sender, EventArgs e)
        {
            var res = ContentsFolderCommonOpenFileDialog.ShowDialog();
            if (res != CommonFileDialogResult.Ok)
                return;

            ContentsFolderTextBox.Text = ContentsFolderCommonOpenFileDialog.FileName;
        }

        private void ItemEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ContentsFolderCommonOpenFileDialog?.Dispose();
        }

        private void ContentsFolderTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void ContentsFolderTextBox_DragDrop(object sender, DragEventArgs e)
        {
            var that = (TextBox)sender;
            var folders = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            if (folders == null || folders.Length <= 0)
                return;

            that.Text = folders[0];
        }

        private void PublishButton_Click(object sender, EventArgs e)
        {
            if (!ValidateItem())
                return;

            byte[] img = null;
            try
            {
                using (var ms = new MemoryStream())
                {
                    PreviewImagePictureBox.Image.Save(ms, ImageFormat.Png);
                    img = ms.ToArray();
                }

                if (img.Length > 1000000)
                {
                    // try to re-save as JPEG to hopefully reduce size
                    using (var ms = new MemoryStream())
                    {
                        PreviewImagePictureBox.Image.Save(ms, ImageFormat.Jpeg);
                        img = ms.ToArray();
                    }
                }

                if (img.Length > 1000000)
                {
                    // https://partner.steamgames.com/doc/api/ISteamUGC#SubmitItemUpdateResult_t
                    MessageBox.Show(this,
                        Localize.Tr("Preview image is larger than 1 megabyte. Try resizing or compressing the image.", "ItemEditForm.ImageTooLarge"),
                        Localize.Tr("Error", "CommonError"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            catch
            {
                // ignore encoder errors...
            }

            if (CurrentUGCItem == null)
                CurrentUGCItem = new UGCItem();

            CurrentUGCItem.Title = TitleTextBox.Text;
            CurrentUGCItem.Description = DescriptionRichTextBox.Text;
            CurrentUGCItem.Changelog = ChangelogRichTextBox.Text;
            CurrentUGCItem.Visibility = (ERemoteStoragePublishedFileVisibility)VisibilityComboBox.SelectedIndex;
            CurrentUGCItem.ContentsFolder = ContentsFolderTextBox.Text;
            CurrentUGCItem.Preview = img;

            CurrentUGCItem.Dirty = UGCItem.DirtyFlags.None;

            if (IsExisting)
            {
                if (EditDescriptionCheckBox.Checked)
                {
                    CurrentUGCItem.Dirty |= UGCItem.DirtyFlags.Description;
                }

                if (EditTitleCheckBox.Checked)
                {
                    CurrentUGCItem.Dirty |= UGCItem.DirtyFlags.Title;
                }

                if (IsPreviewDirty)
                {
                    CurrentUGCItem.Dirty |= UGCItem.DirtyFlags.Preview;
                }
            }
            else
            {
                // for new items we must update all the fields...
                CurrentUGCItem.Dirty |= UGCItem.DirtyFlags.All;
            }

            DialogResult = DialogResult.Yes;
            Close();
        }

        private void AgreementLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MainForm.ProcessStart("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
            ((LinkLabel)sender).LinkVisited = true;
        }

        private void EditWebsiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MainForm.ProcessStart($"https://steamcommunity.com/sharedfiles/filedetails/?id={IDTextBox.Text}");
            ((LinkLabel)sender).LinkVisited = true;
        }

        private void EditDescriptionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DescriptionRichTextBox.Enabled = EditDescriptionCheckBox.Checked;
        }

        private void EditTitleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            TitleTextBox.Enabled = EditTitleCheckBox.Checked;
        }
    }
}

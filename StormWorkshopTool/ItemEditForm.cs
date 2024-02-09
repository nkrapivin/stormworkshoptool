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

        public ItemEditForm()
        {
            InitializeComponent();
            // non-browsable fields: >:(((
            VisibilityComboBox.SelectedIndex = 0;
            PreviewImagePictureBox.AllowDrop = true;
            ContentsFolderCommonOpenFileDialog.IsFolderPicker = true;
            ContentsFolderCommonOpenFileDialog.Title = "Choose your mod folder";
            ContentsFolderCommonOpenFileDialog.EnsurePathExists = true;
            ContentsFolderCommonOpenFileDialog.EnsureValidNames = true;
            // disable this label, will be re-enabled in SetFromExisting
            EditWebsiteLinkLabel.Visible = false;
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
                MessageBox.Show(this, "Contents folder is empty or invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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
            ChangelogRichTextBox.Text = "<please fill this in before submitting!>";
            VisibilityComboBox.SelectedIndex = (int)CurrentUGCItem.Visibility;
            ContentsFolderTextBox.Text = "<please select a new folder!>";
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

            var img = Image.FromFile(imgs[0]);
            //{
                that.Image?.Dispose();
                that.Image = img;
            //}
        }

        private void PreviewImagePictureBox_Click(object sender, EventArgs e)
        {
            PreviewImageOpenFileDialog.ShowDialog();
        }

        private void PreviewImageOpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            var that = (OpenFileDialog)sender;
            var img = Image.FromFile(that.FileName);
            //{
                PreviewImagePictureBox.Image?.Dispose();
                PreviewImagePictureBox.Image = img;
            //}
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
                        "The preview image is too large, it must be less than 1 Megabyte.",
                        "Error",
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

            DialogResult = DialogResult.Yes;
            Close();
        }

        private void AgreementLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://steamcommunity.com/sharedfiles/workshoplegalagreement")?.Dispose();
            ((LinkLabel)sender).LinkVisited = true;
        }

        private void EditWebsiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start($"https://steamcommunity.com/sharedfiles/filedetails/?id={IDTextBox.Text}")?.Dispose();
            ((LinkLabel)sender).LinkVisited = true;
        }
    }
}

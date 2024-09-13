using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Steamworks;

namespace StormWorkshopTool
{
    public partial class MainForm : Form
    {
        private CSteamID SteamID;
        private AppId_t AppID;
        private uint LoadPageIdx = 1;
        private CallResult<SteamUGCQueryCompleted_t> CROnUGCQueryCompleted;
        private Callback<AvatarImageLoaded_t> COnAvatarImageLoaded;
        private readonly List<UGCItem> Items = new List<UGCItem>();
        private readonly Dictionary<UGCHandle_t, CallResult<RemoteStorageDownloadUGCResult_t>> PinnedCRs = new Dictionary<UGCHandle_t, CallResult<RemoteStorageDownloadUGCResult_t>>();

        private UGCItem PendingItem;
        private ulong PendingBytesProcessed, PendingBytesTotal;
        private EItemUpdateStatus PendingStatus;
        private CallResult<CreateItemResult_t> PendingItemCR;
        private CallResult<SubmitItemUpdateResult_t> PendingItemUpdateCR;
        private bool UpdatePending;
        private bool UILockState;
        private readonly ImageList UGCImageList = new ImageList();

        public static void ProcessStart(string arg)
        {
            try
            {
                Process.Start(arg)?.Dispose();
            }
            catch { }
        }

        private void UpdateProgressLabel()
            => ProgressLabel.Text = $"Fetching your items... (Page: {LoadPageIdx}, Preview pics: {PinnedCRs.Count} left)";

        private void SetUILockState(bool state)
        {
            CreateNewItemButton.Enabled = !state;
            RefreshItemsButton.Enabled = !state;
            MyItemsListView.Enabled = !state;
            UILockState = state;
            if (!UILockState) ProgressLabel.Text = "";
        }

        private void SyncItemsWithUI()
        {
            MyItemsListView.BeginUpdate();
            MyItemsListView.Clear();
            UGCImageList.ImageSize = new Size(128, 128);
            UGCImageList.ColorDepth = ColorDepth.Depth32Bit;
            MyItemsListView.LargeImageList = UGCImageList;
            MyItemsListView.SmallImageList = UGCImageList;
            UGCImageList.Images.Clear();
            foreach (var item in Items)
            {
                var lvitem = new ListViewItem
                {
                    Text =
                    (string.IsNullOrEmpty(item.Title) ? "(Untitled)" : item.Title)
                    //+ (item.)
                };
                if (item.Preview != null)
                {
                    try
                    {
                        using (var ms = new MemoryStream(item.Preview, false))
                        using (var img = Image.FromStream(ms))
                        {
                            UGCImageList.Images.Add(img);
                            lvitem.ImageIndex = UGCImageList.Images.Count - 1;
                        }
                    }
                    catch { }
                }
                lvitem.Tag = item;
                MyItemsListView.Items.Add(lvitem);
            }
            MyItemsListView.EndUpdate();
        }

        private void SetAvatarToUI(int imgHandle)
        {
            if (!SteamUtils.GetImageSize(imgHandle, out var width, out var height))
                return;

            if ((width * height) <= 0)
                return;

            var bufflen = (int)width * (int)height * sizeof(int);
            var buff = new byte[bufflen];
            if (!SteamUtils.GetImageRGBA(imgHandle, buff, bufflen))
                return;

            // swap color channels for GDI+ compat
            for (var i = 0; i < bufflen; i += 4)
            {
                var b0 = buff[i];
                buff[i] = buff[i + 2];
                buff[i + 2] = b0;
            }

            var img = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
            //{
                var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, img.PixelFormat);
                Marshal.Copy(buff, 0, data.Scan0, bufflen);
                img.UnlockBits(data);
                AvatarPictureBox.Image?.Dispose();
                AvatarPictureBox.Image = img;
            //}
        }

        private void OnAvatarImageLoaded(AvatarImageLoaded_t param)
        {
            if (param.m_steamID != SteamID)
                return;

            SetAvatarToUI(param.m_iImage);
        }

        private void OnItemUpdate(SubmitItemUpdateResult_t param, bool bIOFailure)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                ShowError($"Failed to update item with EResult {param.m_eResult}");
                return;
            }

            if (param.m_bUserNeedsToAcceptWorkshopLegalAgreement)
            {
                ProcessStart("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
            }
        }

        private void OnRemoteStorageDownloadUGC(RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
        {
            PinnedCRs.Remove(param.m_hFile);
            UpdateProgressLabel();

            if (param.m_eResult != EResult.k_EResultOK || bIOFailure)
            {
                Debug.WriteLine($"UGC remote download fail with eresult {param.m_eResult}");
                return;
            }

            if (param.m_nSizeInBytes <= 0)
            {
                Debug.WriteLine($"UGC remote download fail, file is empty");
                return;
            }

            var bytes = new byte[param.m_nSizeInBytes];
            SteamRemoteStorage.UGCRead(
                param.m_hFile,
                bytes,
                bytes.Length,
                0,
                EUGCReadAction.k_EUGCRead_Close);
            var item = Items.Where(x => x.PreviewHandle == param.m_hFile).FirstOrDefault();
            if (item != null)
            {
                item.Preview = bytes;
            }    
        }

        private void OnCreateItem(CreateItemResult_t param, bool bIOFailure)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                ShowError($"OnCreateItem invalid EResult ({param.m_eResult})");
                return;
            }

            PendingItem.Id = param.m_nPublishedFileId;
            UpdateItem(PendingItem);
        }

        private void OnUGCQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                ShowError($"OnUGCQueryCompleted invalid EResult ({param.m_eResult})");
                return;
            }

            if (param.m_unNumResultsReturned == 0)
            {
                // we're done, there are no more items left.
                return;
            }

            for (var index = 0u; index < param.m_unNumResultsReturned; ++index)
            {
                var details = new SteamUGCDetails_t();
                if (!SteamUGC.GetQueryUGCResult(param.m_handle, index, out details))
                {
                    continue;
                }

                var item = new UGCItem();
                var previewhandle = SteamRemoteStorage.UGCDownload(details.m_hPreviewFile, 0);
                if (previewhandle != SteamAPICall_t.Invalid)
                {
                    item.PreviewHandle = details.m_hPreviewFile;
                    var cr = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnRemoteStorageDownloadUGC);
                    cr.Set(previewhandle);
                    PinnedCRs[details.m_hPreviewFile] = cr;
                }
                item.Title = details.m_rgchTitle;
                item.Description = details.m_rgchDescription;
                item.Id = details.m_nPublishedFileId;
                item.Visibility = details.m_eVisibility;
                Items.Add(item);
            }

            if (!SteamUGC.ReleaseQueryUGCRequest(param.m_handle))
            {
                ShowError("ReleaseQueryUGCRequest (inside CR) failed");
                return;
            }

            var handle = SteamUGC.CreateQueryUserUGCRequest(
                SteamID.GetAccountID(),
                EUserUGCList.k_EUserUGCList_Published,
                EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse,
                EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                AppID,
                AppID,
                ++LoadPageIdx
            );
            if (handle == UGCQueryHandle_t.Invalid)
            {
                ShowError("CreateQueryUserUGCRequest (inside CR) failed");
                return;
            }

            SteamUGC.SetLanguage(handle, "english");

            var call = SteamUGC.SendQueryUGCRequest(handle);
            if (call == SteamAPICall_t.Invalid)
            {
                SteamUGC.ReleaseQueryUGCRequest(handle);
                ShowError("SendQueryUGCRequest (inside CR) failed");
                return;
            }
            CROnUGCQueryCompleted.Set(call);

            UpdateProgressLabel();
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void ShowError(string text)
        {
            MessageBox.Show(this, text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowInfo(string text)
        {
            MessageBox.Show(this, text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadAllItems()
        {
            var handle = SteamUGC.CreateQueryUserUGCRequest(
                SteamID.GetAccountID(),
                EUserUGCList.k_EUserUGCList_Published,
                EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse,
                EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                AppID,
                AppID,
                /*start at page*/1
            );
            if (handle == UGCQueryHandle_t.Invalid)
            {
                ShowError("CreateQueryUserUGCRequest failed");
                return;
            }

            SteamUGC.SetLanguage(handle, "english");

            var call = SteamUGC.SendQueryUGCRequest(handle);
            if (call == SteamAPICall_t.Invalid)
            {
                SteamUGC.ReleaseQueryUGCRequest(handle);
                ShowError("SendQueryUGCRequest failed");
                return;
            }

            LoadPageIdx = 1;
            UpdatePending = true;
            MyItemsListView.BeginUpdate();
            Items.Clear();
            MyItemsListView.Items.Clear();
            UGCImageList.Images.Clear();
            MyItemsListView.EndUpdate();
            CROnUGCQueryCompleted.Set(call);
            UpdateProgressLabel();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var appId = 0u;
            using (var dlg = new AppIDForm())
            {
                var result = dlg.ShowDialog(this);
                if (result != DialogResult.Yes)
                {
                    Application.Exit();
                    return;
                }

                appId = dlg.ChosenAppId;
            }

            File.WriteAllText("steam_appid.txt", appId.ToString(), System.Text.Encoding.ASCII);

            if (!SteamAPI.Init())
            {
                ShowError("Failed to initialize the Steam API");
                Application.Exit();
                return;
            }

            CROnUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnUGCQueryCompleted);
            COnAvatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
            PendingItemCR = CallResult<CreateItemResult_t>.Create(OnCreateItem);
            PendingItemUpdateCR = CallResult<SubmitItemUpdateResult_t>.Create(OnItemUpdate);

            AppID = SteamUtils.GetAppID();
            if (AppID == AppId_t.Invalid)
            {
                ShowError("GetAppID failed");
                Application.Exit();
                return;
            }

            if (AppID != (AppId_t)appId)
            {
                ShowError($"GetAppID result ({AppID}) does not match expected App ID ({appId}), expect bugs!");
            }

            Text += $" (as App ID {AppID})";

            SteamID = SteamUser.GetSteamID();
            if (!SteamID.IsValid())
            {
                ShowError("GetSteamID failed (or invalid)");
                Application.Exit();
                return;
            }

            var personaName = SteamFriends.GetPersonaName();
            if (string.IsNullOrEmpty(personaName))
            {
                ShowError("GetPersonaName failed");
                Application.Exit();
                return;
            }
            PersonaNameLabel.Text = personaName;

            SteamIDLabel.Text = SteamID.ToString();
            var imgHandle = SteamFriends.GetLargeFriendAvatar(SteamID);
            SetAvatarToUI(imgHandle);
            LoadAllItems();

            // now we can enable the timer and dispatch callbacks/callresults
            SteamTimer.Enabled = true;

            ToolVersionLabel.Text += Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void SteamTimer_Tick(object sender, EventArgs e)
        {
            SteamAPI.RunCallbacks();

            if (UpdatePending)
            {
                if (!UILockState)
                {
                    SetUILockState(true);
                    // dirty dirty hack, but my deadline was till 12th so oh well
                    // what could I do?!
                    if (PendingItemCR.IsActive())
                        ProgressLabel.Text = "Waiting for CreateItem to complete...";
                }

                if (PendingItem != null)
                {
                    if (!PendingItemCR.IsActive() && !PendingItemUpdateCR.IsActive())
                    {
                        PendingItem = null;
                        UpdatePending = false;
                        LoadAllItems();
                    }
                    else if (PendingItemCR.IsActive())
                    {
                        //this moved to another place...
                        //ProgressLabel.Text = "Waiting for CreateItem to complete...";
                    }
                    else
                    {
                        var newStatus = SteamUGC.GetItemUpdateProgress(
                            PendingItem.UpdateHandle,
                            out var newProcessed,
                            out var newTotal);

                        // only touch .Text if something changed...
                        if (newStatus != PendingStatus ||
                            newProcessed != PendingBytesProcessed ||
                            newTotal != PendingBytesTotal)
                        {
                            PendingStatus = newStatus;
                            PendingBytesProcessed = newProcessed;
                            PendingBytesTotal = newTotal;

                            var statusString = PendingStatus.ToString().Replace("k_EItemUpdateStatus", "");
                            ProgressLabel.Text = $"Updating item ({statusString}): {newProcessed} out of {newTotal} bytes";
                        }
                    }
                }
                else
                {
                    if (PinnedCRs.Count == 0 && !CROnUGCQueryCompleted.IsActive())
                    {
                        UpdatePending = false;
                        SyncItemsWithUI();
                    }
                }
            }
            else
            {
                if (UILockState)
                {
                    SetUILockState(false);
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Debug.WriteLine("FormClosed: Shutting down Steam API...");
            SteamAPI.Shutdown();
        }

        private bool IsPNGHeader(byte[] b)
        {
            // http://www.libpng.org/pub/png/spec/1.2/PNG-Structure.html
            return
                (b != null) &&
                (b.Length > 8) &&
                (b[0] == 137) &&
                (b[1] == 80) &&
                (b[2] == 78) &&
                (b[3] == 71) &&
                (b[4] == 13) &&
                (b[5] == 10) &&
                (b[6] == 26) &&
                (b[7] == 10);
        }

        private void UpdateItem(UGCItem item)
        {
            var update = SteamUGC.StartItemUpdate(AppID, item.Id);
            if (update == UGCUpdateHandle_t.Invalid)
            {
                ShowError("StartItemUpdate failed");
                return;
            }

            if (!SteamUGC.SetItemUpdateLanguage(update, "english"))
            {
                ShowError("SetItemUpdateLanguage failed");
                return;
            }

            if (!string.IsNullOrEmpty(item.Title) && item.Dirty.HasFlag(UGCItem.DirtyFlags.Title))
            {
                if (!SteamUGC.SetItemTitle(update, item.Title))
                {
                    ShowError("SetItemTitle failed");
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.Description) && item.Dirty.HasFlag(UGCItem.DirtyFlags.Description))
            {
                if (!SteamUGC.SetItemDescription(update, item.Description))
                {
                    ShowError("SetItemDescription failed");
                    return;
                }
            }
            
            if (!SteamUGC.SetItemVisibility(update, item.Visibility))
            {
                ShowError("SetItemVisibility failed");
                return;
            }

            if (!string.IsNullOrWhiteSpace(item.ContentsFolder))
            {
                if (!SteamUGC.SetItemContent(update, item.ContentsFolder))
                {
                    ShowError("SetItemContent failed");
                    return;
                }
            }

            if (item.Preview != null && item.Dirty.HasFlag(UGCItem.DirtyFlags.Preview))
            {
                var tmpname = IsPNGHeader(item.Preview) ? "0a_preview.png" : "0a_preview.jpg";
                var tmpfile = Path.Combine(
                    Path.GetTempPath(),
                    tmpname
                );

                File.WriteAllBytes(tmpfile, item.Preview);
                if (!SteamUGC.SetItemPreview(update, tmpfile))
                {
                    ShowError("SetItemPreview failed");
                    return;
                }
            }

            var changelog = item.Changelog;
            // as per Steamworks SDK docs, this must be `null` for no change note...
            if (string.IsNullOrWhiteSpace(changelog))
                changelog = null;

            var call = SteamUGC.SubmitItemUpdate(update, changelog);
            if (call == SteamAPICall_t.Invalid)
            {
                ShowError("SubmitItemUpdate failed");
                return;
            }

            item.UpdateHandle = update;
            PendingItemUpdateCR.Set(call);
            UpdatePending = true;
        }

        private void PublishItem(UGCItem item)
        {
            PendingBytesTotal = PendingBytesProcessed = 0;
            PendingStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;

            if (item.Id == PublishedFileId_t.Invalid)
            {
                var call = SteamUGC.CreateItem(AppID, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                if (call == SteamAPICall_t.Invalid)
                {
                    ShowError("CreateItem call failed");
                    return;
                }

                PendingItem = item;
                UpdatePending = true;
                PendingItemCR.Set(call);
            }
            else
            {
                PendingItem = item;
                UpdateItem(item);
            }
        }

        private void OpenEditor(UGCItem item)
        {
            using (var editForm = new ItemEditForm())
            {
                if (item != null)
                    editForm.SetFromExisting(item);
                editForm.ShowDialog(this);
                if (editForm.DialogResult != DialogResult.Yes)
                    return;

                item = editForm.CurrentUGCItem;
            }

            PublishItem(item);
        }

        private void CreateNewItemButton_Click(object sender, EventArgs e)
        {
            OpenEditor(null);
        }

        private void MyItemsListView_ItemActivate(object sender, EventArgs e)
        {
            var that = (ListView)sender;
            if (that.SelectedItems.Count > 0)
            {
                var toOpen = that.SelectedItems[0];
                OpenEditor((UGCItem)toOpen.Tag);
            }
        }

        private void GameWorkshopLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStart($"https://steamcommunity.com/app/{AppID}/workshop/");
            ((LinkLabel)sender).LinkVisited = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (PendingItem != null)
            {
                // prevent closing if we're updating something
                e.Cancel = true;
            }
        }

        private void RefreshItemsButton_Click(object sender, EventArgs e)
        {
            // this should be guarded by UILockState hopefully...
            LoadAllItems();
        }
    }
}

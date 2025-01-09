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
        private CallResult<WorkshopEULAStatus_t> CROnWorkshopEULAStatus;
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
            => ProgressLabel.Text = Localize.Tr(
                "Fetching your items... (Page: {0}, Preview pics: {1} left)",
                "MainForm.ProgressLabel.Text.Fetching",
                LoadPageIdx,
                PinnedCRs.Count);

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
                    (string.IsNullOrWhiteSpace(item.Title)
                        ? Localize.Tr("(Untitled)", "MainForm.MyItemsListView.Text.Untitled")
                        : item.Title)
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

        private void OnWorkshopEULAStatus(WorkshopEULAStatus_t param, bool bIOFailure)
        {
            // TODO: m_bNeedsAction does not work in case of Caribbean Legend
            //       because CL is using the default EULA which confuses this call.
            if (param.m_unVersion == 0 || param.m_rtAction.m_RTime32 == 0)
            {
                ProcessStart("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
                ShowError(Localize.Tr(
                    "You must accept the Steam Workshop legal agreement. Please accept it, and then try again if necessary.",
                    "MainForm.MustAcceptAgreement")
                );
            }
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
                ShowError(Localize.Tr(
                    "Failed to update item with EResult {0}",
                    "MainForm.OnItemUpdateFail",
                    param.m_eResult)
                );
                return;
            }

            if (param.m_bUserNeedsToAcceptWorkshopLegalAgreement)
            {
                // this should silently invoke the default browser
                // and persuade the user to accept the agreement....
                ProcessStart("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
                ShowError(Localize.Tr(
                    "You must accept the Steam Workshop legal agreement. Please accept it, and then try again if necessary.",
                    "MainForm.MustAcceptAgreement")
                );
            }
        }

        private void OnRemoteStorageDownloadUGC(RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
        {
            PinnedCRs.Remove(param.m_hFile);
            UpdateProgressLabel();

            if (param.m_eResult != EResult.k_EResultOK || bIOFailure)
            {
                Debug.WriteLine(Localize.Tr(
                    "UGC remote download fail with EResult {0}",
                    "MainForm.UGCDownloadFailCode",
                    param.m_eResult)
                );
                return;
            }

            if (param.m_nSizeInBytes <= 0)
            {
                Debug.WriteLine(Localize.Tr(
                    "UGC remote download fail, file is empty",
                    "MainForm.UGCDownloadFailEmpty")
                );
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
                ShowError(Localize.Tr(
                    "OnCreateItem invalid EResult {0}",
                    "MainForm.OnCreateItemFail",
                    param.m_eResult)
                );
                return;
            }

            PendingItem.Id = param.m_nPublishedFileId;
            UpdateItem(PendingItem);
        }

        private void OnUGCQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                ShowError(Localize.Tr(
                    "OnUGCQueryCompleted invalid EResult {0}",
                    "MainForm.OnUGCQueryCompletedFail",
                    param.m_eResult)
                );
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
                if (details.m_hPreviewFile != UGCHandle_t.Invalid)
                {
                    var previewhandle = SteamRemoteStorage.UGCDownload(details.m_hPreviewFile, 0);
                    if (previewhandle != SteamAPICall_t.Invalid)
                    {
                        item.PreviewHandle = details.m_hPreviewFile;
                        var cr = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnRemoteStorageDownloadUGC);
                        cr.Set(previewhandle);
                        PinnedCRs[details.m_hPreviewFile] = cr;
                    }
                }
                item.Title = details.m_rgchTitle;
                item.Description = details.m_rgchDescription;
                item.Id = details.m_nPublishedFileId;
                item.Visibility = details.m_eVisibility;
                Items.Add(item);
            }

            if (!SteamUGC.ReleaseQueryUGCRequest(param.m_handle))
            {
                ShowError(Localize.Tr(
                    "ReleaseQueryUGCRequest (inside CR) failed",
                    "MainForm.ReleaseQueryUGCRequestInsideCRFail")
                );
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
                ShowError(Localize.Tr(
                    "CreateQueryUserUGCRequest (inside CR) failed",
                    "MainForm.CreateQueryUserUGCRequestInsideCRFail")
                );
                return;
            }

            SteamUGC.SetLanguage(handle, "english");

            var call = SteamUGC.SendQueryUGCRequest(handle);
            if (call == SteamAPICall_t.Invalid)
            {
                SteamUGC.ReleaseQueryUGCRequest(handle);
                ShowError(Localize.Tr(
                    "SendQueryUGCRequest (inside CR) failed",
                    "MainForm.SendQueryUGCRequestInsideCRFail")
                );
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
            MessageBox.Show(this, text, Localize.Tr("Error", "CommonError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowInfo(string text)
        {
            MessageBox.Show(this, text, Localize.Tr("Information", "CommonInformation"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                /* start at page index */ 1
            );
            if (handle == UGCQueryHandle_t.Invalid)
            {
                ShowError(
                    Localize.Tr(
                        "CreateQueryUserUGCRequest failed",
                        "MainForm.CreateQueryUserUGCRequestFail")
                );
                return;
            }

            SteamUGC.SetLanguage(handle, "english");

            var call = SteamUGC.SendQueryUGCRequest(handle);
            if (call == SteamAPICall_t.Invalid)
            {
                SteamUGC.ReleaseQueryUGCRequest(handle);
                ShowError(
                    Localize.Tr(
                        "SendQueryUGCRequest failed",
                        "MainForm.SendQueryUGCRequestFail")
                );
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
            var forcedAppId = false;
            try
            {
                // If this file exists, it will attempt to initialize itself
                // like a normal game would.
                // There is no call to RestartAppIfNecessary because it always
                // starts the "Default" option (which is the game)
                var d = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "depot_appid.txt"));
                var testAppId = uint.Parse(d.Trim());
                if (testAppId != 0)
                {
                    appId = testAppId;
                    forcedAppId = true;
                    Debug.WriteLine($"App ID (forced) = {appId}");
                }
            }
            catch { }

            if (!forcedAppId)
            {
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

                Debug.WriteLine($"App ID (dialog) = {appId}");

                try
                {
                    // NOTE: Steam API should read from the current directory,
                    //       not the app directory... I think... :|
                    File.WriteAllText("steam_appid.txt", appId.ToString(), System.Text.Encoding.ASCII);
                }
                catch
                {
                    ShowError(Localize.Tr(
                        "Unable to write the steam_appid.txt file, Steam API won't initialize properly",
                        "MainForm.SteamAppIdTxt")
                    );
                    Application.Exit();
                    return;
                }
            }

            if (!SteamAPI.Init())
            {
                ShowError(Localize.Tr(
                    "Failed to initialize the Steam API. Is the Steam client running and online?",
                    "MainForm.SteamApiInitFail")
                );
                Application.Exit();
                return;
            }

            CROnUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnUGCQueryCompleted);
            CROnWorkshopEULAStatus = CallResult<WorkshopEULAStatus_t>.Create(OnWorkshopEULAStatus);
            COnAvatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
            PendingItemCR = CallResult<CreateItemResult_t>.Create(OnCreateItem);
            PendingItemUpdateCR = CallResult<SubmitItemUpdateResult_t>.Create(OnItemUpdate);

            AppID = SteamUtils.GetAppID();
            if (AppID == AppId_t.Invalid)
            {
                ShowError(Localize.Tr(
                    "GetAppID failed",
                    "MainForm.SteamApiGetAppIdFail")
                );
                Application.Exit();
                return;
            }

            if (AppID != (AppId_t)appId)
            {
                ShowError(Localize.Tr(
                    "GetAppID result {0} does not match expected App ID {1}, expect bugs!",
                    "MainForm.AppIdMismatch",
                    AppID, appId)
                );
            }

            Text = Localize.Tr(Text, "MainForm.Text", AppID);
            GameWorkshopLinkLabel.Text = Localize.Tr(GameWorkshopLinkLabel.Text, "MainForm.GameWorkshopLinkLabel.Text");

            SteamID = SteamUser.GetSteamID();
            if (!SteamID.IsValid())
            {
                ShowError(Localize.Tr(
                    "GetSteamID failed (or invalid)",
                    "MainForm.GetSteamIdFail")
                );
                Application.Exit();
                return;
            }

            var personaName = SteamFriends.GetPersonaName();
            if (string.IsNullOrEmpty(personaName))
            {
                ShowError(Localize.Tr(
                    "GetPersonaName failed",
                    "MainForm.GetPersonaNameFail")
                );
                Application.Exit();
                return;
            }
            PersonaNameLabel.Text = personaName; // NOLOC

            var eulaCall = SteamUGC.GetWorkshopEULAStatus();
            if (eulaCall != SteamAPICall_t.Invalid)
            {
                CROnWorkshopEULAStatus.Set(eulaCall, OnWorkshopEULAStatus);
            }

            SteamIDLabel.Text = SteamID.ToString(); // NOLOC
            var imgHandle = SteamFriends.GetLargeFriendAvatar(SteamID);
            SetAvatarToUI(imgHandle);
            LoadAllItems();

            var toolVerNative = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ToolVersionLabel.Text = Localize.Tr(
                ToolVersionLabel.Text,
                "MainForm.ToolVersionLabel.Text",
                toolVerNative);

            // now we can enable the timer and dispatch callbacks/callresults
            SteamTimer.Enabled = true;
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
                        ProgressLabel.Text = Localize.Tr(
                            "Waiting for CreateItem to complete...",
                            "MainForm.ProgressLabel.Text.Creating");
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
                        // this moved to another place...
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
                            ProgressLabel.Text =
                                Localize.Tr(
                                    "Updating item (status {0}): {1} out of {2} bytes",
                                    "MainForm.ProgressLabel.Text.Updating",
                                    statusString, newProcessed, newTotal);
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
            Debug.WriteLine(Localize.Tr(
                "Shutting down Steam API...",
                "MainForm.OnClosedDebug")
            );
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
                ShowError(Localize.Tr(
                    "StartItemUpdate failed (do you have access to the item?)",
                    "MainForm.StartItemUpdateFail")
                );
                return;
            }

            if (!SteamUGC.SetItemUpdateLanguage(update, "english"))
            {
                ShowError(Localize.Tr(
                    "SetItemUpdateLanguage failed",
                    "MainForm.SetItemUpdateLanguageFail")
                );
                return;
            }

            if (!string.IsNullOrEmpty(item.Title) && item.Dirty.HasFlag(UGCItem.DirtyFlags.Title))
            {
                if (!SteamUGC.SetItemTitle(update, item.Title))
                {
                    ShowError(Localize.Tr(
                        "SetItemTitle failed (try shortening the title)",
                        "MainForm.SetItemTitleFail")
                    );
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(item.Description) && item.Dirty.HasFlag(UGCItem.DirtyFlags.Description))
            {
                if (!SteamUGC.SetItemDescription(update, item.Description))
                {
                    ShowError(Localize.Tr(
                        "SetItemDescription failed (try shortening the description)",
                        "MainForm.SetItemDescriptionFail")
                    );
                    return;
                }
            }
            
            if (!SteamUGC.SetItemVisibility(update, item.Visibility))
            {
                ShowError(Localize.Tr(
                    "SetItemVisibility failed (try setting Private)",
                    "MainForm.SetItemVisibilityFail")
                );
                return;
            }

            if (!string.IsNullOrWhiteSpace(item.ContentsFolder))
            {
                if (!SteamUGC.SetItemContent(update, item.ContentsFolder))
                {
                    ShowError(Localize.Tr(
                        "SetItemContent failed (do you have access to the folder?)",
                        "MainForm.SetItemContentFail")
                    );
                    return;
                }
            }

            if (item.Preview != null && item.Dirty.HasFlag(UGCItem.DirtyFlags.Preview))
            {
                var tmpname = IsPNGHeader(item.Preview) ? "00a_preview.png" : "00a_preview.jpg";
                var tmpfile = Path.Combine(Path.GetTempPath(),tmpname);

                try
                {
                    File.WriteAllBytes(tmpfile, item.Preview);
                }
                catch
                {
                    ShowError(Localize.Tr(
                        "Failed to write the preview image.",
                        "MainForm.ImageWriteFail")
                    );
                    return;
                }

                if (!SteamUGC.SetItemPreview(update, tmpfile))
                {
                    ShowError(Localize.Tr(
                        "SetItemPreview failed (is the picture valid? try compressing it)",
                        "MainForm.SetItemPreviewFail")
                    );
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
                ShowError(Localize.Tr(
                    "SubmitItemUpdate failed",
                    "MainForm.SubmitItemUpdateFail")
                );
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
                    ShowError(Localize.Tr(
                        "CreateItem call failed (in PublishItem)",
                        "MainForm.CreateItemInPublishFail")
                    );
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

        private void AvatarPictureBox_Click(object sender, EventArgs e)
        {
            ProcessStart($"https://steamcommunity.com/profiles/{SteamID}");
        }

        private void RefreshItemsButton_Click(object sender, EventArgs e)
        {
            // this should be guarded by UILockState hopefully...
            LoadAllItems();
        }
    }
}

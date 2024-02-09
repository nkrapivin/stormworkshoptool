using Steamworks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormWorkshopTool
{
    /// <summary>
    /// This class is shared between ItemEditForm and MainForm to describe a UGC item
    /// </summary>
    internal class UGCItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContentsFolder { get; set; }
        public string Changelog { get; set; }
        public UGCHandle_t PreviewHandle { get; set; }
        public byte[] Preview { get; set; }
        public PublishedFileId_t Id { get; set; }
        public UGCUpdateHandle_t UpdateHandle { get; set; }
        public ERemoteStoragePublishedFileVisibility Visibility { get; set; }
    }
}

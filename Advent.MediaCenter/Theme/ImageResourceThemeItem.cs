


using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Advent.MediaCenter.Theme
{
    public class ImageResourceThemeItem : ResourceThemeItem
    {
        private BitmapSource image;

        public BitmapSource Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (this.image == value)
                    return;
                this.image = value;
                this.OnPropertyChanged("Image");
            }
        }

        public override int ResourceType
        {
            get
            {
                return 23;
            }
        }

        public ImageResourceThemeItem(string dllName, string resourceName, byte[] buffer)
            : base(dllName, resourceName, buffer)
        {
        }

        public ImageResourceThemeItem(string dllName, string resourceName, Func<ResourceThemeItem, byte[]> buffer)
            : base(dllName, resourceName, buffer)
        {
        }

        protected override byte[] Save()
        {
            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(this.Image));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                pngBitmapEncoder.Save((Stream)memoryStream);
                return memoryStream.GetBuffer();
            }
        }

        protected override void LoadInternal()
        {
            this.Image = (BitmapSource)BitmapDecoder.Create((Stream)new MemoryStream(this.OriginalBuffer), BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default).Frames[0];
            this.IsDirty = false;
        }
    }
}

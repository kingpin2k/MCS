


using Advent.Common.Serialization;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Advent.MediaCenter.StartMenu.OEM
{
    [Flags]
    public enum EntryPointCapabilities
    {
        None = 0,
        Audio = 1,
        CdBurning = 2,
        Console = 4,
        DirectX = 8,
        IntensiveRendering = 16,
        Video = 32,
    }

    public class EntryPoint : ApplicationRefObject, IEquatable<EntryPoint>
    {
        public static readonly DependencyPropertyKey ImageUrlPropertyKey = DependencyProperty.RegisterReadOnly("ImageUrl", typeof(string), typeof(EntryPoint), new PropertyMetadata(new PropertyChangedCallback(EntryPoint.ImageUrlChanged)));
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(string), typeof(EntryPoint));
        public static readonly DependencyProperty RawDescriptionProperty = DependencyProperty.Register("RawDescription", typeof(string), typeof(EntryPoint));
        public static readonly DependencyProperty ImageUrlProperty = EntryPoint.ImageUrlPropertyKey.DependencyProperty;
        public static readonly DependencyProperty RawImageUrlProperty = DependencyProperty.Register("RawImageUrl", typeof(string), typeof(EntryPoint), new PropertyMetadata(new PropertyChangedCallback(EntryPoint.RawImageUrlChanged)));
        public static readonly DependencyProperty ThumbnailUrlProperty = DependencyProperty.Register("ThumbnailUrl", typeof(string), typeof(EntryPoint), new PropertyMetadata(new PropertyChangedCallback(EntryPoint.ThumbnailUrlChanged)));
        public static readonly DependencyProperty InactiveImageUrlProperty = DependencyProperty.Register("InactiveImageUrl", typeof(string), typeof(EntryPoint), new PropertyMetadata(new PropertyChangedCallback(EntryPoint.InactiveImageUrlChanged)));
        public static readonly DependencyProperty ImageOverrideProperty = DependencyProperty.Register("ImageOverride", typeof(ImageSource), typeof(EntryPoint), new PropertyMetadata(new PropertyChangedCallback(EntryPoint.ImageOverrideChanged)));
        public static readonly DependencyProperty InactiveImageOverrideProperty = DependencyProperty.Register("InactiveImageOverride", typeof(ImageSource), typeof(EntryPoint), new PropertyMetadata(new PropertyChangedCallback(EntryPoint.InactiveImageOverrideChanged)));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(EntryPoint), new PropertyMetadata(new PropertyChangedCallback(EntryPoint.TitleChanged)));
        public static readonly DependencyProperty RawTitleProperty = DependencyProperty.Register("RawTitle", typeof(string), typeof(EntryPoint), new PropertyMetadata(new PropertyChangedCallback(EntryPoint.RawTitleChanged)));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(EntryPoint));
        public static readonly DependencyProperty NonFocusImageProperty = DependencyProperty.Register("NonFocusImage", typeof(ImageSource), typeof(EntryPoint));
        public static readonly DependencyProperty RunProperty = DependencyProperty.Register("Run", typeof(string), typeof(EntryPoint));
        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register("Url", typeof(string), typeof(EntryPoint));
        public static readonly DependencyProperty AddInProperty = DependencyProperty.Register("AddIn", typeof(string), typeof(EntryPoint));
        public static readonly DependencyProperty ContextProperty = DependencyProperty.Register("Context", typeof(string), typeof(EntryPoint));
        public static readonly DependencyProperty NowPlayingDirectiveProperty = DependencyProperty.Register("NowPlayingDirective", typeof(string), typeof(EntryPoint));
        public static readonly DependencyProperty UiFlagsProperty = DependencyProperty.Register("UiFlags", typeof(string), typeof(EntryPoint));
        public static readonly DependencyProperty CapabilitiesRequiredProperty = DependencyProperty.Register("CapabilitiesRequired", typeof(EntryPointCapabilities), typeof(EntryPoint), new PropertyMetadata((object)EntryPointCapabilities.None));

        public ImageSource Image
        {
            get
            {
                return (ImageSource)this.GetValue(EntryPoint.ImageProperty);
            }
        }

        public ImageSource NonFocusImage
        {
            get
            {
                return (ImageSource)this.GetValue(EntryPoint.NonFocusImageProperty);
            }
        }

        [RegistryKeyName]
        public string ID
        {
            get
            {
                return (string)this.GetValue(EntryPoint.IDProperty);
            }
            set
            {
                this.SetValue(EntryPoint.IDProperty, (object)value);
            }
        }

        public string Description
        {
            get
            {
                return OemManager.ResolveString(this.RawDescription);
            }
        }

        [RegistryValue("Description")]
        public string RawDescription
        {
            get
            {
                return (string)this.GetValue(EntryPoint.RawDescriptionProperty);
            }
            set
            {
                this.SetValue(EntryPoint.RawDescriptionProperty, (object)value);
            }
        }

        public string ImageUrl
        {
            get
            {
                return (string)this.GetValue(EntryPoint.ImageUrlProperty);
            }
        }

        [RegistryValue("ImageUrl")]
        public string RawImageUrl
        {
            get
            {
                return (string)this.GetValue(EntryPoint.RawImageUrlProperty);
            }
            set
            {
                this.SetValue(EntryPoint.RawImageUrlProperty, (object)value);
            }
        }

        [RegistryValue("InactiveImageURL")]
        public string InactiveImageUrl
        {
            get
            {
                return (string)this.GetValue(EntryPoint.InactiveImageUrlProperty);
            }
            set
            {
                this.SetValue(EntryPoint.InactiveImageUrlProperty, (object)value);
            }
        }

        [RegistryValue]
        public string NowPlayingDirective
        {
            get
            {
                return (string)this.GetValue(EntryPoint.NowPlayingDirectiveProperty);
            }
            set
            {
                this.SetValue(EntryPoint.NowPlayingDirectiveProperty, (object)value);
            }
        }

        [RegistryValue]
        public string UiFlags
        {
            get
            {
                return (string)this.GetValue(EntryPoint.UiFlagsProperty);
            }
            set
            {
                this.SetValue(EntryPoint.UiFlagsProperty, (object)value);
            }
        }

        public EntryPointCapabilities CapabilitiesRequired
        {
            get
            {
                return (EntryPointCapabilities)this.GetValue(EntryPoint.CapabilitiesRequiredProperty);
            }
            set
            {
                this.SetValue(EntryPoint.CapabilitiesRequiredProperty, (object)value);
            }
        }

        [RegistryValue]
        public string ThumbnailUrl
        {
            get
            {
                return (string)this.GetValue(EntryPoint.ThumbnailUrlProperty);
            }
            set
            {
                this.SetValue(EntryPoint.ThumbnailUrlProperty, (object)value);
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(EntryPoint.TitleProperty);
            }
            set
            {
                this.SetValue(EntryPoint.TitleProperty, (object)value);
            }
        }

        [RegistryValue("Title")]
        public string RawTitle
        {
            get
            {
                return (string)this.GetValue(EntryPoint.RawTitleProperty);
            }
            set
            {
                this.SetValue(EntryPoint.RawTitleProperty, (object)value);
            }
        }

        [RegistryValue("AppId")]
        public override string ApplicationID
        {
            get
            {
                return base.ApplicationID;
            }
            set
            {
                base.ApplicationID = value;
            }
        }

        [RegistryValue]
        public string Run
        {
            get
            {
                return (string)this.GetValue(EntryPoint.RunProperty);
            }
            set
            {
                this.SetValue(EntryPoint.RunProperty, (object)value);
            }
        }

        [RegistryValue]
        public string Url
        {
            get
            {
                return (string)this.GetValue(EntryPoint.UrlProperty);
            }
            set
            {
                this.SetValue(EntryPoint.UrlProperty, (object)value);
            }
        }

        [RegistryValue]
        public string AddIn
        {
            get
            {
                return (string)this.GetValue(EntryPoint.AddInProperty);
            }
            set
            {
                this.SetValue(EntryPoint.AddInProperty, (object)value);
            }
        }

        [RegistryValue]
        public string Context
        {
            get
            {
                return (string)this.GetValue(EntryPoint.ContextProperty);
            }
            set
            {
                this.SetValue(EntryPoint.ContextProperty, (object)value);
            }
        }

        public ImageSource ImageOverride
        {
            get
            {
                return (ImageSource)this.GetValue(EntryPoint.ImageOverrideProperty);
            }
            set
            {
                this.SetValue(EntryPoint.ImageOverrideProperty, (object)value);
            }
        }

        public ImageSource InactiveImageOverride
        {
            get
            {
                return (ImageSource)this.GetValue(EntryPoint.InactiveImageOverrideProperty);
            }
            set
            {
                this.SetValue(EntryPoint.InactiveImageOverrideProperty, (object)value);
            }
        }

        static EntryPoint()
        {
        }

        public override string ToString()
        {
            return this.Title;
        }

        public void EnsureValidForMenu()
        {
            if (this.RawImageUrl != null || this.ThumbnailUrl == null)
                return;
            this.RawImageUrl = this.ThumbnailUrl;
            this.IsDirty = true;
        }

        public void Save()
        {
            if (!this.IsDirty && this.IsSaved)
                return;
            using (RegistryKey subKey = this.RegHive.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Entry Points"))
                this.Manager.RegistrySerialiser.Serialise((object)this, subKey);
        }

        bool IEquatable<EntryPoint>.Equals(EntryPoint other)
        {
            return this.ID == other.ID;
        }

        protected override void OnApplicationChanged(Advent.MediaCenter.StartMenu.OEM.Application oldApp, Advent.MediaCenter.StartMenu.OEM.Application newApp)
        {
            if (oldApp != null)
                oldApp.EntryPoints.Remove(this);
            if (newApp == null || newApp.EntryPoints.Contains(this))
                return;
            newApp.EntryPoints.Add(this);
        }

        protected override bool ShouldIgnorePropertyChange(DependencyPropertyChangedEventArgs e)
        {
            if (!base.ShouldIgnorePropertyChange(e) && e.Property != EntryPoint.ImageProperty)
                return e.Property == EntryPoint.NonFocusImageProperty;
            else
                return true;
        }

        protected override void OnAfterSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            if (this.CapabilitiesRequired != EntryPointCapabilities.None)
            {
                string str = ((object)this.CapabilitiesRequired).ToString().ToLower();
                key.SetValue("CapabilitiesRequired", (object)str);
            }
            else
                key.DeleteValue("CapabilitiesRequired", false);
            if (this.Application == null || this.Application.IsSaved)
                return;
            using (RegistryKey subKey = this.Application.RegHive.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Applications"))
                rs.Serialise((object)this.Application, subKey);
        }

        protected override void OnAfterDeserialise(RegistrySerialiser rs, RegistryKey key)
        {
            string text = key.GetValue("CapabilitiesRequired") as string;
            if (text != null)
            {
                try
                {
                    this.CapabilitiesRequired = (EntryPointCapabilities)new EnumConverter(typeof(EntryPointCapabilities)).ConvertFromString(text);
                }
                catch
                {
                }
            }
            if (this.ApplicationID == null)
                return;
            this.Application = this.Manager.Applications[this.ApplicationID];
        }

        private static void RawTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            sender.SetValue(EntryPoint.TitleProperty, (object)OemManager.ResolveString((string)args.NewValue));
        }

        private static void TitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!((string)args.NewValue != (string)args.OldValue))
                return;
            sender.SetValue(EntryPoint.RawTitleProperty, args.NewValue);
        }

        private static void InactiveImageOverrideChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((EntryPoint)sender).UpdateImages();
        }

        private static void ImageOverrideChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((EntryPoint)sender).UpdateImages();
        }

        private static void InactiveImageUrlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((EntryPoint)sender).UpdateImages();
        }

        private static void ThumbnailUrlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!string.IsNullOrEmpty((string)sender.GetValue(EntryPoint.ImageUrlProperty)))
                return;
            sender.SetValue(EntryPoint.ImageUrlPropertyKey, args.NewValue);
        }

        private static void ImageUrlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((EntryPoint)sender).UpdateImages();
        }

        private static void RawImageUrlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            sender.SetValue(EntryPoint.ImageUrlPropertyKey, args.NewValue);
        }

        private ImageSource ImageFromUrl(string url)
        {
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                return (ImageSource)BitmapFrame.Create(new Uri(url));
            try
            {
                return (ImageSource)BitmapFrame.Create((Stream)new MemoryStream(File.ReadAllBytes(url)));
            }
            catch
            {
            }
            return (ImageSource)null;
        }

        private void UpdateImages()
        {
            ImageSource imageSource1 = this.ImageOverride;
            ImageSource imageSource2 = this.InactiveImageOverride;
            if (imageSource1 == null && !string.IsNullOrEmpty(this.ImageUrl))
                imageSource1 = this.ImageFromUrl(this.ImageUrl);
            if (imageSource2 == null && this.InactiveImageUrl != null && !string.IsNullOrEmpty(this.InactiveImageUrl))
                imageSource2 = this.ImageFromUrl(this.InactiveImageUrl);
            if (imageSource2 == null && imageSource1 != null && imageSource1 is BitmapSource)
                imageSource2 = (ImageSource)new FormatConvertedBitmap((BitmapSource)imageSource1, PixelFormats.Gray32Float, (BitmapPalette)null, 0.0);
            this.SetValue(EntryPoint.ImageProperty, (object)imageSource1);
            this.SetValue(EntryPoint.NonFocusImageProperty, (object)imageSource2);
        }
    }
}

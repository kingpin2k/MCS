


using Advent.MediaCenter;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Advent.MediaCenter.StartMenu
{
    internal class NowPlayingQuickLink : IQuickLink, ISupportInitialize
    {
        private static ImageSource image = (ImageSource)Imaging.CreateBitmapSourceFromHBitmap(Resources.Play.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        private readonly StartMenuManager startMenuManager;

        public string Title
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        public ImageSource Image
        {
            get
            {
                return NowPlayingQuickLink.image;
            }
        }

        public ImageSource NonFocusImage
        {
            get
            {
                return this.Image;
            }
        }

        public int Priority
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool CanSetEnabled
        {
            get
            {
                return false;
            }
        }

        public bool CanDelete
        {
            get
            {
                return false;
            }
        }

        public bool CanEditTitle
        {
            get
            {
                return false;
            }
        }

        public IMenuStrip OriginalStrip
        {
            get
            {
                return (IMenuStrip)null;
            }
        }

        static NowPlayingQuickLink()
        {
        }

        public NowPlayingQuickLink(StartMenuManager smm)
        {
            this.startMenuManager = smm;
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }
    }
}

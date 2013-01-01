


using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Windows7
{
    internal class FavouritePartnerQuickLink : Windows7QuickLinkBase
    {
        private string favouriteIndex;

        public override bool CanSetEnabled
        {
            get
            {
                return true;
            }
        }

        public override bool CanDelete
        {
            get
            {
                return false;
            }
        }

        public override bool CanEditTitle
        {
            get
            {
                return false;
            }
        }

        public FavouritePartnerQuickLink(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
            this.SetValue(XmlQuickLink.ImageProperty, (object)Imaging.CreateBitmapSourceFromHBitmap(Resources.Favourites.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
        }

        protected override void Load()
        {
            this.favouriteIndex = this.LinkElement.GetAttribute("FavoriteIndex");
            this.Title = string.Format("Favourite ({0})", (object)this.favouriteIndex);
        }
    }
}

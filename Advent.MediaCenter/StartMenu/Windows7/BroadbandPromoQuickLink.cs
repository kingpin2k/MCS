


using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Windows7
{
    internal class BroadbandPromoQuickLink : Windows7QuickLinkBase
    {
        private static ImageSource image = (ImageSource)Imaging.CreateBitmapSourceFromHBitmap(Resources.Question.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        private string promoGroupID;

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

        static BroadbandPromoQuickLink()
        {
        }

        public BroadbandPromoQuickLink(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
            this.SetValue(XmlQuickLink.ImageProperty, (object)BroadbandPromoQuickLink.image);
        }

        protected override void Load()
        {
            this.promoGroupID = this.LinkElement.GetAttribute("PromoGroupId");
            this.Title = string.Format("Promo ({0})", (object)this.promoGroupID);
        }
    }
}




using Advent.MediaCenter.StartMenu.OEM;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    internal class OemCategoryStrip : IMenuStrip, ISupportInitialize
    {
        private readonly ObservableCollection<IQuickLink> quickLinks;
        private readonly string title;
        private readonly string[] categories;
        private readonly PartnerQuickLink quickLink;

        public ObservableCollection<IQuickLink> QuickLinks
        {
            get
            {
                return this.quickLinks;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
            }
        }

        public int Priority
        {
            get
            {
                return int.MaxValue;
            }
            set
            {
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
            }
        }

        public bool CanSetLinkPriority
        {
            get
            {
                return false;
            }
        }

        public bool CanSetPriority
        {
            get
            {
                return false;
            }
        }

        public bool CanSetEnabled
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

        public bool CanDelete
        {
            get
            {
                return false;
            }
        }

        public OemCategoryStrip(StartMenuManager manager, string title, string[] categories)
        {
            this.title = title;
            this.categories = categories;
            this.quickLinks = (ObservableCollection<IQuickLink>)new OemCategoryStrip.SingleQuickLinkCollection();
            this.quickLink = (PartnerQuickLink)new OemCategoryStrip.OemCategoryLink(manager, this.categories);
            this.quickLink.BeginInit();
            this.quickLink.EndInit();
            this.QuickLinks.Add((IQuickLink)this.quickLink);
        }

        public bool CanSwapWith(IMenuStrip strip)
        {
            return false;
        }

        public bool CanAddQuickLink(IQuickLink link)
        {
            if (link is OemQuickLink)
                return this.quickLink.OemQuickLink == null;
            else
                return false;
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }

        private class OemCategoryLink : PartnerQuickLink
        {
            private readonly string[] categories;

            protected override string[] Categories
            {
                get
                {
                    return this.categories;
                }
            }

            public OemCategoryLink(StartMenuManager manager, string[] categories)
                : base(manager, (XmlElement)null)
            {
                this.categories = categories;
            }
        }

        private class SingleQuickLinkCollection : ObservableCollection<IQuickLink>
        {
            private PartnerQuickLink singleLink;

            protected override void InsertItem(int index, IQuickLink item)
            {
                if (this.Count == 0)
                {
                    if (!(item is PartnerQuickLink))
                        throw new ArgumentException();
                    this.singleLink = (PartnerQuickLink)item;
                    base.InsertItem(index, item);
                }
                else
                {
                    OemQuickLink oemQuickLink = item as OemQuickLink;
                    if (oemQuickLink == null)
                    {
                        PartnerQuickLink partnerQuickLink = item as PartnerQuickLink;
                        if (partnerQuickLink != null)
                            oemQuickLink = partnerQuickLink.OemQuickLink;
                    }
                    if (oemQuickLink == null)
                        return;
                    this.singleLink.OemQuickLink = oemQuickLink;
                }
            }

            protected override void RemoveItem(int index)
            {
                if (index != 0)
                    return;
                this.singleLink.OemQuickLink = (OemQuickLink)null;
            }
        }
    }
}

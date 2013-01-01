


using Advent.MediaCenter.StartMenu.OEM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    internal abstract class PartnerQuickLink : PartnerQuickLinkBase, IPartnerQuickLink, IQuickLink, ISupportInitialize
    {
        private string[] categories;
        private OemCategory category;

        protected abstract string[] Categories { get; }

        public PartnerQuickLink(StartMenuManager manager, XmlElement element)
            : base(manager, element)
        {
        }

        protected override void Load()
        {
            base.Load();
            this.categories = this.Categories;
            if (this.categories != null)
            {
                foreach (string index in this.categories)
                {
                    OemCategory oemCategory = this.Manager.OemManager.Categories[index];
                    if (oemCategory != null)
                        oemCategory.QuickLinks.CollectionChanged += new NotifyCollectionChangedEventHandler(this.QuickLinks_CollectionChanged);
                }
            }
            this.OemQuickLink = this.GetBestQuickLink();
        }

        protected OemQuickLink GetBestQuickLink()
        {
            List<IQuickLink> list = new List<IQuickLink>();
            Dictionary<IQuickLink, OemCategory> dictionary = new Dictionary<IQuickLink, OemCategory>();
            foreach (string index in this.categories)
            {
                OemCategory oemCategory = this.Manager.OemManager.Categories[index];
                if (oemCategory != null)
                {
                    IQuickLink newestQuickLink = PartnerQuickLink.GetNewestQuickLink((IList<IQuickLink>)oemCategory.QuickLinks);
                    if (newestQuickLink != null)
                    {
                        list.Add(newestQuickLink);
                        dictionary[newestQuickLink] = oemCategory;
                    }
                }
            }
            if (list.Count > 0)
            {
                IQuickLink newestQuickLink = PartnerQuickLink.GetNewestQuickLink((IList<IQuickLink>)list);
                this.category = dictionary[newestQuickLink];
                return (OemQuickLink)newestQuickLink;
            }
            else
            {
                this.category = this.Manager.OemManager.Categories[this.categories[0]];
                return (OemQuickLink)null;
            }
        }

        protected override void OnQuickLinkChanged(OemQuickLink oldValue, OemQuickLink newValue)
        {
            if (this.IsInitInProgress)
                return;
            if (oldValue != null)
                this.category.QuickLinks.Remove((IQuickLink)oldValue);
            if (newValue != null)
            {
                ObservableCollection<IQuickLink> quickLinks = this.category.QuickLinks;
                if (!quickLinks.Contains((IQuickLink)newValue))
                    quickLinks.Add((IQuickLink)newValue);
            }
            OemQuickLink bestQuickLink = this.GetBestQuickLink();
            if (bestQuickLink == newValue)
                return;
            this.OemQuickLink = bestQuickLink;
        }

        private static IQuickLink GetNewestQuickLink(IList<IQuickLink> links)
        {
            return PartnerQuickLink.GetNewestQuickLink(links, true) ?? PartnerQuickLink.GetNewestQuickLink(links, false);
        }

        private static IQuickLink GetNewestQuickLink(IList<IQuickLink> links, bool isEnabled)
        {
            IQuickLink quickLink1 = (IQuickLink)null;
            if (links != null)
            {
                for (int index = 0; index < links.Count; ++index)
                {
                    IQuickLink quickLink2 = links[index];
                    if (quickLink2 != null && quickLink2.IsEnabled == isEnabled && (quickLink1 == null || quickLink1.Priority < quickLink2.Priority))
                        quickLink1 = quickLink2;
                }
            }
            return quickLink1;
        }

        private void QuickLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OemQuickLink = this.GetBestQuickLink();
        }
    }
}

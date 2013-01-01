


using Advent.Common.Serialization;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Advent.MediaCenter.StartMenu.OEM
{
    public class OemCategory : MediaCenterRegistryObject
    {
        private readonly OemCategory.OemQuickLinkCollection categoryQuickLinks;
        private readonly List<string> deletedLinks;
        private string categoryName;

        public string CategoryPath
        {
            get
            {
                return this.categoryName;
            }
            set
            {
                this.categoryName = value;
            }
        }

        [RegistryKeyName]
        public string CategoryName
        {
            get
            {
                return this.categoryName.Substring(this.CategoryPath.LastIndexOf('\\') + 1);
            }
            set
            {
            }
        }

        public string ParentCategory
        {
            get
            {
                int length = this.CategoryPath.LastIndexOf('\\');
                if (length >= 1)
                    return this.categoryName.Substring(0, length);
                else
                    return string.Empty;
            }
        }

        public ObservableCollection<IQuickLink> QuickLinks
        {
            get
            {
                return (ObservableCollection<IQuickLink>)this.categoryQuickLinks;
            }
        }

        public OemCategory()
        {
            this.deletedLinks = new List<string>();
            this.categoryQuickLinks = new OemCategory.OemQuickLinkCollection(this);
            this.categoryQuickLinks.CollectionChanged += new NotifyCollectionChangedEventHandler(this.QuickLinks_CollectionChanged);
        }

        public override bool DeleteKey()
        {
            if (!this.IsSaved)
                return false;
            bool flag = true;
            foreach (MediaCenterRegistryObject centerRegistryObject in (Collection<IQuickLink>)this.QuickLinks)
            {
                if (!centerRegistryObject.DeleteKey())
                    flag = false;
            }
            if (flag)
                flag = this.DeleteIfEmpty();
            return flag;
        }

        public void Save()
        {
            if (!this.IsDirty && this.IsSaved)
                return;
            using (RegistryKey subKey = this.RegHive.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories\\" + this.ParentCategory))
                this.Manager.RegistrySerialiser.Serialise((object)this, subKey);
        }

        protected override void OnAfterSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            this.PurgeDeletedLinks(key);
            foreach (OemQuickLink oemQuickLink in (Collection<IQuickLink>)this.categoryQuickLinks)
                rs.Serialise((object)oemQuickLink, key);
            base.OnAfterSerialise(rs, key);
        }

        protected override void OnAfterDeserialise(RegistrySerialiser rs, RegistryKey key)
        {
            List<IQuickLink> list = new List<IQuickLink>();
            foreach (string str in key.GetSubKeyNames())
            {
                if (MediaCenterUtil.IsGuid(str))
                {
                    using (RegistryKey key1 = key.OpenSubKey(str))
                    {
                        OemQuickLink l = (OemQuickLink)rs.Deserialise(this.Manager.QuickLinkType, key1);
                        OemQuickLink quickLink = this.GetQuickLink(l);
                        if (quickLink != null)
                        {
                            rs.Deserialise((object)quickLink, key1);
                            list.Add((IQuickLink)quickLink);
                        }
                        else if (l.EntryPoint != null)
                        {
                            if (l.Application != null)
                                list.Add((IQuickLink)l);
                        }
                    }
                }
            }
            foreach (IQuickLink quickLink in list)
            {
                if (!this.categoryQuickLinks.Contains(quickLink))
                {
                    int index = 0;
                    while (index < this.categoryQuickLinks.Count && quickLink.Priority >= this.categoryQuickLinks[index].Priority)
                        ++index;
                    this.categoryQuickLinks.Insert(index, quickLink);
                }
            }
            this.categoryName = key.Name.Substring(key.Name.LastIndexOf("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories") + "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories".Length + 1);
            base.OnAfterDeserialise(rs, key);
        }

        private OemQuickLink GetQuickLink(OemQuickLink l)
        {
            foreach (OemQuickLink oemQuickLink in (Collection<IQuickLink>)this.QuickLinks)
            {
                if (oemQuickLink.EntryPointID.ToUpper() == l.EntryPointID.ToUpper())
                    return oemQuickLink;
            }
            return (OemQuickLink)null;
        }

        private bool DeleteIfEmpty()
        {
            bool flag = true;
            string str = this.IsSaved ? this.RegPath : "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories\\" + this.CategoryPath;
            using (RegistryKey key = this.RegHive.OpenSubKey(str, true))
            {
                if (key != null)
                {
                    this.PurgeDeletedLinks(key);
                    if (key.SubKeyCount == 0)
                    {
                        if (this.IsSaved)
                            flag = base.DeleteKey();
                        else
                            this.RegHive.DeleteSubKey(str);
                        if (!string.IsNullOrEmpty(this.ParentCategory))
                        {
                            OemCategory oemCategory = this.Manager.Categories[this.ParentCategory];
                            if (oemCategory != null)
                                oemCategory.DeleteIfEmpty();
                        }
                    }
                }
            }
            return flag;
        }

        private void PurgeDeletedLinks(RegistryKey key)
        {
            foreach (string subkey1 in this.deletedLinks)
            {
                key.DeleteSubKey(subkey1, false);
                foreach (object obj in Registry.Users.GetSubKeyNames())
                {
                    string subkey2 = string.Format("{0}\\{1}\\{2}", obj, (object)key.Name.Substring(key.Name.IndexOf('\\') + 1), (object)subkey1);
                    Registry.Users.DeleteSubKey(subkey2, false);
                }
            }
            this.deletedLinks.Clear();
        }

        private void QuickLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInInit)
                return;
            this.IsDirty = true;
        }

        private class OemQuickLinkCollection : ObservableCollection<IQuickLink>
        {
            private readonly OemCategory oemCategory;

            public OemQuickLinkCollection(OemCategory category)
            {
                this.oemCategory = category;
            }

            protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                bool flag = false;
                int num = -1;
                foreach (OemQuickLink oemQuickLink in (Collection<IQuickLink>)this)
                {
                    if (oemQuickLink.Priority > num)
                        num = oemQuickLink.Priority;
                    else
                        flag = true;
                }
                if (flag)
                {
                    for (int index = 0; index < this.Count; ++index)
                    {
                        OemQuickLink oemQuickLink = (OemQuickLink)this[index];
                        if (this.oemCategory.IsInInit)
                            oemQuickLink.BeginInit();
                        oemQuickLink.Priority = index;
                        if (this.oemCategory.IsInInit)
                            oemQuickLink.EndInit();
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (OemQuickLink oemQuickLink in (IEnumerable)e.OldItems)
                        this.oemCategory.deletedLinks.Add(oemQuickLink.EntryPointID.ToUpper());
                }
                if (e.NewItems != null)
                {
                    foreach (OemQuickLink oemQuickLink in (IEnumerable)e.NewItems)
                    {
                        this.oemCategory.deletedLinks.Remove(oemQuickLink.EntryPointID.ToUpper());
                        oemQuickLink.EntryPoint.EnsureValidForMenu();
                    }
                }
                base.OnCollectionChanged(e);
            }

            protected override void InsertItem(int index, IQuickLink item)
            {
                IQuickLink quickLink = item;
                IPartnerQuickLink partnerQuickLink = item as IPartnerQuickLink;
                if (partnerQuickLink != null)
                    quickLink = (IQuickLink)partnerQuickLink.OemQuickLink;
                base.InsertItem(index, quickLink);
            }
        }
    }
}

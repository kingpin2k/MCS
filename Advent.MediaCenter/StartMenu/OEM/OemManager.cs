


using Advent.Common;
using Advent.Common.Interop;
using Advent.Common.IO;
using Advent.Common.Serialization;
using Advent.Common.UI;
using Advent.MediaCenter;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Advent.MediaCenter.StartMenu.OEM
{
    public class OemManager : NotifyPropertyChangedBase
    {
        internal const string BaseKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center";
        internal const string StartMenuKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Start Menu";
        internal const string StartMenuAppsKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Start Menu\\Applications";
        internal const string ExtensibilityBaseKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility";
        internal const string ApplicationsKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Applications";
        internal const string EntryPointsKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Entry Points";
        internal const string CategoriesKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories";
        internal const string BackgroundCategoryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories\\Background";
        private OemManager.ApplicationCollection managerApps;
        private OemManager.EntryPointCollection managerEntryPoints;
        private OemManager.CategoryCollection managerCategories;
        private ObservableCollection<OemMenuStrip> managerStrips;
        private List<OemMenuStrip> managerDeletedStrips;
        private RegistrySerialiser managerRs;
        private bool isDirty;

        public bool IsDirty
        {
            get
            {
                if (!this.isDirty)
                    return this.DirtyObjects.Count > 0;
                else
                    return true;
            }
            private set
            {
                if (value == this.isDirty)
                    return;
                this.isDirty = value;
                this.OnPropertyChanged("IsDirty");
            }
        }

        internal ObservableCollection<MediaCenterRegistryObject> DirtyObjects { get; private set; }

        public OemManager.ApplicationCollection Applications
        {
            get
            {
                return this.managerApps;
            }
        }

        public OemManager.EntryPointCollection EntryPoints
        {
            get
            {
                return this.managerEntryPoints;
            }
        }

        public OemManager.CategoryCollection Categories
        {
            get
            {
                return this.managerCategories;
            }
        }

        public IList<OemMenuStrip> StartMenuStrips
        {
            get
            {
                return (IList<OemMenuStrip>)this.managerStrips;
            }
        }

        public RegistrySerialiser RegistrySerialiser
        {
            get
            {
                return this.managerRs;
            }
        }

        protected internal virtual Type QuickLinkType
        {
            get
            {
                return typeof(OemQuickLink);
            }
        }

        protected virtual Type EntryPointType
        {
            get
            {
                return typeof(EntryPoint);
            }
        }

        protected virtual Type ApplicationType
        {
            get
            {
                return typeof(Application);
            }
        }

        protected virtual Type MenuStripType
        {
            get
            {
                return typeof(OemMenuStrip);
            }
        }

        protected virtual Type CategoryType
        {
            get
            {
                return typeof(OemCategory);
            }
        }

        public OemManager()
        {
            this.DirtyObjects = new ObservableCollection<MediaCenterRegistryObject>();
            this.DirtyObjects.CollectionChanged += new NotifyCollectionChangedEventHandler(this.DirtyObjectsCollectionChanged);
            this.Reset();
        }

        private void DirtyObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("IsDirty");
        }

        public static string ResolveString(string s)
        {
            if (s == null || !s.StartsWith("@"))
                return s;
            s = s.Remove(0, 1);
            string[] strArray = s.Split(new char[1]
      {
        ','
      });
            int result;
            if (strArray.Length != 2 || !int.TryParse(strArray[1], out result))
                return s;
            int id = Math.Abs(result);
            using (IResourceLibrary library = (IResourceLibrary)new UnmanagedLibrary(strArray[0]))
                return ResourceExtensions.GetStringResource(library, id);
        }

        public void Reset()
        {
            this.managerApps = new OemManager.ApplicationCollection(this);
            this.managerEntryPoints = new OemManager.EntryPointCollection(this);
            this.managerCategories = new OemManager.CategoryCollection(this);
            List<OemMenuStrip> list = new List<OemMenuStrip>();
            this.managerDeletedStrips = new List<OemMenuStrip>();
            this.managerRs = MediaCenterRegistryObject.CreateRegistrySerialiser(this);
            this.BuildInfo((ICollection<OemMenuStrip>)list);
            this.isDirty = false;
            list.Sort((Comparison<OemMenuStrip>)((x, y) => y.TimeStamp.CompareTo(x.TimeStamp)));
            this.managerStrips = new ObservableCollection<OemMenuStrip>(list);
            this.managerStrips.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Strips_CollectionChanged);
        }

        public void Save()
        {
            foreach (MediaCenterRegistryObject centerRegistryObject in this.EntryPoints.DeletedItems)
                centerRegistryObject.DeleteKey();
            this.EntryPoints.ClearDeletedItems();
            foreach (MediaCenterRegistryObject centerRegistryObject in this.Applications.DeletedItems)
                centerRegistryObject.DeleteKey();
            this.Applications.ClearDeletedItems();
            foreach (MediaCenterRegistryObject centerRegistryObject in this.Categories.DeletedItems)
                centerRegistryObject.DeleteKey();
            this.Categories.ClearDeletedItems();
            foreach (EntryPoint entryPoint in (Collection<EntryPoint>)this.EntryPoints)
                entryPoint.Save();
            foreach (OemMenuStrip oemMenuStrip in this.managerDeletedStrips)
            {
                oemMenuStrip.DeleteKey();
                if (!string.IsNullOrEmpty(oemMenuStrip.Category))
                {
                    OemCategory oemCategory = this.Categories[oemMenuStrip.Category];
                    if (oemCategory != null)
                        oemCategory.DeleteKey();
                }
            }
            foreach (OemMenuStrip oemMenuStrip in (Collection<OemMenuStrip>)this.managerStrips)
            {
                using (RegistryKey subKey = oemMenuStrip.RegHive.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Start Menu\\Applications"))
                    this.RegistrySerialiser.Serialise((object)oemMenuStrip, subKey);
            }
            List<OemCategory> list = new List<OemCategory>();
            foreach (OemCategory oemCategory in (Collection<OemCategory>)this.Categories)
            {
                if (oemCategory.IsDirty || !oemCategory.IsSaved)
                {
                    if (oemCategory.QuickLinks.Count > 0)
                        oemCategory.Save();
                    else if (oemCategory.IsSaved)
                    {
                        list.Add(oemCategory);
                        oemCategory.DeleteKey();
                    }
                }
            }
            foreach (OemCategory oemCategory in list)
                this.Categories.Remove(oemCategory);
            this.Categories.ClearDeletedItems();
            this.IsDirty = false;
        }

        internal static RegistryKey GetRegistryKey(bool isAllUsers)
        {
            if (isAllUsers)
                return Registry.LocalMachine;
            else
                return Registry.CurrentUser;
        }

        protected virtual void BuildInfo(ICollection<OemMenuStrip> strips)
        {
            this.BuildApplicationInfo(true);
            this.BuildApplicationInfo(false);
            this.BuildEntryPointInfo(true);
            this.BuildEntryPointInfo(false);
            this.BuildCategoryInfo(true);
            this.BuildCategoryInfo(false);
            this.BuildStripInfo(true, strips);
            this.BuildStripInfo(false, strips);
        }

        private void AddCategories(RegistryKey key)
        {
            foreach (string name in key.GetSubKeyNames())
            {
                using (RegistryKey key1 = key.OpenSubKey(name))
                {
                    if (key1.SubKeyCount != 0)
                        this.AddCategories(key1);
                }
            }
            if (MediaCenterUtil.IsGuid(key.Name.Substring(key.Name.LastIndexOf('\\') + 1)))
                return;
            OemCategory oemCategory1 = (OemCategory)this.RegistrySerialiser.Deserialise(this.CategoryType, key);
            OemCategory oemCategory2 = this.managerCategories[oemCategory1.CategoryPath];
            if (oemCategory2 != null)
            {
                this.RegistrySerialiser.Deserialise((object)oemCategory2, key);
            }
            else
            {
                if (oemCategory1.QuickLinks.Count <= 0)
                    return;
                this.managerCategories.Add(oemCategory1);
            }
        }

        private void BuildApplicationInfo(bool isAllUsers)
        {
            using (RegistryKey registryKey = OemManager.GetRegistryKey(isAllUsers).OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Applications", false))
            {
                if (registryKey == null)
                    return;
                foreach (string name in registryKey.GetSubKeyNames())
                {
                    using (RegistryKey key = registryKey.OpenSubKey(name, false))
                    {
                        Application application1 = (Application)this.managerRs.Deserialise(this.ApplicationType, key);
                        Application application2 = this.Applications[application1.ID];
                        if (application2 != null)
                            this.managerRs.Deserialise((object)application2, key);
                        else
                            this.Applications.Add(application1);
                    }
                }
            }
        }

        private void BuildEntryPointInfo(bool isAllUsers)
        {
            using (RegistryKey key = OemManager.GetRegistryKey(isAllUsers).OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Entry Points", false))
            {
                if (key != null)
                    this.AddEntryPoints(key);
            }
            using (RegistryKey key = OemManager.GetRegistryKey(isAllUsers).OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories", false))
            {
                if (key == null)
                    return;
                this.AddEntryPoints(key);
            }
        }

        private void BuildCategoryInfo(bool isAllUsers)
        {
            using (RegistryKey registryKey = OemManager.GetRegistryKey(isAllUsers).OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories", false))
            {
                if (registryKey == null)
                    return;
                foreach (string name in registryKey.GetSubKeyNames())
                {
                    using (RegistryKey key = registryKey.OpenSubKey(name))
                        this.AddCategories(key);
                }
            }
        }

        private void BuildStripInfo(bool isAllUsers, ICollection<OemMenuStrip> strips)
        {
            if (strips == null)
                return;
            using (RegistryKey registryKey = OemManager.GetRegistryKey(isAllUsers).OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Start Menu\\Applications"))
            {
                if (registryKey == null)
                    return;
                foreach (string name in registryKey.GetSubKeyNames())
                {
                    using (RegistryKey key = registryKey.OpenSubKey(name))
                    {
                        OemMenuStrip oemMenuStrip = (OemMenuStrip)this.managerRs.Deserialise(this.MenuStripType, key);
                        OemMenuStrip oemStrip = this.GetOemStrip(oemMenuStrip.Application, (IEnumerable<OemMenuStrip>)strips);
                        if (oemStrip != null)
                            this.managerRs.Deserialise((object)oemStrip, key);
                        else if (oemMenuStrip.Application != null)
                        {
                            if (!string.IsNullOrEmpty(oemMenuStrip.Category))
                                strips.Add(oemMenuStrip);
                        }
                    }
                }
            }
        }

        private OemMenuStrip GetOemStrip(Application app, IEnumerable<OemMenuStrip> strips)
        {
            foreach (OemMenuStrip oemMenuStrip in strips)
            {
                if (oemMenuStrip.Application == app)
                    return oemMenuStrip;
            }
            return (OemMenuStrip)null;
        }

        private void Strips_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (OemMenuStrip oemMenuStrip in (IEnumerable)e.OldItems)
                    this.managerDeletedStrips.Add(oemMenuStrip);
            }
            if (e.NewItems == null)
                return;
            foreach (OemMenuStrip oemMenuStrip in (IEnumerable)e.NewItems)
                this.managerDeletedStrips.Remove(oemMenuStrip);
        }

        private void AddEntryPoints(RegistryKey key)
        {
            EntryPoint entryPoint1 = (EntryPoint)this.managerRs.Deserialise(this.EntryPointType, key);
            if (entryPoint1.Application != null)
            {
                EntryPoint entryPoint2 = this.EntryPoints[entryPoint1.ID];
                if (entryPoint2 != null)
                    this.managerRs.Deserialise((object)entryPoint2, key);
                else
                    this.EntryPoints.Add(entryPoint1);
            }
            foreach (string name in key.GetSubKeyNames())
            {
                using (RegistryKey key1 = key.OpenSubKey(name))
                    this.AddEntryPoints(key1);
            }
        }

        public abstract class OemObjectCollection<T> : ObservableKeyedCollection<string, T> where T : MediaCenterRegistryObject
        {
            private readonly OemManager manager;
            private readonly Dictionary<string, T> deletedItems;

            public IEnumerable<T> DeletedItems
            {
                get
                {
                    return (IEnumerable<T>)this.deletedItems.Values;
                }
            }

            public OemManager Manager
            {
                get
                {
                    return this.manager;
                }
            }

            public override T this[string key]
            {
                get
                {
                    if (key == null)
                        return default(T);
                    T obj;
                    this.Dictionary.TryGetValue(key.ToUpper(), out obj);
                    return obj;
                }
            }

            protected OemObjectCollection(OemManager manager)
            {
                if (manager == null)
                    throw new ArgumentNullException();
                this.manager = manager;
                this.deletedItems = new Dictionary<string, T>();
            }

            public void ClearDeletedItems()
            {
                foreach (T obj in this.deletedItems.Values)
                    obj.Manager.DirtyObjects.Remove((MediaCenterRegistryObject)obj);
                this.deletedItems.Clear();
            }

            protected abstract string GetID(T obj);

            protected override string GetKey(T obj)
            {
                return this.GetID(obj).ToUpper();
            }

            protected override void InsertItem(int index, T item)
            {
                base.InsertItem(index, item);
                item.Manager = this.manager;
                this.deletedItems.Remove(this.GetKey(item));
            }

            protected override void RemoveItem(int index)
            {
                T obj = base[index];
                base.RemoveItem(index);
                if (obj.IsSaved)
                    this.deletedItems[this.GetKey(obj)] = obj;
                this.manager.IsDirty = true;
            }

            protected virtual bool IsDirtyOnInsert(T item)
            {
                return true;
            }
        }

        public class CategoryCollection : OemManager.OemObjectCollection<OemCategory>
        {
            public override OemCategory this[string key]
            {
                get
                {
                    OemCategory oemCategory1 = base[key];
                    if (key != null && oemCategory1 == null)
                    {
                        OemCategory oemCategory2 = new OemCategory();
                        oemCategory2.Manager = this.Manager;
                        oemCategory2.CategoryPath = key;
                        oemCategory1 = oemCategory2;
                        string parentCategory = oemCategory1.ParentCategory;
                        if (!string.IsNullOrEmpty(parentCategory))
                        {
                            OemCategory oemCategory3 = this[parentCategory];
                        }
                        this.Add(oemCategory1);
                    }
                    return oemCategory1;
                }
            }

            public CategoryCollection(OemManager manager)
                : base(manager)
            {
            }

            protected override string GetID(OemCategory obj)
            {
                return obj.CategoryPath;
            }

            protected override bool IsDirtyOnInsert(OemCategory obj)
            {
                return obj.QuickLinks.Count > 0;
            }
        }

        public class EntryPointCollection : OemManager.OemObjectCollection<EntryPoint>
        {
            public EntryPointCollection(OemManager manager)
                : base(manager)
            {
            }

            protected override void InsertItem(int index, EntryPoint entryPoint)
            {
                if (entryPoint.Application == null)
                    throw new ArgumentException("The entry point must have an application.");
                if (entryPoint.ImageUrl == null && entryPoint.ImageOverride == null)
                    return;
                using (RegistryKey registryKey = entryPoint.RegHive.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories\\Background\\" + entryPoint.ID, false))
                {
                    if (registryKey != null)
                        return;
                    base.InsertItem(index, entryPoint);
                }
            }

            protected override string GetID(EntryPoint obj)
            {
                return obj.ID;
            }

            protected override void RemoveItem(int index)
            {
                EntryPoint entryPoint = base[index];
                base.RemoveItem(index);
                foreach (OemCategory oemCategory in (Collection<OemCategory>)this.Manager.Categories)
                {
                    for (int index1 = 0; index1 < oemCategory.QuickLinks.Count; ++index1)
                    {
                        OemQuickLink oemQuickLink = oemCategory.QuickLinks[index1] as OemQuickLink;
                        if (oemQuickLink != null && oemQuickLink.EntryPoint == entryPoint)
                        {
                            oemCategory.QuickLinks.RemoveAt(index1);
                            break;
                        }
                    }
                }
                Application application = entryPoint.Application;
                if (application == null)
                    return;
                application.EntryPoints.Remove(entryPoint);
                if (application.EntryPoints.Count != 0)
                    return;
                this.Manager.Applications.Remove(application);
            }
        }

        public class ApplicationCollection : OemManager.OemObjectCollection<Application>
        {
            public ApplicationCollection(OemManager manager)
                : base(manager)
            {
            }

            protected override void InsertItem(int index, Application app)
            {
                if (app.ID == null || app.ID.Trim() == string.Empty)
                    throw new ArgumentException("Application must have an ID.", "app");
                base.InsertItem(index, app);
            }

            protected override string GetID(Application app)
            {
                return app.ID.ToUpper();
            }

            protected override void RemoveItem(int index)
            {
                Application application = base[index];
                base.RemoveItem(index);
                foreach (EntryPoint entryPoint in (Collection<EntryPoint>)application.EntryPoints)
                    this.Manager.EntryPoints.Remove(entryPoint);
            }
        }
    }
}

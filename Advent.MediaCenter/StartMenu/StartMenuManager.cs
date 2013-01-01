using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu.Default;
using Advent.MediaCenter.StartMenu.Fiji;
using Advent.MediaCenter.StartMenu.OEM;
using Advent.MediaCenter.StartMenu.Windows7;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    public abstract class StartMenuManager : INotifyPropertyChanged
    {
        private string customName = "Custom";
        private readonly OemManager oemManager;
        private readonly ObservableCollection<IMenuStrip> managerStrips;
        private XmlDocument startMenuXml;
        private bool isDirty;
        private PropertyChangedEventHandler propertyChanged;
        private EventHandler saving;
        private EventHandler saved;

        public IResourceLibraryCache Resources { get; private set; }

        public string CustomCategory
        {
            get
            {
                return this.customName;
            }
            set
            {
                this.customName = value;
            }
        }

        public bool IsDirty
        {
            get
            {
                if (this.isDirty)
                    return true;
                if (this.oemManager != null)
                    return this.oemManager.IsDirty;
                else
                    return false;
            }
            internal set
            {
                if (this.isDirty == value)
                    return;
                this.isDirty = value;
                this.OnIsDirtyChanged();
            }
        }

        public ObservableCollection<IMenuStrip> Strips
        {
            get
            {
                return this.managerStrips;
            }
        }

        public int CustomStripCount
        {
            get
            {
                int num = 0;
                foreach (IMenuStrip menuStrip in (Collection<IMenuStrip>)this.managerStrips)
                {
                    if (menuStrip is OemMenuStrip)
                        ++num;
                }
                return num;
            }
        }

        public abstract int MaxCustomStripCount { get; }

        public abstract int MinCustomStripCount { get; }

        public OemManager OemManager
        {
            get
            {
                return this.oemManager;
            }
        }

        internal string CustomInternalCategory
        {
            get
            {
                return string.Format("{0}\\Internal", (object)this.CustomCategory);
            }
        }

        protected abstract XmlNode StripParentNode { get; }

        protected XmlDocument StartMenuDocument
        {
            get
            {
                return this.startMenuXml;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                PropertyChangedEventHandler changedEventHandler = this.propertyChanged;
                PropertyChangedEventHandler comparand;
                do
                {
                    comparand = changedEventHandler;
                    changedEventHandler = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.propertyChanged, comparand + value, comparand);
                }
                while (changedEventHandler != comparand);
            }
            remove
            {
                PropertyChangedEventHandler changedEventHandler = this.propertyChanged;
                PropertyChangedEventHandler comparand;
                do
                {
                    comparand = changedEventHandler;
                    changedEventHandler = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.propertyChanged, comparand - value, comparand);
                }
                while (changedEventHandler != comparand);
            }
        }

        public event EventHandler Saving
        {
            add
            {
                EventHandler eventHandler = this.saving;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.saving, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.saving;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.saving, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler Saved
        {
            add
            {
                EventHandler eventHandler = this.saved;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.saved, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.saved;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.saved, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        protected StartMenuManager(IResourceLibraryCache resourceLibrary)
            : this(resourceLibrary, new OemManager())
        {
        }

        protected StartMenuManager(IResourceLibraryCache resourceLibrary, OemManager oemManager)
        {
            this.Resources = resourceLibrary;
            this.oemManager = oemManager;
            if (this.oemManager != null)
                this.oemManager.PropertyChanged += new PropertyChangedEventHandler(this.OemManager_PropertyChanged);
            this.managerStrips = (ObservableCollection<IMenuStrip>)new StartMenuManager.StripCollection(this);
            this.Reset(false);
        }

        public static StartMenuManager Create(IResourceLibraryCache cache)
        {
            return StartMenuManager.Create(cache, (OemManager)null);
        }

        public static StartMenuManager Create(IResourceLibraryCache resourceLibrary, OemManager oemManager)
        {
            if (oemManager == null)
                oemManager = new OemManager();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(MediaCenterUtil.MediaCenterPath, "ehres.dll"));
            if (versionInfo.FileMajorPart != 6)
                throw new InvalidOperationException(string.Format("Version {0} of Media Center is not supported.", (object)versionInfo.ProductVersion));
            if (versionInfo.ProductMinorPart == 0)
                return (StartMenuManager)new DefaultStartMenuManager(resourceLibrary, oemManager);
            if (versionInfo.ProductMinorPart == 1 && versionInfo.ProductBuildPart < 7100)
                return (StartMenuManager)new FijiStartMenuManager(resourceLibrary, oemManager);
            else
                return (StartMenuManager)new Windows7StartMenuManager(resourceLibrary, oemManager);
        }

        public void Reset()
        {
            this.Resources.Clear();
            this.Reset(true);
        }

        public void DeleteInternalKeys()
        {
            try
            {
                Registry.LocalMachine.DeleteSubKeyTree("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories\\" + this.CustomInternalCategory);
            }
            catch (ArgumentException )
            {
            }
            foreach (string str in Registry.Users.GetSubKeyNames())
            {
                try
                {
                    string subkey = string.Format("{0}\\{1}\\{2}", (object)str, (object)"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Categories", (object)this.CustomInternalCategory);
                    Registry.Users.DeleteSubKeyTree(subkey);
                }
                catch (ArgumentException)
                {
                }
            }
        }

        public IMenuStrip AddCustomStrip()
        {
            IMenuStrip customStrip = this.CreateCustomStrip();
            ObservableCollection<IMenuStrip> strips = this.Strips;
            int index1 = -1;
            for (int index2 = strips.Count - 1; index2 >= 0; --index2)
            {
                if (strips[index2].CanSetPriority)
                {
                    index1 = index2 + 1;
                    break;
                }
            }
            if (index1 >= 0)
                strips.Insert(index1, customStrip);
            else
                strips.Add(customStrip);
            return customStrip;
        }

        public void Save(IResourceLibraryCache cache)
        {
            this.Save(cache, false);
        }

        public virtual void Save(IResourceLibraryCache cache, bool forceSave)
        {
            try
            {
                if (!this.IsDirty && !forceSave)
                    return;
                this.OnSaving(EventArgs.Empty);
                IResourceLibrary resourceLibrary = cache["ehres.dll"];
                this.SaveInternal(resourceLibrary);
                List<XmlNode> list = new List<XmlNode>();
                foreach (XmlElement element in this.StripParentNode.ChildNodes)
                {
                    int index;
                    if (this.IsOemPlaceholderElement(element, out index))
                        list.Add((XmlNode)element);
                }
                foreach (XmlNode oldChild in list)
                    this.StripParentNode.RemoveChild(oldChild);
                int index1 = 0;
                foreach (IMenuStrip menuStrip in (Collection<IMenuStrip>)this.managerStrips)
                {
                    BaseXmlMenuStrip strip1 = menuStrip as BaseXmlMenuStrip;
                    if (strip1 != null)
                    {
                        if (strip1.StartMenuElement.ParentNode != null)
                            strip1.StartMenuElement.ParentNode.RemoveChild((XmlNode)strip1.StartMenuElement);
                        this.StripParentNode.AppendChild(this.GetNodeForSave(strip1));
                        strip1.Save(resourceLibrary);
                    }
                    else
                    {
                        OemMenuStrip strip2 = menuStrip as OemMenuStrip;
                        if (strip2 != null)
                        {
                            strip2.TimeStamp = int.MaxValue - index1;
                            this.StripParentNode.AppendChild(this.CreateOemStripNode(strip2, index1));
                            ++index1;
                        }
                    }
                }
                int customStripCount;
                for (customStripCount = this.MinCustomStripCount; index1 < customStripCount; ++index1)
                    this.StripParentNode.AppendChild(this.CreateOemStripNode((OemMenuStrip)null, index1));
                this.UpdateOemStripCount(customStripCount);
                if (this.oemManager != null)
                    this.oemManager.Save();
                MediaCenterUtil.SaveXmlResource(resourceLibrary, "STARTMENU.XML", 23, this.StartMenuDocument);
                this.IsDirty = false;
            }
            finally
            {
                this.OnSaved(EventArgs.Empty);
            }
        }

        protected virtual void UpdateOemStripCount(int minCustomStrips)
        {
            (this.StripParentNode.ParentNode as XmlElement).SetAttribute("MaxOEMStrips", minCustomStrips.ToString());
        }

        protected virtual void SaveInternal(IResourceLibrary ehres)
        {
        }

        protected abstract IMenuStrip CreateMenuStrip(XmlNode element, IResourceLibrary ehres);

        protected abstract IMenuStrip CreateCustomStrip();

        protected virtual XmlNode GetNodeForSave(BaseXmlMenuStrip strip)
        {
            return (XmlNode)strip.StartMenuElement;
        }

        protected virtual void OnIsDirtyChanged()
        {
            if (this.propertyChanged == null)
                return;
            this.propertyChanged((object)this, new PropertyChangedEventArgs("IsDirty"));
        }

        protected virtual void OnSaving(EventArgs args)
        {
            EventHandler eventHandler = this.saving;
            if (eventHandler == null)
                return;
            eventHandler((object)this, args);
        }

        protected virtual void OnSaved(EventArgs args)
        {
            EventHandler eventHandler = this.saved;
            if (eventHandler == null)
                return;
            eventHandler((object)this, args);
        }

        protected abstract XmlNode CreateOemStripNode(OemMenuStrip strip, int index);

        private void Reset(bool resetOem)
        {
            try
            {
                this.managerStrips.Clear();
                if (resetOem && this.oemManager != null)
                    this.oemManager.Reset();
                IResourceLibrary resourceLibrary = this.Resources["ehres.dll"];
                XmlReader xmlResource = MediaCenterUtil.GetXmlResource(resourceLibrary, "STARTMENU.XML", 23);
                if (xmlResource != null)
                {
                    this.startMenuXml = new XmlDocument();
                    this.startMenuXml.Load(xmlResource);
                    List<IMenuStrip> list = new List<IMenuStrip>();
                    XmlNodeList childNodes = this.StripParentNode.ChildNodes;
                    for (int index1 = 0; index1 < childNodes.Count; ++index1)
                    {
                        IMenuStrip menuStrip = (IMenuStrip)null;
                        XmlElement element = childNodes[index1] as XmlElement;
                        int index2;
                        if (element != null && this.IsOemPlaceholderElement(element, out index2) && index2 >= 0)
                        {
                            if (index2 < this.MaxCustomStripCount)
                            {
                                if (this.oemManager != null && index2 < this.oemManager.StartMenuStrips.Count)
                                    menuStrip = (IMenuStrip)this.oemManager.StartMenuStrips[index2];
                                else
                                    continue;
                            }
                            else
                                Trace.WriteLine(string.Format("OEM strip element has index of {0}, which is over the max strip count of {1}.", (object)index2, (object)this.MaxCustomStripCount));
                        }
                        if (menuStrip == null)
                            menuStrip = this.CreateMenuStrip(childNodes[index1], resourceLibrary);
                        if (menuStrip != null)
                        {
                            if (element == null)
                            {
                                XmlMenuStrip xmlMenuStrip = menuStrip as XmlMenuStrip;
                                if (xmlMenuStrip != null)
                                {
                                    XmlElement startMenuElement = xmlMenuStrip.StartMenuElement;
                                }
                            }
                            list.Add(menuStrip);
                        }
                    }
                    foreach (IMenuStrip menuStrip in list)
                        this.managerStrips.Add(menuStrip);
                    this.managerStrips.Add((IMenuStrip)new OemCategoryStrip(this, "Auto Play (Blu-ray)", new string[1]
          {
            "AutoPlay\\Blu-ray"
          }));
                    this.managerStrips.Add((IMenuStrip)new OemCategoryStrip(this, "Auto Play (HD DVD)", new string[1]
          {
            "AutoPlay\\HD DVD"
          }));
                }
                else
                    Trace.TraceWarning("Could not find STARTMENU.XML in ehres.dll!");
                this.IsDirty = false;
            }
            finally
            {
                this.Resources.Clear();
            }
        }

        protected abstract bool IsOemPlaceholderElement(XmlElement element, out int index);

        private void OemManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName == "IsDirty"))
                return;
            this.OnIsDirtyChanged();
        }

        private class StripCollection : ObservableCollection<IMenuStrip>
        {
            private readonly StartMenuManager manager;

            public StripCollection(StartMenuManager smm)
            {
                this.manager = smm;
            }

            protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                this.manager.IsDirty = true;
                List<IMenuStrip> list = new List<IMenuStrip>();
                if (e.OldItems != null)
                {
                    foreach (IMenuStrip menuStrip in (IEnumerable)e.OldItems)
                    {
                        OemMenuStrip oemMenuStrip1 = menuStrip as OemMenuStrip;
                        if (oemMenuStrip1 != null)
                        {
                            this.manager.OemManager.StartMenuStrips.Remove(oemMenuStrip1);
                            for (int index = 0; index < this.manager.OemManager.StartMenuStrips.Count; ++index)
                            {
                                OemMenuStrip oemMenuStrip2 = this.manager.OemManager.StartMenuStrips[index];
                                if (oemMenuStrip2 != null && oemMenuStrip2.IsEnabled && (!this.Contains((IMenuStrip)oemMenuStrip2) && !e.OldItems.Contains((object)oemMenuStrip2)))
                                {
                                    oemMenuStrip2.Priority = oemMenuStrip1.Priority;
                                    list.Add((IMenuStrip)oemMenuStrip2);
                                    break;
                                }
                            }
                        }
                        if (e.NewItems == null || !e.NewItems.Contains((object)menuStrip))
                            menuStrip.QuickLinks.Clear();
                    }
                }
                if (e.NewItems != null)
                {
                    foreach (IMenuStrip menuStrip in (IEnumerable)e.NewItems)
                    {
                        OemMenuStrip oemMenuStrip = menuStrip as OemMenuStrip;
                        if (oemMenuStrip != null && !this.manager.OemManager.StartMenuStrips.Contains(oemMenuStrip))
                            this.manager.OemManager.StartMenuStrips.Add(oemMenuStrip);
                    }
                }
                base.OnCollectionChanged(e);
                foreach (IMenuStrip menuStrip in list)
                    this.Insert(e.OldStartingIndex, menuStrip);
            }
        }
    }
}

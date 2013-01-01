


using Advent.Common.IO;
using Advent.MediaCenter;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    internal class NowPlayingStrip : BaseXmlMenuStrip, IMenuStrip, ISupportInitialize
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NowPlayingStrip));
        //this was originally readonly
        private NowPlayingStrip.NowPlayingLinkCollection links;
        private readonly int titleResourceID;
        private readonly string originalTitle;

        public ObservableCollection<IQuickLink> QuickLinks
        {
            get
            {
                return (ObservableCollection<IQuickLink>)this.links;
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(NowPlayingStrip.TitleProperty);
            }
            set
            {
                this.SetValue(NowPlayingStrip.TitleProperty, (object)value);
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

        public bool IsEnabled
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotSupportedException();
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

        static NowPlayingStrip()
        {
        }

        internal NowPlayingStrip(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
            this.BeginInit();
            this.originalTitle = MediaCenterUtil.GetMagicString(this.Manager.Resources, "#SM.NowPlaying.Title", out this.titleResourceID);
            this.Title = this.originalTitle;
            NowPlayingStrip nowPlayingStrip = this;
            NowPlayingStrip.NowPlayingLinkCollection playingLinkCollection1 = new NowPlayingStrip.NowPlayingLinkCollection();
            playingLinkCollection1.Add((IQuickLink)new NowPlayingQuickLink(smm));
            NowPlayingStrip.NowPlayingLinkCollection playingLinkCollection2 = playingLinkCollection1;
            nowPlayingStrip.links = playingLinkCollection2;
            this.EndInit();
        }

        public bool CanSwapWith(IMenuStrip strip)
        {
            return true;
        }

        public bool CanAddQuickLink(IQuickLink link)
        {
            return false;
        }

        protected override void OnPriorityChanged(int oldValue, int newValue)
        {
            this.StartMenuTargetElement.SetAttribute("Priority", newValue.ToString());
        }

        internal override void Save(IResourceLibrary ehres)
        {
            base.Save(ehres);
        }

        private class NowPlayingLinkCollection : ObservableCollection<IQuickLink>
        {
            protected override void InsertItem(int index, IQuickLink item)
            {
                if (!(item is NowPlayingQuickLink) || this.Count != 0)
                    throw new ArgumentException();
                base.InsertItem(index, item);
            }
        }
    }
}

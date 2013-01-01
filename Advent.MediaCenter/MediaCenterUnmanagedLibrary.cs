


using Advent.Common.Interop;
using Advent.Common.IO;
using System;
using System.Threading;

namespace Advent.MediaCenter
{
    public class MediaCenterUnmanagedLibrary : UnmanagedLibrary
    {
        private string muiFile;
        private UnmanagedLibraryAccess mode;
        private static EventHandler<MediaCenterLibraryUpdatedEventArgs> mediaCenterLibraryUpdated;

        public static event EventHandler<MediaCenterLibraryUpdatedEventArgs> MediaCenterLibraryUpdated
        {
            add
            {
                EventHandler<MediaCenterLibraryUpdatedEventArgs> eventHandler1 = MediaCenterUnmanagedLibrary.mediaCenterLibraryUpdated;
                EventHandler<MediaCenterLibraryUpdatedEventArgs> comparand;
                do
                {
                    comparand = eventHandler1;
                    EventHandler<MediaCenterLibraryUpdatedEventArgs> eventHandler2 = comparand + value;
                    eventHandler1 = Interlocked.CompareExchange<EventHandler<MediaCenterLibraryUpdatedEventArgs>>(ref MediaCenterUnmanagedLibrary.mediaCenterLibraryUpdated, eventHandler2, comparand);
                }
                while (eventHandler1 != comparand);
            }
            remove
            {
                EventHandler<MediaCenterLibraryUpdatedEventArgs> eventHandler1 = MediaCenterUnmanagedLibrary.mediaCenterLibraryUpdated;
                EventHandler<MediaCenterLibraryUpdatedEventArgs> comparand;
                do
                {
                    comparand = eventHandler1;
                    EventHandler<MediaCenterLibraryUpdatedEventArgs> eventHandler2 = comparand - value;
                    eventHandler1 = Interlocked.CompareExchange<EventHandler<MediaCenterLibraryUpdatedEventArgs>>(ref MediaCenterUnmanagedLibrary.mediaCenterLibraryUpdated, eventHandler2, comparand);
                }
                while (eventHandler1 != comparand);
            }
        }

        public MediaCenterUnmanagedLibrary(string file)
            : this(file, UnmanagedLibraryAccess.Read)
        {
        }

        public MediaCenterUnmanagedLibrary(string file, UnmanagedLibraryAccess mode)
            : base(file, mode)
        {
            this.mode = mode;
            if (mode != UnmanagedLibraryAccess.Write)
                return;
            this.muiFile = this.GetMUIFile();
            if (this.muiFile == null)
                return;
            this.GetMUI().Update((byte[])null, new ushort?((ushort)1033));
        }

        protected void OnMediaCenterLibraryUpdated(MediaCenterLibraryUpdatedEventArgs args)
        {
            EventHandler<MediaCenterLibraryUpdatedEventArgs> eventHandler = MediaCenterUnmanagedLibrary.mediaCenterLibraryUpdated;
            if (eventHandler == null)
                return;
            eventHandler((object)this, args);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing && this.muiFile != null)
            {
                using (UnmanagedLibrary unmanagedLibrary = new UnmanagedLibrary(this.muiFile))
                {
                    foreach (object index in unmanagedLibrary.ResourceTypes)
                    {
                        string str = index as string;
                        if ((str == null || !(str == "MUI")) && (str != null || (int)index != 16))
                        {
                            foreach (Resource resource1 in unmanagedLibrary[index])
                            {
                                IResource resource2 = resource1.ID.HasValue ? this.GetResource(resource1.ID.Value, index) : this.GetResource(resource1.Name, index);
                                foreach (ushort num in resource1.Languages)
                                    resource2.Update(resource1.GetBytes(new ushort?(num)), new ushort?(num));
                            }
                        }
                    }
                }
            }
            base.Dispose(isDisposing);
            if (this.mode != UnmanagedLibraryAccess.Write)
                return;
            this.OnMediaCenterLibraryUpdated(new MediaCenterLibraryUpdatedEventArgs(this.File));
        }
    }
}

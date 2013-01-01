


using Advent.Common.Interop;
using Advent.Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Advent.MediaCenter
{
    public class MediaCenterLibraryCache : IResourceLibraryCache, IDisposable
    {
        private readonly Dictionary<string, IResourceLibrary> libraries = new Dictionary<string, IResourceLibrary>();
        private readonly UnmanagedLibraryAccess mode;
        private EventHandler<LibraryLoadEventArgs> libraryLoading;
        private EventHandler<LibraryLoadEventArgs> libraryLoaded;

        public string SearchPath { get; private set; }

        public IEnumerable<string> LoadedFiles
        {
            get
            {
                return (IEnumerable<string>)this.libraries.Keys;
            }
        }

        public IResourceLibrary this[string dllName]
        {
            get
            {
                return this.LoadLibrary(dllName);
            }
        }

        public event EventHandler<LibraryLoadEventArgs> LibraryLoading
        {
            add
            {
                EventHandler<LibraryLoadEventArgs> eventHandler = this.libraryLoading;
                EventHandler<LibraryLoadEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<LibraryLoadEventArgs>>(ref this.libraryLoading, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<LibraryLoadEventArgs> eventHandler = this.libraryLoading;
                EventHandler<LibraryLoadEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<LibraryLoadEventArgs>>(ref this.libraryLoading, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler<LibraryLoadEventArgs> LibraryLoaded
        {
            add
            {
                EventHandler<LibraryLoadEventArgs> eventHandler = this.libraryLoaded;
                EventHandler<LibraryLoadEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<LibraryLoadEventArgs>>(ref this.libraryLoaded, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<LibraryLoadEventArgs> eventHandler = this.libraryLoaded;
                EventHandler<LibraryLoadEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<LibraryLoadEventArgs>>(ref this.libraryLoaded, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public MediaCenterLibraryCache()
            : this(MediaCenterUtil.MediaCenterPath)
        {
        }

        public MediaCenterLibraryCache(string baseSearchPath)
            : this(baseSearchPath, UnmanagedLibraryAccess.Read)
        {
        }

        public MediaCenterLibraryCache(string baseSearchPath, UnmanagedLibraryAccess mode)
        {
            if (baseSearchPath == null)
                throw new ArgumentNullException();
            this.mode = mode;
            this.SearchPath = baseSearchPath;
        }

        public void Clear()
        {
            foreach (IDisposable disposable in this.libraries.Values)
                disposable.Dispose();
            this.libraries.Clear();
        }

        public IResourceLibrary LoadLibrary(string dllName)
        {
            string file = Path.Combine(this.SearchPath, dllName);
            IResourceLibrary resourceLibrary;
            if (!this.libraries.TryGetValue(dllName.ToUpperInvariant(), out resourceLibrary))
            {
                LibraryLoadEventArgs e = new LibraryLoadEventArgs(dllName);
                e.Mode = this.mode;
                this.OnLibraryLoading(e);
                resourceLibrary = (IResourceLibrary)new MediaCenterUnmanagedLibrary(file, e.Mode);
                this.libraries[dllName.ToUpperInvariant()] = resourceLibrary;
                this.OnLibraryLoaded(e);
            }
            return resourceLibrary;
        }

        public void Dispose()
        {
            this.Clear();
        }

        protected virtual void OnLibraryLoading(LibraryLoadEventArgs e)
        {
            if (this.libraryLoading == null)
                return;
            this.libraryLoading((object)this, e);
        }

        protected virtual void OnLibraryLoaded(LibraryLoadEventArgs e)
        {
            if (this.libraryLoaded == null)
                return;
            this.libraryLoaded((object)this, e);
        }
    }
}

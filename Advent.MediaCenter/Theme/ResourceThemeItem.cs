


using Advent.MediaCenter;
using System;

namespace Advent.MediaCenter.Theme
{
    [MediaCenterVersion(typeof(ResourceThemeItemApplicator))]
    public abstract class ResourceThemeItem : ThemeItemBase
    {
        private readonly Func<ResourceThemeItem, byte[]> fetchOriginalBuffer;
        private byte[] originalBuffer;

        public string DllName { get; private set; }

        public override string Name
        {
            get
            {
                return this.ResourceName;
            }
        }

        public string ResourceName { get; private set; }

        public abstract int ResourceType { get; }

        public byte[] OriginalBuffer
        {
            get
            {
                if (this.originalBuffer == null && this.fetchOriginalBuffer != null)
                    this.originalBuffer = this.fetchOriginalBuffer(this);
                return this.originalBuffer;
            }
            private set
            {
                this.originalBuffer = value;
            }
        }

        protected ResourceThemeItem(string dllName, string resourceName, byte[] buffer)
            : this(dllName, resourceName)
        {
            this.OriginalBuffer = buffer;
        }

        protected ResourceThemeItem(string dllName, string resourceName, Func<ResourceThemeItem, byte[]> fetchBuffer)
            : this(dllName, resourceName)
        {
            this.fetchOriginalBuffer = fetchBuffer;
        }

        private ResourceThemeItem(string dllName, string resourceName)
        {
            this.DllName = dllName;
            this.ResourceName = resourceName;
        }

        public byte[] Save(bool saveBuffer)
        {
            byte[] numArray = this.Save();
            if (saveBuffer)
                this.OriginalBuffer = numArray;
            return numArray;
        }

        protected abstract byte[] Save();

        public override void Apply(MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            this.Load();
            base.Apply(readCache, writeCache);
        }
    }
}

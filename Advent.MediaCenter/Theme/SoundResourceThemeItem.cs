


using System;
using System.IO;
using System.Media;

namespace Advent.MediaCenter.Theme
{
    public class SoundResourceThemeItem : ResourceThemeItem
    {
        private SoundPlayer sound = new SoundPlayer();

        public SoundPlayer Sound
        {
            get
            {
                return this.sound;
            }
        }

        public override int ResourceType
        {
            get
            {
                return 10;
            }
        }

        public SoundResourceThemeItem(string dllName, string resourceName, byte[] buffer)
            : base(dllName, resourceName, buffer)
        {
        }

        public SoundResourceThemeItem(string dllName, string resourceName, Func<ResourceThemeItem, byte[]> buffer)
            : base(dllName, resourceName, buffer)
        {
        }

        protected override byte[] Save()
        {
            MemoryStream memoryStream = new MemoryStream();
            if (this.sound.Stream != null)
            {
                this.sound.Stream.Seek(0L, SeekOrigin.Begin);
                byte[] buffer = new byte[this.sound.Stream.Length];
                this.sound.Stream.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, buffer.Length);
            }
            else if (!string.IsNullOrEmpty(this.sound.SoundLocation))
            {
                byte[] buffer = File.ReadAllBytes(this.sound.SoundLocation);
                memoryStream.Write(buffer, 0, buffer.Length);
            }
            return memoryStream.GetBuffer();
        }

        protected override void LoadInternal()
        {
            byte[] originalBuffer = this.OriginalBuffer;
            if (originalBuffer == null)
                return;
            this.sound.Stream = (Stream)new MemoryStream(originalBuffer);
            this.sound.Load();
        }
    }
}

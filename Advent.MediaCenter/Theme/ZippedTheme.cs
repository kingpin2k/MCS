


using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using System.IO;

namespace Advent.MediaCenter.Theme
{
    public abstract class ZippedTheme : MediaCenterTheme
    {
        private FileMode mode;

        protected ZipFile ZipFile { get; private set; }

        protected ZippedTheme(string file, FileMode mode)
        {
            this.File = file;
            this.mode = mode;
            if ((mode == FileMode.Create || mode == FileMode.CreateNew || mode == FileMode.OpenOrCreate) && !System.IO.File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                this.ZipFile = ZipFile.Create(file);
                this.mode = FileMode.Open;
            }
            else
                this.ResetZipFile();
        }

        protected void ResetZipFile()
        {
            if (this.ZipFile != null)
                this.ZipFile.Close();
            this.ZipFile = new ZipFile(System.IO.File.Open(this.File, this.mode, FileAccess.ReadWrite, FileShare.ReadWrite));
        }

        protected static byte[] ReadZipEntry(ZipFile zip, ZipEntry entry)
        {
            if (entry == null)
            {
                Trace.TraceWarning("Null resource zip entry passed to ReadZipEntry.");
                return (byte[])null;
            }
            else
            {
                long num = 0L;
                long size = entry.Size;
                byte[] buffer = new byte[size];
                Stream inputStream = zip.GetInputStream(entry);
                while ((size -= (long)inputStream.Read(buffer, (int)num, (int)size)) > 0L)
                    num = entry.Size - size;
                return buffer;
            }
        }

        protected static IStaticDataSource GetResourceThemeItemData(ResourceThemeItem item)
        {
            return (IStaticDataSource)new ZippedTheme.ThemeItemDataSource(item);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            this.ZipFile.Close();
        }

        public class StreamDataSource : IStaticDataSource
        {
            public Stream Stream { get; set; }

            public StreamDataSource(byte[] bytes)
            {
                this.Stream = (Stream)new MemoryStream(bytes);
            }

            public StreamDataSource(Stream stream)
            {
                this.Stream = stream;
            }

            public Stream GetSource()
            {
                if (this.Stream.CanSeek)
                    this.Stream.Seek(0L, SeekOrigin.Begin);
                return this.Stream;
            }
        }

        private class ThemeItemDataSource : IStaticDataSource
        {
            public ResourceThemeItem ThemeItem { get; private set; }

            public ThemeItemDataSource(ResourceThemeItem item)
            {
                this.ThemeItem = item;
            }

            public Stream GetSource()
            {
                return (Stream)new MemoryStream(this.ThemeItem.Save(true));
            }
        }
    }
}

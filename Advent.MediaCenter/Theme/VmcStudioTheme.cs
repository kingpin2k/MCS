


using Advent.Common.UI;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    public class VmcStudioTheme : ZippedTheme
    {
        private readonly Dictionary<IThemeItem, ZipEntry> themeItemEntries = new Dictionary<IThemeItem, ZipEntry>();
        private readonly List<ZipEntry> fontEntries = new List<ZipEntry>();
        public const string FileExtension = ".mct";
        private const string ImageResource = "Images";
        private const string SoundResource = "Sounds";
        private readonly VmcStudioTheme.ThemeInfo themeInfo;

        public override bool CanSave
        {
            get
            {
                return true;
            }
        }

        public VmcStudioTheme(string file, string id)
            : base(file, FileMode.CreateNew)
        {
            this.ID = id;
            this.themeInfo = new VmcStudioTheme.ThemeInfo();
            this.FontsItem = new FontsThemeItem();
            this.ColorsItem = new ColorsThemeItem();
        }

        public VmcStudioTheme(string file, MediaCenterTheme theme)
            : base(file, FileMode.CreateNew)
        {
            this.themeInfo = new VmcStudioTheme.ThemeInfo();
            foreach (IThemeItem themeItem in (Collection<IThemeItem>)theme.ThemeItems)
            {
                if (themeItem is FontsThemeItem)
                    this.themeInfo.Fonts = (FontsThemeItem)themeItem;
                if (themeItem is ColorsThemeItem)
                    this.themeInfo.Colors = (ColorsThemeItem)themeItem;
                this.ThemeItems.Add(themeItem);
            }
            this.Name = theme.Name;
            this.Author = theme.Author;
            this.Comments = theme.Comments;
            this.Version = theme.Version;
            this.ThemeType = theme.ThemeType;
            this.MainScreenshot = theme.MainScreenshot;
            this.FontFiles = theme.FontFiles;
            this.ID = Guid.NewGuid().ToString();
            this.themeInfo.Fonts.ClearDirty();
        }

        public VmcStudioTheme(string file)
            : base(file, FileMode.Open)
        {
            ZipEntry entry1 = this.ZipFile.GetEntry("Theme.xml");
            if (entry1 == null)
            {
                Trace.TraceError("Could not find Theme.xml in theme " + file);
                throw new Exception("Could not find theme entry in theme file.");
            }
            else
            {
                this.themeInfo = (VmcStudioTheme.ThemeInfo)new XmlSerializer(typeof(VmcStudioTheme.ThemeInfo)).Deserialize(this.ZipFile.GetInputStream(entry1));
                this.themeInfo.BeginInit();
                if (this.themeInfo.Fonts == null)
                    this.themeInfo.Fonts = new FontsThemeItem();
                this.themeInfo.Fonts.ClearDirty();
                this.FontsItem = this.themeInfo.Fonts;
                if (this.themeInfo.Colors == null)
                    this.themeInfo.Colors = new ColorsThemeItem();
                this.themeInfo.Colors.ClearDirty();
                this.ColorsItem = this.themeInfo.Colors;
                this.Name = this.themeInfo.Name;
                this.Author = this.themeInfo.Author;
                this.Version = new Version(this.themeInfo.Version);
                this.Comments = this.themeInfo.Comments;
                this.ThemeType = this.themeInfo.ThemeType;
                this.ID = this.themeInfo.ID;
                foreach (ZipEntry entry2 in this.ZipFile)
                {
                    IThemeItem index = (IThemeItem)null;
                    string[] strArray = entry2.Name.Split(new char[2]
          {
            '\\',
            '/'
          });
                    if (strArray[0] == "Resources")
                    {
                        string dllName = strArray[1] + ".dll";
                        string str = strArray[2];
                        string resourceName = strArray[3];
                        switch (str)
                        {
                            case "Images":
                                index = (IThemeItem)new ImageResourceThemeItem(dllName, resourceName, (Func<ResourceThemeItem, byte[]>)(themeItem => ZippedTheme.ReadZipEntry(this.ZipFile, this.themeItemEntries[(IThemeItem)themeItem])));
                                break;
                            case "Sounds":
                                index = (IThemeItem)new SoundResourceThemeItem(dllName, resourceName, (Func<ResourceThemeItem, byte[]>)(themeItem => ZippedTheme.ReadZipEntry(this.ZipFile, this.themeItemEntries[(IThemeItem)themeItem])));
                                break;
                        }
                    }
                    else if (strArray[0] == "Fonts")
                        this.fontEntries.Add(entry2);
                    else if (entry2.Name == "Screenshot.png")
                        this.MainScreenshot = (BitmapSource)BitmapDecoder.Create(this.ZipFile.GetInputStream(entry2), BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0];
                    else if (strArray[0] == "Screenshots")
                        this.Screenshots.Add((BitmapSource)BitmapDecoder.Create(this.ZipFile.GetInputStream(entry2), BitmapCreateOptions.None, BitmapCacheOption.None).Frames[0]);
                    if (index != null)
                    {
                        this.themeItemEntries[index] = entry2;
                        this.ThemeItems.Add(index);
                    }
                }
                this.themeInfo.EndInit();
            }
        }

        protected override void SaveInternal()
        {
            this.themeInfo.ID = this.ID;
            this.themeInfo.Name = this.Name;
            this.themeInfo.Author = this.Author;
            this.themeInfo.Version = ((object)(this.Version ?? new Version(0, 0))).ToString();
            this.themeInfo.Comments = this.Comments;
            this.themeInfo.ThemeType = this.ThemeType;
            foreach (IThemeItem themeItem in (Collection<IThemeItem>)this.ThemeItems)
            {
                if (!themeItem.IsLoaded)
                    themeItem.Load();
            }
            this.ZipFile.BeginUpdate();
            foreach (ZipEntry entry in new List<ZipEntry>(Enumerable.OfType<ZipEntry>((IEnumerable)this.ZipFile)))
                this.ZipFile.Delete(entry);
            this.themeItemEntries.Clear();
            this.themeInfo.Fonts = this.FontsItem;
            this.themeInfo.Colors = this.ColorsItem;
            MemoryStream memoryStream = new MemoryStream();
            new XmlSerializer(typeof(VmcStudioTheme.ThemeInfo)).Serialize((Stream)memoryStream, (object)this.themeInfo);
            this.ZipFile.Add((IStaticDataSource)new ZippedTheme.StreamDataSource((Stream)memoryStream), "Theme.xml");
            if (this.MainScreenshot != null)
                VmcStudioTheme.AddImageResource(this.MainScreenshot, this.ZipFile, "Screenshot.png");
            for (int index = 0; index < this.Screenshots.Count; ++index)
                VmcStudioTheme.AddImageResource(this.Screenshots[index], this.ZipFile, string.Format("Screenshots\\{0}.png", (object)index));
            foreach (ResourceThemeItem resourceThemeItem in Enumerable.OfType<ResourceThemeItem>((IEnumerable)this.ThemeItems))
            {
                string entryName = (string)null;
                if (resourceThemeItem is ImageResourceThemeItem)
                    entryName = VmcStudioTheme.GetResourceZipPath(resourceThemeItem, "Images");
                else if (resourceThemeItem is SoundResourceThemeItem)
                    entryName = VmcStudioTheme.GetResourceZipPath(resourceThemeItem, "Sounds");
                if (entryName == null)
                    throw new Exception("Unknown theme item type - " + (object)resourceThemeItem.GetType());
                this.ZipFile.Add(ZippedTheme.GetResourceThemeItemData(resourceThemeItem), entryName);
            }
            foreach (string fontName in Enumerable.Distinct<string>(Enumerable.Concat<string>(Enumerable.Select<FontClass, string>((IEnumerable<FontClass>)this.themeInfo.Fonts.FontClasses, (Func<FontClass, string>)(fontClass => fontClass.FontFace.FontFamily)), Enumerable.Select<FontOverride, string>(Enumerable.Where<FontOverride>((IEnumerable<FontOverride>)this.themeInfo.Fonts.FontOverrides, (Func<FontOverride, bool>)(fontOverride => fontOverride.FontFace != null)), (Func<FontOverride, string>)(fontOverride => fontOverride.FontFace.FontFamily)))))
            {
                FontFamily fontFamily = this.GetFontFamily(fontName);
                if (fontFamily != null)
                {
                    string file = FontUtil.GetFile(fontFamily);
                    if (file != null)
                        this.ZipFile.Add(file, "Fonts\\" + Path.GetFileName(file));
                }
            }
            this.ZipFile.CommitUpdate();
            this.ResetZipFile();
            foreach (IThemeItem themeItem in Enumerable.Where<IThemeItem>((IEnumerable<IThemeItem>)this.ThemeItems, (Func<IThemeItem, bool>)(t => t.IsDirty)))
                themeItem.ClearDirty();
            this.FontsItem.ClearDirty();
            this.ColorsItem.ClearDirty();
            this.IsDirty = false;
        }

        protected override IEnumerable<string> LoadFontFiles()
        {
            List<string> list = new List<string>();
            foreach (ZipEntry entry in this.fontEntries)
            {
                byte[] buffer = ZippedTheme.ReadZipEntry(this.ZipFile, entry);
                string path = Path.Combine(Path.GetTempPath(), entry.Name.Replace('/', '\\'));
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (FileStream fileStream = System.IO.File.Create(path))
                    fileStream.Write(buffer, 0, buffer.Length);
                list.Add(path);
            }
            return (IEnumerable<string>)list;
        }

        private static string GetResourceZipPath(ResourceThemeItem item, string themeType)
        {
            return string.Format("Resources\\{0}\\{1}\\{2}", (object)Path.GetFileNameWithoutExtension(item.DllName), (object)themeType, (object)item.ResourceName);
        }

        private static void AddImageResource(BitmapSource source, ZipFile file, string name)
        {
            MemoryStream memoryStream = new MemoryStream();
            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(source));
            pngBitmapEncoder.Save((Stream)memoryStream);
            file.Add((IStaticDataSource)new ZippedTheme.StreamDataSource((Stream)memoryStream), name);
        }

        public class ThemeInfo : ISupportInitialize
        {
            public FontsThemeItem Fonts { get; set; }

            public ColorsThemeItem Colors { get; set; }

            [XmlAttribute]
            public string ID { get; set; }

            [XmlAttribute]
            public string Name { get; set; }

            [XmlAttribute]
            public string Author { get; set; }

            [XmlAttribute]
            public string Version { get; set; }

            [XmlAttribute]
            public string Comments { get; set; }

            [XmlAttribute]
            public ThemeType ThemeType { get; set; }

            public void BeginInit()
            {
                this.Fonts.BeginInit();
                this.Colors.BeginInit();
            }

            public void EndInit()
            {
                this.Fonts.EndInit();
                this.Colors.EndInit();
            }
        }
    }
}

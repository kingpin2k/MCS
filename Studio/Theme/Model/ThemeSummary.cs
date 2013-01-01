using Advent.Common.UI;
using Advent.MediaCenter.Theme;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Advent.VmcStudio.Theme.Model
{
    [Serializable]
    public class ThemeSummary : NotifyPropertyChangedBase
    {
        private BitmapSource screenshot;
        private string name;

        [XmlAttribute]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (!(this.name != value))
                    return;
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        [XmlIgnore]
        public BitmapSource Screenshot
        {
            get
            {
                if (this.screenshot == null && File.Exists(this.ScreenshotPath))
                    this.screenshot = (BitmapSource)BitmapFrame.Create((Stream)new MemoryStream(File.ReadAllBytes(this.ScreenshotPath)));
                return this.screenshot;
            }
            protected set
            {
                if (this.screenshot == value)
                    return;
                this.screenshot = value;
                this.OnPropertyChanged("Screenshot");
            }
        }

        [XmlIgnore]
        public string ScreenshotPath
        {
            get
            {
                return Path.Combine(this.BasePath, "Screenshot.png");
            }
        }

        [XmlAttribute]
        public string ID { get; set; }

        [XmlAttribute]
        public string ThemeFile { get; set; }

        [XmlIgnore]
        public string BasePath { get; set; }

        [XmlIgnore]
        public string SummaryPath
        {
            get
            {
                return ThemeSummary.GetSummaryFilePath(this.BasePath);
            }
        }

        [DefaultValue(ThemeType.Base)]
        [XmlAttribute]
        public ThemeType ThemeType { get; set; }

        public string ThemeFullPath
        {
            get
            {
                return Path.Combine(this.BasePath, this.ThemeFile);
            }
        }

        public static ThemeSummary Load(string path)
        {
            String summary_file = ThemeSummary.GetSummaryFilePath(path);
            if(!File.Exists(summary_file))
                return null;
            ThemeSummary themeSummary = (ThemeSummary)new XmlSerializer(typeof(ThemeSummary)).Deserialize((Stream)File.Open(summary_file, FileMode.Open));
            themeSummary.BasePath = path;
            return themeSummary;
        }

        public void Save()
        {
            using (MediaCenterTheme theme = this.OpenTheme())
                this.Save(theme);
        }

        public void Save(MediaCenterTheme theme)
        {
            if (this.BasePath == null)
                throw new InvalidOperationException("Theme path not set.");
            this.UpdateFromTheme(theme);
            if (File.Exists(this.SummaryPath))
                File.Delete(this.SummaryPath);
            using (FileStream fileStream = File.Open(this.SummaryPath, FileMode.CreateNew))
                new XmlSerializer(typeof(ThemeSummary)).Serialize((Stream)fileStream, (object)this);
            if (this.Screenshot != null)
            {
                using (FileStream fileStream = File.Open(this.ScreenshotPath, FileMode.OpenOrCreate))
                {
                    while (this.Screenshot.IsDownloading)
                        Application.DoEvents();
                    PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
                    pngBitmapEncoder.Frames.Add(BitmapFrame.Create(this.Screenshot));
                    pngBitmapEncoder.Save((Stream)fileStream);
                }
            }
            else
            {
                if (!File.Exists(this.ScreenshotPath))
                    return;
                File.Delete(this.ScreenshotPath);
            }
        }

        public MediaCenterTheme OpenTheme()
        {
            if (string.IsNullOrEmpty(this.BasePath) || string.IsNullOrEmpty(this.ThemeFile))
                throw new InvalidOperationException("Theme path not set.");
            MediaCenterTheme mediaCenterTheme = MediaCenterTheme.FromFile(this.ThemeFullPath);
            mediaCenterTheme.Saved += new EventHandler(this.ThemeSaved);
            return mediaCenterTheme;
        }

        private void ThemeSaved(object sender, EventArgs e)
        {
            this.Save((MediaCenterTheme)sender);
        }

        private static string GetSummaryFilePath(string basePath)
        {
            return Path.Combine(basePath, "Theme.xml");
        }

        private void UpdateFromTheme(MediaCenterTheme theme)
        {
            this.Name = theme.Name;
            this.ThemeType = theme.ThemeType;
            this.ID = theme.ID;
            this.Screenshot = theme.MainScreenshot;
        }
    }
}

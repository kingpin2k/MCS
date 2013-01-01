


using Advent.MediaCenter;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace Advent.MediaCenter.Theme
{
    public class MediaCenterFXTheme : ZippedTheme
    {
        private readonly Dictionary<IThemeItem, ZipEntry> zipEntries = new Dictionary<IThemeItem, ZipEntry>();
        private readonly List<ZipEntry> fontEntries = new List<ZipEntry>();
        public const string FileExtension = ".vmcthemepack";

        public override bool CanSave
        {
            get
            {
                return false;
            }
        }

        public MediaCenterFXTheme(string file)
            : base(file, FileMode.Open)
        {
            FontsThemeItem fonts = new FontsThemeItem();
            fonts.BeginInit();
            ColorsThemeItem colorsThemeItem = new ColorsThemeItem();
            colorsThemeItem.BeginInit();
            StartMenuThemeItem startMenuThemeItem = new StartMenuThemeItem();
            startMenuThemeItem.BeginInit();
            this.ThemeItems.Add((IThemeItem)startMenuThemeItem);
            AnimationsItem animationsItem = new AnimationsItem();
            animationsItem.BeginInit();
            this.ThemeItems.Add((IThemeItem)animationsItem);
            foreach (ZipEntry entry in this.ZipFile)
            {
                if (entry.IsFile)
                {
                    ResourceThemeItem resourceThemeItem = (ResourceThemeItem)null;
                    string str = entry.Name.ToUpper();
                    switch (Path.GetExtension(str))
                    {
                        case ".PNG":
                            if (str == "CUSTOM.THEMESHOT.PNG")
                            {
                                this.MainScreenshot = (BitmapSource)BitmapDecoder.Create(this.ZipFile.GetInputStream(entry), BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames[0];
                                break;
                            }
                            else
                            {
                                resourceThemeItem = (ResourceThemeItem)new ImageResourceThemeItem("ehres.dll", str, (Func<ResourceThemeItem, byte[]>)(themeItem => ZippedTheme.ReadZipEntry(this.ZipFile, this.zipEntries[(IThemeItem)themeItem])));
                                break;
                            }
                        case ".TTF":
                            this.fontEntries.Add(entry);
                            break;
                    }
                    if (resourceThemeItem != null)
                    {
                        this.zipEntries[(IThemeItem)resourceThemeItem] = entry;
                        this.ThemeItems.Add((IThemeItem)resourceThemeItem);
                    }
                }
            }
            ZipEntry entry1 = this.ZipFile.GetEntry("Theme.xml");
            if (entry1 == null)
                throw new InvalidDataException("Could not find Theme.xml.");
            XDocument xdocument = XDocument.Load((XmlReader)new XmlTextReader(this.ZipFile.GetInputStream(entry1)));
            if (xdocument.Root != null)
            {
                foreach (XElement xelement in xdocument.Root.Elements())
                {
                    switch (xelement.Name.LocalName)
                    {
                        case "Biography":
                            this.Name = MediaCenterUtil.AttributeValue(xelement, (XName)"Name");
                            this.Author = MediaCenterUtil.AttributeValue(xelement, (XName)"Author");
                            this.Version = new Version(MediaCenterUtil.AttributeValue(xelement, (XName)"Version"));
                            this.Comments = MediaCenterUtil.AttributeValue(xelement, (XName)"Comments");
                            continue;
                        case "Sounds":
                            using (IEnumerator<XAttribute> enumerator = xelement.Attributes().GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    XAttribute current = enumerator.Current;
                                    string localName = current.Name.LocalName;
                                    ZipEntry entry2 = this.ZipFile.GetEntry(current.Value);
                                    SoundResourceThemeItem resourceThemeItem = new SoundResourceThemeItem("ehres.dll", localName, (Func<ResourceThemeItem, byte[]>)(themeItem => ZippedTheme.ReadZipEntry(this.ZipFile, this.zipEntries[(IThemeItem)themeItem])));
                                    this.zipEntries[(IThemeItem)resourceThemeItem] = entry2;
                                    this.ThemeItems.Add((IThemeItem)resourceThemeItem);
                                }
                                continue;
                            }
                        case "MainFonts":
                            using (IEnumerator<XAttribute> enumerator = xelement.Attributes().GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    XAttribute current = enumerator.Current;
                                    FontClass fontClass = new FontClass()
                                    {
                                        Name = current.Name.LocalName
                                    };
                                    fontClass.FontFace = FontUtilities.GetFontFaceInfo(current.Value, (MediaCenterTheme)this);
                                    if (fontClass.FontFace != null)
                                        fonts.FontClasses.Add(fontClass);
                                }
                                continue;
                            }
                        case "FontsOverrides":
                            MediaCenterFXTheme.AddFontOverride(fonts, xelement, "FontDialogC", "DialogCSize", "DialogContent");
                            MediaCenterFXTheme.AddFontOverride(fonts, xelement, "FontButton", "SizeButton", "ButtonText");
                            MediaCenterFXTheme.AddFontOverride(fonts, xelement, "FontDialogT", "DialogTSize", "DialogTitle");
                            MediaCenterFXTheme.AddFontOverride(fonts, xelement, "FontThumbnail", "ThumbnailSize", "ThumbnailButtonText");
                            MediaCenterFXTheme.AddFontOverride(fonts, xelement, "FontVolume", "VolumeSize", "VolumeText");
                            MediaCenterFXTheme.AddFontOverride(fonts, xelement, "FontBackground", "SizeBackground", "BackgroundTitleText");
                            continue;
                        case "MainColors":
                            using (IEnumerator<XAttribute> enumerator = xelement.Attributes().GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    XAttribute current = enumerator.Current;
                                    Color color;
                                    if (this.TryParseColor(current.Value, out color))
                                        colorsThemeItem.DefaultColors.Add(new ColorItem()
                                        {
                                            Color = color,
                                            Name = current.Name.LocalName
                                        });
                                    else
                                        Trace.TraceWarning("Could not parse color value for {0} \"{1}\" in theme file {2}.", (object)current.Name, (object)current.Value, (object)file);
                                }
                                continue;
                            }
                        case "StartMenu":
                            startMenuThemeItem.StripTitleFont = this.GetFontOverride(MediaCenterUtil.AttributeValue(xelement, (XName)"FontFace"), MediaCenterUtil.AttributeValue(xelement, (XName)"FontSize"));
                            Color color1;
                            if (this.TryParseColor(MediaCenterUtil.AttributeValue(xelement, (XName)"FocusColor"), out color1))
                                startMenuThemeItem.FocusedStripTitleColor = new ColorReference()
                                {
                                    Color = new Color?(color1)
                                };
                            Color color2;
                            if (this.TryParseColor(MediaCenterUtil.AttributeValue(xelement, (XName)"NoFocusColor"), out color2))
                            {
                                startMenuThemeItem.NonFocusedStripTitleColor = new ColorReference()
                                {
                                    Color = new Color?(color2)
                                };
                                continue;
                            }
                            else
                                continue;
                        case "QuickLink":
                            startMenuThemeItem.FocusedQuickLinkFont = this.GetFontOverride(MediaCenterUtil.AttributeValue(xelement, (XName)"SelectedFontFace"), MediaCenterUtil.AttributeValue(xelement, (XName)"SelectedFontSize"));
                            startMenuThemeItem.NonFocusedQuickLinkFont = this.GetFontOverride(MediaCenterUtil.AttributeValue(xelement, (XName)"NonSelectedFontFace"), MediaCenterUtil.AttributeValue(xelement, (XName)"NonSelectedFontSize"));
                            if (startMenuThemeItem.FocusedQuickLinkFont.Size == 0)
                                startMenuThemeItem.FocusedQuickLinkFont.Size = startMenuThemeItem.NonFocusedQuickLinkFont.Size;
                            if (startMenuThemeItem.NonFocusedQuickLinkFont.Size == 0)
                                startMenuThemeItem.NonFocusedQuickLinkFont.Size = startMenuThemeItem.FocusedQuickLinkFont.Size;
                            Color color3;
                            if (this.TryParseColor(MediaCenterUtil.AttributeValue(xelement, (XName)"SelectedColor"), out color3))
                                startMenuThemeItem.FocusedQuickLinkColor = new ColorReference()
                                {
                                    Color = new Color?(color3)
                                };
                            Color color4;
                            if (this.TryParseColor(MediaCenterUtil.AttributeValue(xelement, (XName)"NonSelectedColor"), out color4))
                            {
                                startMenuThemeItem.NonFocusedQuickLinkColor = new ColorReference()
                                {
                                    Color = new Color?(color4)
                                };
                                continue;
                            }
                            else
                                continue;
                        case "StartText":
                            startMenuThemeItem.StartMenuText = MediaCenterUtil.AttributeValue(xelement, (XName)"String");
                            startMenuThemeItem.StartMenuTextFont = this.GetFontOverride(MediaCenterUtil.AttributeValue(xelement, (XName)"FontFace"), MediaCenterUtil.AttributeValue(xelement, (XName)"FontSize"));
                            Color color5;
                            if (this.TryParseColor(MediaCenterUtil.AttributeValue(xelement, (XName)"Color"), out color5))
                            {
                                startMenuThemeItem.StartMenuTextColor = new ColorReference()
                                {
                                    Color = new Color?(color5)
                                };
                                continue;
                            }
                            else
                                continue;
                        case "MainBackgroundAnimation":
                            string str = MediaCenterUtil.AttributeValue(xelement, (XName)"BackgroundAnimationDisabled");
                            animationsItem.IsBackgroundAnimationDisabled = str != null && bool.Parse(str);
                            continue;
                        default:
                            continue;
                    }
                }
            }
            this.ID = this.Name;
            fonts.EndInit();
            colorsThemeItem.EndInit();
            startMenuThemeItem.EndInit();
            animationsItem.EndInit();
            this.FontsItem = fonts;
            this.ColorsItem = colorsThemeItem;
        }

        protected override void SaveInternal()
        {
            throw new InvalidOperationException("MediaCenterFX themes cannot be saved.");
        }

        protected override IEnumerable<string> LoadFontFiles()
        {
            List<string> list = new List<string>();
            foreach (ZipEntry entry in this.fontEntries)
            {
                byte[] buffer = ZippedTheme.ReadZipEntry(this.ZipFile, entry);
                string path = Path.Combine(Path.GetTempPath(), entry.Name);
                using (FileStream fileStream = System.IO.File.Create(path))
                    fileStream.Write(buffer, 0, buffer.Length);
                list.Add(path);
            }
            return (IEnumerable<string>)list;
        }

        private FontOverride GetFontOverride(string fontFace, string fontSize)
        {
            FontFace fontFace1 = (FontFace)null;
            if (fontFace != null)
                fontFace1 = FontUtilities.GetFontFaceInfo(fontFace, (MediaCenterTheme)this);
            int result = 0;
            if (fontSize != null && !int.TryParse(fontSize, out result))
                Trace.TraceWarning("Could not parse font size value {0} in theme file {1}.", (object)fontSize, (object)this.File);
            return new FontOverride()
            {
                FontFace = fontFace1,
                Size = result
            };
        }

        private static void AddFontOverride(FontsThemeItem fonts, XElement node, string fontName, string fontSize, string fontMC)
        {
            FontOverride fontOverride = MediaCenterFXTheme.GetFontOverride(node, fontName, fontSize, fontMC);
            if (fontOverride == null)
                return;
            fonts.FontOverrides.Add(fontOverride);
        }

        private static FontOverride GetFontOverride(XElement node, string fontName, string fontSize, string fontMC)
        {
            XAttribute xattribute = node.Attribute((XName)fontName);
            if (xattribute == null)
                return (FontOverride)null;
            string str = (string)null;
            switch (xattribute.Value)
            {
                case "Main Font":
                    str = "MainFontFace";
                    break;
                case "Regular Font":
                    str = "RegularFontFace";
                    break;
                case "Light Font":
                    str = "LightFontFace";
                    break;
            }
            FontOverride fontOverride = new FontOverride()
            {
                Name = fontMC,
                FontClass = str
            };
            int result;
            fontOverride.Size = int.TryParse(MediaCenterUtil.AttributeValue(node, (XName)fontSize), out result) ? result : 20;
            return fontOverride;
        }

        private bool TryParseColor(string colorText, out Color color)
        {
            int result;
            if (int.TryParse(colorText, out result))
            {
                color = this.IntToColor(result);
                return true;
            }
            else
            {
                color = new Color();
                return false;
            }
        }

        private Color IntToColor(int color)
        {
            return Color.FromArgb((byte)(color >> 24), (byte)(color >> 16), (byte)(color >> 8), (byte)color);
        }
    }
}

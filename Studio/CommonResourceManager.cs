using Advent.Common;
using Advent.Common.IO;
using Advent.Common.UI;
using Advent.MediaCenter;
using Advent.MediaCenter.Theme;
using Advent.VmcStudio.Theme.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Advent.VmcStudio
{
    public class CommonResourceManager : NotifyPropertyChangedBase
    {
        private readonly Dictionary<string, ImageSource> images;
        private readonly VmcStudioApp app;
        private Dictionary<string, FontFamily> fonts;
        private Dictionary<string, Color> colors;
        private MediaCenterLibraryCache cache;

        public MediaCenterLibraryCache LibraryCache
        {
            get
            {
                return this.cache;
            }
        }

        public Color TextColor
        {
            get
            {
                return this.GetColor("White80Percent");
            }
        }

        public FontFamily MainFont
        {
            get
            {
                return this.GetFontFamily("MainFontFace");
            }
        }

        public FontFamily RegularFont
        {
            get
            {
                return this.GetFontFamily("RegularFontFace");
            }
        }

        public Color HighlightColor
        {
            get
            {
                return this.GetColor("LightBlue");
            }
        }

        public ImageSource RadioImage
        {
            get
            {
                return this.GetImage("res://ehres!STARTMENU.QUICKLINK.RADIO.FOCUS.PNG");
            }
        }

        public ImageSource MusicImage
        {
            get
            {
                return this.GetImage("res://ehres!STARTMENU.QUICKLINK.MUSIC.FOCUS.PNG");
            }
        }

        public ImageSource GuideImage
        {
            get
            {
                return this.GetImage("res://ehres!STARTMENU.QUICKLINK.GUIDE.FOCUS.PNG");
            }
        }

        public ImageSource CommonBackground
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.BACKGROUND.PNG");
            }
        }

        public ImageSource QuickLinkBackground
        {
            get
            {
                return this.GetImage("res://ehres!STARTMENU.QUICKLINK.BACKGROUND.PNG");
            }
        }

        public ImageSource ChevronLeftImage
        {
            get
            {
                return this.GetImage("res://ehres!AUTOSCROLL.CHEVRON.LEFT.PNG");
            }
        }

        public ImageSource QuickLinkFocusImage
        {
            get
            {
                return this.GetImage("res://ehres!SELECTOR.FOCUSLOOP.GLOW.GLASS.PNG");
            }
        }

        public ImageSource ChevronLeftPressedImage
        {
            get
            {
                return this.GetImage("res://ehres!AUTOSCROLL.CHEVRON.LEFT.PRESSED.PNG");
            }
        }

        public ImageSource ChevronRightImage
        {
            get
            {
                return this.GetImage("res://ehres!AUTOSCROLL.CHEVRON.RIGHT.PNG");
            }
        }

        public ImageSource ChevronRightPressedImage
        {
            get
            {
                return this.GetImage("res://ehres!AUTOSCROLL.CHEVRON.RIGHT.PRESSED.PNG");
            }
        }

        public ImageSource ChevronUpImage
        {
            get
            {
                return this.GetImage("res://ehres!AUTOSCROLL.CHEVRON.TOP.PNG");
            }
        }

        public ImageSource ChevronUpPressedImage
        {
            get
            {
                return this.GetImage("res://ehres!AUTOSCROLL.CHEVRON.TOP.PRESSED.PNG");
            }
        }

        public ImageSource ChevronDownImage
        {
            get
            {
                return this.GetImage("res://ehres!AUTOSCROLL.CHEVRON.BOTTOM.PNG");
            }
        }

        public ImageSource ChevronDownPressedImage
        {
            get
            {
                return this.GetImage("res://ehres!AUTOSCROLL.CHEVRON.BOTTOM.PRESSED.PNG");
            }
        }

        public ImageSource ScrollDownDisabledImage
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.SCROLL.DOWN.DISABLED.PNG");
            }
        }

        public ImageSource ScrollDownFocusImage
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.SCROLL.DOWN.FOCUS.PNG");
            }
        }

        public ImageSource ScrollDownNoFocusImage
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.SCROLL.DOWN.NOFOCUS.PNG");
            }
        }

        public ImageSource ScrollDownPressedImage
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.SCROLL.DOWN.PRESSED.PNG");
            }
        }

        public ImageSource ScrollUpDisabledImage
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.SCROLL.UP.DISABLED.PNG");
            }
        }

        public ImageSource ScrollUpFocusImage
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.SCROLL.UP.FOCUS.PNG");
            }
        }

        public ImageSource ScrollUpNoFocusImage
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.SCROLL.UP.NOFOCUS.PNG");
            }
        }

        public ImageSource ScrollUpPressedImage
        {
            get
            {
                return this.GetImage("res://ehres!COMMON.SCROLL.UP.PRESSED.PNG");
            }
        }

        public ImageSource PlayImage
        {
            get
            {
                return this.GetImage("res://ehres!TWOFOOT.CONTROL.PLAY.FOCUS.PNG");
            }
        }

        public ImageSource PlayDisabledImage
        {
            get
            {
                return this.GetImage("res://ehres!TWOFOOT.CONTROL.PLAY.DISABLED.PNG");
            }
        }

        internal CommonResourceManager(VmcStudioApp app)
        {
            this.app = app;
            this.images = new Dictionary<string, ImageSource>();
            this.ResetResources();
            app.ThemeManager.ApplyingThemes += new EventHandler<ApplyThemesEventArgs>(this.ThemeManagerApplyingThemes);
        }

        public void CloseResources()
        {
            if (this.cache == null)
                return;
            this.cache.Dispose();
            this.cache = (MediaCenterLibraryCache)null;
        }

        public void ResetResources()
        {
            if (this.cache == null)
                this.cache = new MediaCenterLibraryCache();
            this.images.Clear();
            this.colors = null;
            this.fonts = null;
            foreach (MemberInfo memberInfo in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                this.OnPropertyChanged(memberInfo.Name);
        }

        private void ThemeManagerApplyingThemes(object sender, ApplyThemesEventArgs e)
        {
            this.CloseResources();
            e.ApplyOperation.Completed += new EventHandler<EventArgs>(this.ApplyOperationCompleted);
            e.ApplyOperation.Abandoned += new EventHandler<Advent.Common.ExceptionEventArgs>(this.ApplyOperationCompleted);
        }

        private void ApplyOperationCompleted(object sender, EventArgs e)
        {
            this.app.Dispatcher.Invoke((Delegate)new Action(this.ResetResources), new object[0]);
        }

        private ImageSource GetImage(string resourceUrl)
        {
            ImageSource imageResource;
            if (!this.images.TryGetValue(resourceUrl, out imageResource))
            {
                imageResource = MediaCenterUtil.GetImageResource((IResourceLibraryCache)this.cache, resourceUrl);
                this.images[resourceUrl] = imageResource;
                if (imageResource == null)
                    Trace.TraceWarning("Could not find image resource: {0}", new object[1]
          {
            (object) resourceUrl
          });
            }
            return imageResource;
        }

        private Color GetColor(string colorName)
        {
            if (this.colors == null)
                this.colors = Enumerable.ToDictionary<ColorItem, string, Color>(ColorsThemeItem.GetColors(this.cache), (Func<ColorItem, string>)(o => o.Name), (Func<ColorItem, Color>)(o => o.Color));
            return this.colors[colorName];
        }

        private static FontFamily backup_font = null;

        private static FontFamily BackupFont
        {
            get
            {
                if (backup_font == null)
                {
                    backup_font = new FontFamily("Segoe UI");
                }
                return backup_font;
            }
        }

        private FontFamily GetFontFamily(string name)
        {
            if (this.fonts == null)
            {
                this.fonts = new Dictionary<string, FontFamily>();
                foreach (FontClass fontClass in FontsThemeItem.GetFontClasses(this.cache))
                    this.fonts[fontClass.Name] = new FontFamily(fontClass.FontFace.FontFamily);
            }

            if (this.fonts.ContainsKey(name))
                return this.fonts[name];
            else
                return CommonResourceManager.BackupFont;
        }
    }
}

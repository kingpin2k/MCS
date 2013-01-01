


using Advent.MediaCenter.Theme.Default;
using Advent.MediaCenter.Theme.TVPack;
using Advent.MediaCenter.Theme.Windows7;
using System.ComponentModel;

namespace Advent.MediaCenter.Theme
{
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.Default.StartMenuApplicator), MajorVersion = 6, MinorVersion = 0)]
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.TVPack.StartMenuApplicator), BuildVersionMax = 7599, MajorVersion = 6, MinorVersion = 1)]
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.Windows7.StartMenuApplicator), BuildVersionMin = 7600, MajorVersion = 6, MinorVersion = 1)]
    public class StartMenuThemeItem : ThemeItemBase
    {
        private FontOverride stripTitleFont;
        private FontOverride focusedLinkFont;
        private FontOverride nonFocusedLinkFont;
        private FontOverride backgroundTextFont;
        private ColorReference backgroundTextColor;
        private ColorReference focusedLinkColor;
        private ColorReference nonFocusedLinkColor;
        private ColorReference focusedStripTitleColor;
        private ColorReference nonFocusedStripTitleColor;
        private string startMenuText;

        public override string Name
        {
            get
            {
                return "Start Menu";
            }
        }

        public string StartMenuText
        {
            get
            {
                return this.startMenuText;
            }
            set
            {
                this.startMenuText = value;
                this.OnPropertyChanged("StartMenuText");
            }
        }

        [DefaultValue(null)]
        public FontOverride StripTitleFont
        {
            get
            {
                return this.stripTitleFont;
            }
            set
            {
                this.stripTitleFont = value;
                this.OnPropertyChanged("StripTitleFont");
            }
        }

        public ColorReference FocusedStripTitleColor
        {
            get
            {
                return this.focusedStripTitleColor;
            }
            set
            {
                this.focusedStripTitleColor = value;
                this.OnPropertyChanged("FocusedStripTitleColor");
            }
        }

        public ColorReference NonFocusedStripTitleColor
        {
            get
            {
                return this.nonFocusedStripTitleColor;
            }
            set
            {
                this.nonFocusedStripTitleColor = value;
                this.OnPropertyChanged("NonFocusedStripTitleColor");
            }
        }

        [DefaultValue(null)]
        public FontOverride FocusedQuickLinkFont
        {
            get
            {
                return this.focusedLinkFont;
            }
            set
            {
                this.focusedLinkFont = value;
                this.OnPropertyChanged("FocusedQuickLinkFont");
            }
        }

        [DefaultValue(null)]
        public FontOverride NonFocusedQuickLinkFont
        {
            get
            {
                return this.nonFocusedLinkFont;
            }
            set
            {
                this.nonFocusedLinkFont = value;
                this.OnPropertyChanged("NonFocusedQuickLinkFont");
            }
        }

        public FontOverride StartMenuTextFont
        {
            get
            {
                return this.backgroundTextFont;
            }
            set
            {
                this.backgroundTextFont = value;
                this.OnPropertyChanged("StartMenuTextFont");
            }
        }

        public ColorReference StartMenuTextColor
        {
            get
            {
                return this.backgroundTextColor;
            }
            set
            {
                this.backgroundTextColor = value;
                this.OnPropertyChanged("StartMenuTextColor");
            }
        }

        public ColorReference FocusedQuickLinkColor
        {
            get
            {
                return this.focusedLinkColor;
            }
            set
            {
                this.focusedLinkColor = value;
                this.OnPropertyChanged("FocusedLinkColor");
            }
        }

        public ColorReference NonFocusedQuickLinkColor
        {
            get
            {
                return this.nonFocusedLinkColor;
            }
            set
            {
                this.nonFocusedLinkColor = value;
                this.OnPropertyChanged("NonFocusedQuickLinkColor");
            }
        }

        protected override void LoadInternal()
        {
        }
    }
}

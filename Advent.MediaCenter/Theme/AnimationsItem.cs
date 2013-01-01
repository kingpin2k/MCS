


using Advent.MediaCenter.Theme.Default;
using Advent.MediaCenter.Theme.TVPack;
using Advent.MediaCenter.Theme.Windows7;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.Windows7.AnimationsApplicator), BuildVersionMin = 7600, MajorVersion = 6, MinorVersion = 1)]
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.TVPack.AnimationsApplicator), BuildVersionMax = 7599, MajorVersion = 6, MinorVersion = 1)]
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.Default.AnimationsApplicator), MajorVersion = 6, MinorVersion = 0)]
    public class AnimationsItem : ThemeItemBase
    {
        private bool isBackgroundDisabled;

        public override string Name
        {
            get
            {
                return "Animations";
            }
        }

        [XmlAttribute]
        public bool IsBackgroundAnimationDisabled
        {
            get
            {
                return this.isBackgroundDisabled;
            }
            set
            {
                if (this.isBackgroundDisabled == value)
                    return;
                this.isBackgroundDisabled = value;
                this.OnPropertyChanged("IsBackgroundAnimationDisabled");
            }
        }

        protected override void LoadInternal()
        {
        }
    }
}




using Advent.Common.UI;
using Advent.MediaCenter;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    public class ColorItem : NotifyPropertyChangedBase
    {
        private string name;
        private Color color;

        [XmlAttribute]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        [XmlIgnore]
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
                this.OnPropertyChanged("Color");
            }
        }

        [XmlAttribute("Color")]
        public string ColorString
        {
            get
            {
                return this.ToString();
            }
            set
            {
                Color color;
                MediaCenterUtil.TryParseColor(value, out color);
                this.Color = color;
            }
        }

        public override string ToString()
        {
            return MediaCenterUtil.ColorToString(this.Color);
        }
    }
}

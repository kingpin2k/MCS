


using Advent.Common.UI;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    public class FontFace : NotifyPropertyChangedBase
    {
        private string fontFamily;
        private FontWeight fontWeight;

        [XmlAttribute]
        public string FontFamily
        {
            get
            {
                return this.fontFamily;
            }
            set
            {
                this.fontFamily = value;
                this.OnPropertyChanged("FontFamily");
            }
        }

        [XmlIgnore]
        public FontWeight FontWeight
        {
            get
            {
                return this.fontWeight;
            }
            set
            {
                this.fontWeight = value;
                this.OnPropertyChanged("FontWeight");
            }
        }

        [XmlAttribute("FontWeight")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FontWeightString
        {
            get
            {
                return new FontWeightConverter().ConvertToString((object)this.FontWeight);
            }
            set
            {
                this.FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(value);
            }
        }

        public FontFace()
        {
            this.fontWeight = FontWeights.Normal;
        }

        public FontFace(FontFace source)
        {
            this.fontFamily = source.FontFamily;
            this.fontWeight = source.FontWeight;
        }

        public override bool Equals(object obj)
        {
            FontFace fontFace = obj as FontFace;
            if (fontFace == null || !(this.FontWeight == fontFace.FontWeight))
                return false;
            else
                return this.FontFamily == fontFace.FontFamily;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

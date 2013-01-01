


using Advent.Common.UI;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    public class FontOverride : NotifyPropertyChangedBase
    {
        private string name;
        private string fontClass;
        private FontFace typeface;
        private int size;
        private bool isBold;
        private bool isItalic;

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

        [DefaultValue(null)]
        [XmlAttribute]
        public string FontClass
        {
            get
            {
                return this.fontClass;
            }
            set
            {
                this.fontClass = value;
                this.OnPropertyChanged("FontClass");
            }
        }

        public FontFace FontFace
        {
            get
            {
                return this.typeface;
            }
            set
            {
                this.typeface = value;
                this.OnPropertyChanged("FontFace");
            }
        }

        [DefaultValue(0)]
        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
                this.OnPropertyChanged("Size");
            }
        }

        public bool IsBold
        {
            get
            {
                return this.isBold;
            }
            set
            {
                this.isBold = value;
                this.OnPropertyChanged("IsBold");
            }
        }

        public bool IsItalic
        {
            get
            {
                return this.isItalic;
            }
            set
            {
                this.isItalic = value;
                this.OnPropertyChanged("IsItalic");
            }
        }
    }
}

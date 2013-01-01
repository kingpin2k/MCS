


using Advent.Common.UI;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    public class FontClass : NotifyPropertyChangedBase
    {
        private string name;
        private FontFace typeface;

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

        public FontFace FontFace
        {
            get
            {
                return this.typeface;
            }
            set
            {
                if (this.typeface != null)
                    this.typeface.PropertyChanged -= new PropertyChangedEventHandler(this.Typeface_PropertyChanged);
                this.typeface = value;
                if (this.typeface != null)
                    this.typeface.PropertyChanged += new PropertyChangedEventHandler(this.Typeface_PropertyChanged);
                this.OnPropertyChanged("FontFace");
            }
        }

        public FontClass()
        {
        }

        public FontClass(FontClass source)
        {
            this.name = source.Name;
            this.FontFace = new FontFace(source.FontFace);
        }

        private void Typeface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("FontFace");
        }
    }
}

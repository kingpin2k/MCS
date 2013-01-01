


using Advent.Common.UI;
using Advent.MediaCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    public class ColorReference : NotifyPropertyChangedBase
    {
        private string colorItemName;
        private Color? color;

        [XmlAttribute]
        public string ColorName
        {
            get
            {
                return this.colorItemName;
            }
            set
            {
                this.colorItemName = value;
                this.OnPropertyChanged("ColorName");
            }
        }

        [XmlIgnore]
        public Color? Color
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
                if (!this.Color.HasValue)
                    return (string)null;
                else
                    return MediaCenterUtil.ColorToString(this.Color.Value);
            }
            set
            {
                if (value == null)
                {
                    this.Color = new Color?();
                }
                else
                {
                    Color color;
                    MediaCenterUtil.TryParseColor(value, out color);
                    this.Color = new Color?(color);
                }
            }
        }

        public Color? GetColor(MediaCenterTheme theme)
        {
            if (this.Color.HasValue)
                return this.Color;
            ColorItem colorItem = Enumerable.FirstOrDefault<ColorItem>((IEnumerable<ColorItem>)theme.ColorsItem.DefaultColors, (Func<ColorItem, bool>)(o => o.Name == this.ColorName));
            if (colorItem != null)
                return new Color?(colorItem.Color);
            else
                return new Color?();
        }
    }
}

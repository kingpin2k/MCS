using Advent.MediaCenter.Theme;
using System.Windows.Media;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ColorItemModel : ViewModelItem<ColorItem, Color?>
    {
        public byte A
        {
            get
            {
                return this.Item.Color.A;
            }
            set
            {
                this.Item.Color = new Color()
                {
                    A = value,
                    R = this.Item.Color.R,
                    G = this.Item.Color.G,
                    B = this.Item.Color.B
                };
                this.OnPropertyChanged("Value");
                this.OnPropertyChanged("IsDefault");
                this.OnPropertyChanged("IsDirty");
                this.OnPropertyChanged("A");
            }
        }

        public byte R
        {
            get
            {
                return this.Item.Color.R;
            }
            set
            {
                this.Item.Color = new Color()
                {
                    A = this.Item.Color.A,
                    R = value,
                    G = this.Item.Color.G,
                    B = this.Item.Color.B
                };
                this.OnPropertyChanged("Value");
                this.OnPropertyChanged("IsDefault");
                this.OnPropertyChanged("IsDirty");
                this.OnPropertyChanged("R");
            }
        }

        public byte G
        {
            get
            {
                return this.Item.Color.G;
            }
            set
            {
                this.Item.Color = new Color()
                {
                    A = this.Item.Color.A,
                    R = this.Item.Color.R,
                    G = value,
                    B = this.Item.Color.B
                };
                this.OnPropertyChanged("Value");
                this.OnPropertyChanged("IsDefault");
                this.OnPropertyChanged("IsDirty");
                this.OnPropertyChanged("G");
            }
        }

        public byte B
        {
            get
            {
                return this.Item.Color.B;
            }
            set
            {
                this.Item.Color = new Color()
                {
                    A = this.Item.Color.A,
                    R = this.Item.Color.R,
                    G = this.Item.Color.G,
                    B = value
                };
                this.OnPropertyChanged("Value");
                this.OnPropertyChanged("IsDefault");
                this.OnPropertyChanged("IsDirty");
                this.OnPropertyChanged("B");
            }
        }

        public override Color? Value
        {
            get
            {
                return new Color?(this.Item.Color);
            }
        }

        public ColorItemModel(ColorItem item)
            : base(item)
        {
        }

        public override Color? Clone(Color? obj)
        {
            return obj;
        }
    }
}

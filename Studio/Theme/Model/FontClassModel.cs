using Advent.MediaCenter.Theme;

namespace Advent.VmcStudio.Theme.Model
{
    internal class FontClassModel : ViewModelItem<FontClass, FontFace>
    {
        public override FontFace Value
        {
            get
            {
                return this.Item.FontFace;
            }
        }

        public FontClassModel(FontClass fontClass)
            : base(fontClass)
        {
            this.OnValueChanged();
        }

        public override FontFace Clone(FontFace obj)
        {
            return new FontFace(obj);
        }

        public override bool IsEqual(FontFace a, FontFace b)
        {
            if (a == null && b != null || a != null && b == null || !(a.FontFamily == b.FontFamily))
                return false;
            else
                return a.FontWeight == b.FontWeight;
        }
    }
}

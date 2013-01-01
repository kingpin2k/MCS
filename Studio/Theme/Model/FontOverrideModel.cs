using Advent.MediaCenter.Theme;

namespace Advent.VmcStudio.Theme.Model
{
    internal class FontOverrideModel : ViewModelItem<FontOverride, FontOverride>
    {
        private string nameOverride;

        public FontsItemModel FontsItem { get; private set; }

        public string Name
        {
            get
            {
                if (this.nameOverride != null)
                    return this.nameOverride;
                else
                    return this.Item.Name;
            }
        }

        public override FontOverride Value
        {
            get
            {
                return this.Item;
            }
        }

        public FontOverrideModel(FontOverride item, FontsItemModel fonts)
            : base(item)
        {
            this.FontsItem = fonts;
        }

        public FontOverrideModel(FontOverride item, FontsItemModel fonts, string name)
            : this(item, fonts)
        {
            this.nameOverride = name;
        }

        public override bool IsEqual(FontOverride a, FontOverride b)
        {
            if (a.FontClass == b.FontClass && (a.FontFace == null && b.FontFace == null || a.FontFace.Equals((object)b.FontFace)) && (a.IsBold == b.IsBold && a.IsItalic == b.IsItalic && a.Name == b.Name))
                return a.Size == b.Size;
            else
                return false;
        }

        public override FontOverride Clone(FontOverride obj)
        {
            if (obj == null)
                return (FontOverride)null;
            return new FontOverride()
            {
                FontClass = this.Item.FontClass,
                FontFace = this.Item.FontFace == null ? (FontFace)null : new FontFace(this.Item.FontFace),
                IsBold = this.Item.IsBold,
                IsItalic = this.Item.IsItalic,
                Name = this.Item.Name,
                Size = this.Item.Size
            };
        }
    }
}

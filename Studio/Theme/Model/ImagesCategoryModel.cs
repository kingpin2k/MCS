using Advent.MediaCenter.Theme;
using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ImagesCategoryModel : ResourceCategoryModel<ImageResourceModel>
    {
        internal ImagesCategoryModel(string name, ThemeTreeItem parent)
            : base(name, parent)
        {
        }

        internal void Import(string[] files)
        {
            ImagesCategoryModel imagesCategoryModel = this.Parent as ImagesCategoryModel;
            if (imagesCategoryModel != null)
            {
                imagesCategoryModel.Import(files);
            }
            else
            {
                foreach (string path in files)
                {
                    string fileName = Path.GetFileName(path);
                    ImageResourceModel imageResourceModel = Enumerable.FirstOrDefault<ImageResourceModel>(this.GetThemeItems(), (Func<ImageResourceModel, bool>)(o => o.ThemeItem.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)));
                    if (imageResourceModel != null)
                    {
                        imageResourceModel.ThemeItem.Load();
                        MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(path));
                        ((ImageResourceThemeItem)imageResourceModel.ThemeItem).Image = (BitmapSource)BitmapFrame.Create((Stream)memoryStream);
                        imageResourceModel.IsExpanded = true;
                    }
                }
            }
        }
    }
}

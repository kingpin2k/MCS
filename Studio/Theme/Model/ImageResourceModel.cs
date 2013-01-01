using Advent.Common.Interop;
using Advent.Common.UI;
using Advent.MediaCenter.Theme;
using Advent.VmcStudio;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ImageResourceModel : ThemeItemModel
    {
        public const string BaseCategory = "Images";

        public override string Category
        {
            get
            {
                return this.GetCategory(this.ThemeItem.Name, "Images");
            }
        }

        public ImageResourceModel(IThemeItem themeItem, ThemeModel theme, bool isDefault)
            : base(themeItem, theme, isDefault)
        {
        }

        public override ThemeItemCategoryModel CreateCategory(string name, ThemeTreeItem parent)
        {
            return (ThemeItemCategoryModel)new ImagesCategoryModel(name, parent);
        }

        private string GetCategory(string item, string baseCategory)
        {
            string str1 = baseCategory;
            string str2 = item.ToUpperInvariant();
            string str3;
            if (str2.StartsWith("AUDIO"))
                str3 = str1 + "\\Audio";
            else if (str2.StartsWith("COMMON"))
            {
                str3 = str1 + "\\Common";
                if (str2.IndexOf("BACKGROUND") != -1)
                    str3 = str3 + "\\Background";
                else if (str2.IndexOf("BUTTON") != -1)
                    str3 = str3 + "\\Buttons";
                else if (str2.IndexOf("FOLDER") != -1)
                    str3 = str3 + "\\Folders";
                else if (str2.IndexOf("SCROLL") != -1)
                    str3 = str3 + "\\Scroll";
                else if (str2.IndexOf("STAR") != -1)
                    str3 = str3 + "\\Star";
                else if (str2.IndexOf("TRANSPORTFEEDBACK") != -1)
                    str3 = str3 + "\\Transport";
            }
            else
                str3 = !str2.StartsWith("BROWSE") ? (!str2.StartsWith("DETAILS") ? (str2.StartsWith("EPG") || str2.StartsWith("GUIDE") ? str1 + "\\EPG" : (!str2.StartsWith("FIRSTRUN") ? (!str2.StartsWith("GLOBALSETTINGS") ? (!str2.StartsWith("HC1") ? (!str2.StartsWith("HC2") ? (!str2.StartsWith("STARTMENU") ? str1 + "\\Other" : str1 + "\\Start Menu") : this.GetCategory(str2.Substring(4), str1 + "\\HC2")) : this.GetCategory(str2.Substring(4), str1 + "\\HC1")) : str1 + "\\Global Settings") : str1 + "\\First Run")) : str1 + "\\Details") : str1 + "\\Browse";
            return str3;
        }

        public override DragDropEffects DoDragDrop(UIElement source, Point cursorPos)
        {
            this.ThemeItem.Load();
            ImageResourceThemeItem resourceThemeItem = (ImageResourceThemeItem)this.ThemeItem;
            byte[] numArray = ((ResourceThemeItem)resourceThemeItem).Save(false);
            IDataObject dataObject1 = Advent.Common.Interop.DataObject.CreateDataObject();
            IDataObject dataObject2 = dataObject1;
            VirtualFile[] virtualFileArray = new VirtualFile[1];
            virtualFileArray[0] = new VirtualFile()
            {
                Name = this.ThemeItem.Name,
                LastWriteTime = DateTime.Now,
                Contents = numArray
            };
            VirtualFile[] files = virtualFileArray;
            DataObjectExtensions.SetVirtualFiles(dataObject2, files);
            try
            {
                VmcStudioUtil.DragDropObject = (object)this;
                return DataObjectExtensions.DoDragDrop(dataObject1, source, UIExtensions.Resize(resourceThemeItem.Image, 100), cursorPos, DragDropEffects.Copy);
            }
            finally
            {
                VmcStudioUtil.DragDropObject = (object)null;
            }
        }

        public override DragDropEffects GetDropEffects(IDataObject data)
        {
            if (VmcStudioUtil.DragDropObject == this)
                return DragDropEffects.None;
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] strArray = data.GetData(DataFormats.FileDrop, true) as string[];
                if (strArray != null && strArray.Length == 1 && ImageResourceModel.IsImageFile(strArray[0]))
                    return DragDropEffects.Copy;
            }
            VirtualFile[] virtualFiles = DataObjectExtensions.GetVirtualFiles(data, false);
            return virtualFiles != null && virtualFiles.Length == 1 && ImageResourceModel.IsImageFile(virtualFiles[0].Name) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public override void AcceptData(IDataObject data)
        {
            if (this.GetDropEffects(data) == DragDropEffects.None)
                return;
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] strArray = data.GetData(DataFormats.FileDrop, true) as string[];
                if (strArray != null && strArray.Length == 1 && ImageResourceModel.IsImageFile(strArray[0]))
                    ((ImageResourceThemeItem)this.ThemeItem).Image = (BitmapSource)BitmapDecoder.Create(new Uri(strArray[0]), BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
            }
            VirtualFile[] virtualFiles = DataObjectExtensions.GetVirtualFiles(data);
            if (virtualFiles == null || virtualFiles.Length != 1 || !ImageResourceModel.IsImageFile(virtualFiles[0].Name))
                return;
            ((ImageResourceThemeItem)this.ThemeItem).Image = (BitmapSource)BitmapDecoder.Create((Stream)new MemoryStream(virtualFiles[0].Contents), BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
        }

        private static bool IsImageFile(string filename)
        {
            string str = Path.GetExtension(filename).ToUpperInvariant();
            if (!(str == ".PNG") && !(str == ".JPG") && !(str == ".BMP"))
                return str == ".GIF";
            else
                return true;
        }
    }
}

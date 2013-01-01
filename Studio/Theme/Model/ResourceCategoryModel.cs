using Advent.Common.Interop;
using Advent.MediaCenter.Theme;
using Advent.VmcStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Advent.VmcStudio.Theme.Model
{
    internal abstract class ResourceCategoryModel<T> : ThemeItemCategoryModel where T : ThemeItemModel
    {
        internal ResourceCategoryModel(string name, ThemeTreeItem parent)
            : base(name, parent)
        {
        }

        public override DragDropEffects DoDragDrop(UIElement source, Point cursorPos)
        {
            Advent.Common.Interop.DataObject dataObject1 = new Advent.Common.Interop.DataObject();
            List<VirtualFile> list = new List<VirtualFile>();
            foreach (T obj in Enumerable.Where<T>(this.GetThemeItems(), (Func<T, bool>)(o => o.Visibility == Visibility.Visible)))
            {
                obj.ThemeItem.Load();
                VirtualFile virtualFile = new VirtualFile()
                {
                    Name = obj.ThemeItem.Name,
                    LastWriteTime = DateTime.Now
                };
                if (obj.ThemeItem.IsDirty)
                {
                    virtualFile.Contents = ((ResourceThemeItem)obj.ThemeItem).Save(false);
                }
                else
                {
                    T copy = obj;
                    virtualFile.ContentsFunc = (Func<byte[]>)(() => ((ResourceThemeItem)copy.ThemeItem).OriginalBuffer);
                }
                list.Add(virtualFile);
            }
            DataObjectExtensions.SetVirtualFiles(dataObject1, list.ToArray());
            IDataObject dataObject2 = (IDataObject)new System.Windows.DataObject((object)dataObject1);
            try
            {
                VmcStudioUtil.DragDropObject = (object)list;
                return DataObjectExtensions.DoDragDrop(dataObject2, source, cursorPos, DragDropEffects.Copy);
            }
            finally
            {
                VmcStudioUtil.DragDropObject = (object)list;
            }
        }

        protected IEnumerable<T> GetThemeItems()
        {
            return this.GetThemeItems((ThemeItemCategoryModel)this);
        }

        private IEnumerable<T> GetThemeItems(ThemeItemCategoryModel item)
        {
            IEnumerable<T> first = Enumerable.OfType<T>((IEnumerable)item.Children);
            foreach (ThemeItemCategoryModel itemCategoryModel in Enumerable.OfType<ThemeItemCategoryModel>((IEnumerable)item.Children))
                first = Enumerable.Concat<T>(first, this.GetThemeItems(itemCategoryModel));
            return first;
        }
    }
}

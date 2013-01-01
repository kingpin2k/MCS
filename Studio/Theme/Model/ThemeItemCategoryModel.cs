using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ThemeItemCategoryModel : ThemeTreeItem
    {
        public ObservableCollection<ThemeTreeItem> Children { get; protected set; }

        public ThemeItemCategoryModel(string name, ThemeTreeItem parent)
        {
            this.Name = name;
            this.Parent = parent;
            this.Children = new ObservableCollection<ThemeTreeItem>();
        }

        public override int CompareTo(object obj)
        {
            if (obj is ThemeItemCategoryModel)
                return base.CompareTo(obj);
            else
                return -1;
        }

        protected void Sort()
        {
            List<ThemeTreeItem> list = new List<ThemeTreeItem>((IEnumerable<ThemeTreeItem>)this.Children);
            list.Sort();
            this.Children.Clear();
            foreach (ThemeTreeItem themeTreeItem in list)
            {
                this.Children.Add(themeTreeItem);
                ThemeItemCategoryModel itemCategoryModel = themeTreeItem as ThemeItemCategoryModel;
                if (itemCategoryModel != null)
                    itemCategoryModel.Sort();
            }
        }
    }
}

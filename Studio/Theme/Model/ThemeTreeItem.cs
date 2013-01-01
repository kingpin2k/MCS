using System;
using System.Windows;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ThemeTreeItem : DependencyObject, IComparable
    {
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ThemeTreeItem), new PropertyMetadata(new PropertyChangedCallback(ThemeTreeItem.OnIsExpandedChanged)));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ThemeTreeItem));
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(ThemeTreeItem), new PropertyMetadata((object)Visibility.Visible));

        public bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(ThemeTreeItem.IsExpandedProperty);
            }
            set
            {
                this.SetValue(ThemeTreeItem.IsExpandedProperty, value);
            }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(ThemeTreeItem.IsSelectedProperty);
            }
            set
            {
                this.SetValue(ThemeTreeItem.IsSelectedProperty, value);
            }
        }

        public Visibility Visibility
        {
            get
            {
                return (Visibility)this.GetValue(ThemeTreeItem.VisibilityProperty);
            }
            set
            {
                this.SetValue(ThemeTreeItem.VisibilityProperty, (object)value);
            }
        }

        public string Name { get; protected set; }

        public ThemeTreeItem Parent { get; internal set; }

        static ThemeTreeItem()
        {
        }

        public virtual int CompareTo(object obj)
        {
            ThemeTreeItem themeTreeItem = obj as ThemeTreeItem;
            if (themeTreeItem == null)
                return -1;
            else
                return this.Name.CompareTo(themeTreeItem.Name);
        }

        public virtual DragDropEffects GetDropEffects(IDataObject data)
        {
            return DragDropEffects.None;
        }

        public virtual void AcceptData(IDataObject data)
        {
        }

        public virtual DragDropEffects DoDragDrop(UIElement source, Point cursorPos)
        {
            return DragDropEffects.None;
        }

        private static void OnIsExpandedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ThemeTreeItem themeTreeItem = (ThemeTreeItem)sender;
            if (!(bool)args.NewValue || themeTreeItem.Parent == null)
                return;
            themeTreeItem.Parent.IsExpanded = true;
        }
    }
}

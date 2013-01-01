using Advent.Common.UI;
using Advent.MediaCenter.StartMenu;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
namespace Advent.VmcStudio.StartMenu
{
    public partial class MenuStrip : UserControl, IComponentConnector, IStyleConnector
    {
        private static readonly DependencyPropertyKey DefaultLinkPropertyKey = DependencyProperty.RegisterReadOnly("DefaultLink", typeof(IQuickLink), typeof(MenuStrip), new PropertyMetadata(null));
        public static readonly DependencyProperty DefaultLinkProperty = MenuStrip.DefaultLinkPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey TitleColorPropertyKey = DependencyProperty.RegisterReadOnly("TitleColor", typeof(Color), typeof(MenuStrip), new PropertyMetadata(null));
        public static readonly DependencyProperty TitleColorProperty = MenuStrip.TitleColorPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey CanMoveUpPropertyKey = DependencyProperty.RegisterReadOnly("CanMoveUp", typeof(bool), typeof(MenuStrip), new PropertyMetadata(false));
        public static readonly DependencyProperty CanMoveUpProperty = MenuStrip.CanMoveUpPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey CanMoveDownPropertyKey = DependencyProperty.RegisterReadOnly("CanMoveDown", typeof(bool), typeof(MenuStrip), new PropertyMetadata(false));
        public static readonly DependencyProperty CanMoveDownProperty = MenuStrip.CanMoveDownPropertyKey.DependencyProperty;
        public static readonly DependencyProperty StartMenuProperty = DependencyProperty.Register("StartMenu", typeof(StartMenu), typeof(MenuStrip), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MenuStrip strip = (MenuStrip)sender;
            DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(StartMenu.DefaultStripProperty, typeof(StartMenu));
            if (args.OldValue != null)
            {
                dependencyPropertyDescriptor.RemoveValueChanged(args.OldValue, new EventHandler(strip.DefaultStripPropertyChanged));
                ((StartMenu)args.OldValue).StartMenuManager.Strips.CollectionChanged -= new NotifyCollectionChangedEventHandler(strip.Strips_CollectionChanged);
            }
            if (args.NewValue != null)
            {
                dependencyPropertyDescriptor.AddValueChanged(args.NewValue, new EventHandler(strip.DefaultStripPropertyChanged));
                strip.UpdateIsDefault();
                ((StartMenu)args.NewValue).StartMenuManager.Strips.CollectionChanged += new NotifyCollectionChangedEventHandler(strip.Strips_CollectionChanged);
                strip.CheckIfCanMove();
                strip.CheckTitleColor();
                VmcStudioUtil.Application.CommonResources.PropertyChanged += delegate(object o, PropertyChangedEventArgs oargs)
                {
                    if (oargs.PropertyName == "TextColor" || oargs.PropertyName == "HighlightColor")
                    {
                        strip.CheckTitleColor();
                    }
                };
            }
        }));
        private static readonly DependencyPropertyKey IsDefaultStripPropertyKey = DependencyProperty.RegisterReadOnly("IsDefaultStrip", typeof(bool), typeof(MenuStrip), new PropertyMetadata(false));
        public static readonly DependencyProperty IsDefaultStripProperty = MenuStrip.IsDefaultStripPropertyKey.DependencyProperty;
        public static readonly DependencyProperty StripProperty = DependencyProperty.Register("Strip", typeof(IMenuStrip), typeof(MenuStrip), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MenuStrip menuStrip = (MenuStrip)sender;
            if (args.OldValue != null)
            {
                ((IMenuStrip)args.OldValue).QuickLinks.CollectionChanged -= new NotifyCollectionChangedEventHandler(menuStrip.QuickLinks_CollectionChanged);
            }
            menuStrip.SetValue(MenuStrip.DefaultLinkPropertyKey, null);
            if (args.NewValue != null)
            {
                IMenuStrip menuStrip2 = (IMenuStrip)args.NewValue;
                menuStrip2.QuickLinks.CollectionChanged += new NotifyCollectionChangedEventHandler(menuStrip.QuickLinks_CollectionChanged);
                menuStrip.UpdateLinks();
            }
            menuStrip.UpdateIsDefault();
            menuStrip.CheckIfCanMove();
            menuStrip.CheckTitleColor();
        }));
        public static readonly DependencyProperty DropTargetVisibilityProperty = DependencyProperty.Register("DropTargetVisibility", typeof(Visibility), typeof(MenuStrip), new PropertyMetadata(Visibility.Hidden));
        private bool m_removeDragLink;
        private DispatcherTimer timer;
        /*
        internal MenuStrip m_strip;
        internal Button m_moveUpButton;
        internal CheckBox m_isEnabledCheck;
        internal ImageButton m_deleteButton;
        internal EditableTextBlock m_titleText;
        internal Button m_moveDownButton;
        internal ItemsControl m_linkList;
        private bool _contentLoaded;
         * */

        public bool CanMoveUp
        {
            get
            {
                return (bool)base.GetValue(MenuStrip.CanMoveUpProperty);
            }
        }
        public bool CanMoveDown
        {
            get
            {
                return (bool)base.GetValue(MenuStrip.CanMoveDownProperty);
            }
        }
        public Color TitleColor
        {
            get
            {
                return (Color)base.GetValue(MenuStrip.TitleColorProperty);
            }
        }
        public StartMenu StartMenu
        {
            get
            {
                return (StartMenu)base.GetValue(MenuStrip.StartMenuProperty);
            }
            set
            {
                base.SetValue(MenuStrip.StartMenuProperty, value);
            }
        }
        public ItemsControl QuickLinks
        {
            get
            {
                return this.m_linkList;
            }
        }
        public bool IsDefaultStrip
        {
            get
            {
                return (bool)base.GetValue(MenuStrip.IsDefaultStripProperty);
            }
        }
        public IMenuStrip Strip
        {
            get
            {
                return (IMenuStrip)base.GetValue(MenuStrip.StripProperty);
            }
            set
            {
                base.SetValue(MenuStrip.StripProperty, value);
            }
        }
        public IQuickLink DefaultLink
        {
            get
            {
                return (IQuickLink)base.GetValue(MenuStrip.DefaultLinkProperty);
            }
        }
        public Visibility DropTargetVisibility
        {
            get
            {
                return (Visibility)base.GetValue(MenuStrip.DropTargetVisibilityProperty);
            }
            set
            {
                base.SetValue(MenuStrip.DropTargetVisibilityProperty, value);
            }
        }
        public string Title
        {
            get
            {
                return this.Strip.Title;
            }
        }
        public MenuStrip()
        {
            this.InitializeComponent();
            Advent.Common.UI.DragDrop.EnhancedDragStarted += new EventHandler<EnhancedDragEventArgs>(this.dragHelper_DragStarted);
            Advent.Common.UI.DragDrop.EnhancedDragEnded += new EventHandler<EnhancedDragEventArgs>(this.dragHelper_DragEnded);
        }
        private void UpdateIsDefault()
        {
            base.SetValue(MenuStrip.IsDefaultStripPropertyKey, this.StartMenu.DefaultStrip == this.Strip);
        }
        private void DefaultStripPropertyChanged(object sender, EventArgs args)
        {
            this.UpdateIsDefault();
        }
        private void dragHelper_DragStarted(object sender, EnhancedDragEventArgs e)
        {
            QuickLinkDrag quickLinkDrag = VmcStudioUtil.DragDropObject as QuickLinkDrag;
            if (quickLinkDrag != null)
            {
                this.m_removeDragLink = this.Strip.QuickLinks.Contains(quickLinkDrag.Link);
                if (this.Strip.CanAddQuickLink(quickLinkDrag.Link))
                {
                    this.DropTargetVisibility = Visibility.Visible;
                }
            }
        }
        private void dragHelper_DragEnded(object sender, EnhancedDragEventArgs e)
        {
            this.DropTargetVisibility = Visibility.Hidden;
            if (e.Effects != DragDropEffects.None)
            {
                QuickLinkDrag quickLinkDrag = VmcStudioUtil.DragDropObject as QuickLinkDrag;
                if (this.m_removeDragLink && quickLinkDrag != null)
                {
                    this.Strip.QuickLinks.Remove(quickLinkDrag.Link);
                }
            }
            this.m_removeDragLink = false;
        }
        internal void UpdateLinks()
        {
            IQuickLink quickLink = null;
            foreach (IQuickLink current in this.Strip.QuickLinks)
            {
                if (current.IsValid && (quickLink == null || current.Priority < quickLink.Priority))
                {
                    quickLink = current;
                }
            }
            base.SetValue(MenuStrip.DefaultLinkPropertyKey, quickLink);
        }
        private void QuickLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateLinks();
        }
        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer && !e.Handled)
            {
                e.Handled = true;
                MouseWheelEventArgs mouseWheelEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                mouseWheelEventArgs.RoutedEvent = UIElement.MouseWheelEvent;
                mouseWheelEventArgs.Source = sender;
                UIElement uIElement = ((Control)sender).Parent as UIElement;
                uIElement.RaiseEvent(mouseWheelEventArgs);
            }
        }
        private void QuickLinkTargetImage_Drop(object sender, DragEventArgs e)
        {

            QuickLinkTargetImage quickLinkTargetImage = (QuickLinkTargetImage)sender;
            QuickLinkDrag quickLinkDrag = VmcStudioUtil.DragDropObject as QuickLinkDrag;
            if (quickLinkDrag != null)
            {
                int num;
                if (quickLinkTargetImage.Link != null)
                {
                    num = this.Strip.QuickLinks.IndexOf(quickLinkTargetImage.Link);
                }
                else
                {
                    num = this.Strip.QuickLinks.Count;
                }
                if (this.Strip.QuickLinks.Contains(quickLinkDrag.Link))
                {
                    int num2 = this.Strip.QuickLinks.IndexOf(quickLinkDrag.Link);
                    this.Strip.QuickLinks.Move(num2, (num2 < num) ? (num - 1) : num);
                    this.m_removeDragLink = false;
                }
                else
                {
                    this.Strip.QuickLinks.Insert(num, quickLinkDrag.Link);
                }
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }


        }
        private void m_moveUpButton_Click(object sender, RoutedEventArgs e)
        {
            StartMenuManager startMenuManager = this.StartMenu.StartMenuManager;
            if (startMenuManager != null)
            {
                int num = startMenuManager.Strips.IndexOf(this.Strip);
                if (this.CanMoveUp)
                {
                    startMenuManager.Strips.Move(num, num - 1);
                }
            }
        }
        private void m_moveDownButton_Click(object sender, RoutedEventArgs e)
        {
            StartMenuManager startMenuManager = this.StartMenu.StartMenuManager;
            if (startMenuManager != null)
            {
                int num = startMenuManager.Strips.IndexOf(this.Strip);
                if (this.CanMoveDown)
                {
                    startMenuManager.Strips.Move(num, num + 1);
                }
            }
        }
        private void m_strip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = base.CaptureMouse();
            }
        }
        private void m_strip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled && this.Strip.IsEnabled)
            {
                if (this.Strip.CanSetPriority)
                {
                    StartMenuManager startMenuManager = this.StartMenu.StartMenuManager;
                    int num = 2;
                    foreach (IMenuStrip current in startMenuManager.Strips)
                    {
                        if (current.CanSetPriority)
                        {
                            if (current == this.Strip)
                            {
                                current.Priority = 1;
                            }
                            else
                            {
                                current.Priority = num;
                                num++;
                            }
                        }
                    }
                    this.StartMenu.UpdateStrips();
                }
                e.Handled = true;
            }
            base.ReleaseMouseCapture();
        }
        private void Strips_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CheckIfCanMove();
        }
        private void CheckIfCanMove()
        {
            if (this.Strip != null && this.StartMenu != null && this.StartMenu.StartMenuManager != null)
            {
                ObservableCollection<IMenuStrip> strips = this.StartMenu.StartMenuManager.Strips;
                int num = strips.IndexOf(this.Strip);
                bool flag = num > 0 && this.Strip.CanSwapWith(strips[num - 1]) && strips[num - 1].CanSwapWith(this.Strip);
                base.SetValue(MenuStrip.CanMoveUpPropertyKey, flag);
                bool flag2 = num < strips.Count - 1 && this.Strip.CanSwapWith(strips[num + 1]) && strips[num + 1].CanSwapWith(this.Strip);
                base.SetValue(MenuStrip.CanMoveDownPropertyKey, flag2);
            }
        }
        private void CheckTitleColor()
        {
            if (this.Strip != null && this.StartMenu != null)
            {
                if (this.Strip.CanSetPriority)
                {
                    base.SetValue(MenuStrip.TitleColorPropertyKey, VmcStudioUtil.Application.CommonResources.TextColor);
                    return;
                }
                base.SetValue(MenuStrip.TitleColorPropertyKey, VmcStudioUtil.Application.CommonResources.HighlightColor);
            }
        }
        private void RepeatButtonDragEnter(object sender, DragEventArgs e)
        {
            RepeatButton button = (RepeatButton)sender;
            this.timer = new DispatcherTimer(DispatcherPriority.Background, base.Dispatcher);
            this.timer.Interval = TimeSpan.FromMilliseconds((double)button.Interval);
            this.timer.Tick += delegate
            {
                RoutedCommand routedCommand = (RoutedCommand)button.Command;
                routedCommand.Execute(button.CommandParameter, button);
            };
            this.timer.Start();
        }
        private void RepeatButtonDragLeave(object sender, DragEventArgs e)
        {
            this.timer.Stop();
        }
        private void m_deleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.StartMenu.StartMenuManager.Strips.Remove(this.Strip);
        }
        
        
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 8:
                    ((RepeatButton)target).DragEnter += new DragEventHandler(this.RepeatButtonDragEnter);
                    ((RepeatButton)target).DragLeave += new DragEventHandler(this.RepeatButtonDragLeave);
                    ((RepeatButton)target).Drop += new DragEventHandler(this.RepeatButtonDragLeave);
                    return;
                case 9:
                    ((RepeatButton)target).DragEnter += new DragEventHandler(this.RepeatButtonDragEnter);
                    ((RepeatButton)target).DragLeave += new DragEventHandler(this.RepeatButtonDragLeave);
                    ((RepeatButton)target).Drop += new DragEventHandler(this.RepeatButtonDragLeave);
                    return;
                default:
                    return;
            }
        }
    }
}

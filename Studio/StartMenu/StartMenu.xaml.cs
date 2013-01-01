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
using System.Windows.Threading;

namespace Advent.VmcStudio.StartMenu
{
    public partial class StartMenu : UserControl, IComponentConnector, IStyleConnector
    {
        // Fields
        //private bool _contentLoaded;
        
        private static readonly DependencyPropertyKey DefaultStripPropertyKey = DependencyProperty.RegisterReadOnly("DefaultStrip", typeof(IMenuStrip), typeof(Advent.VmcStudio.StartMenu.StartMenu), new PropertyMetadata(null));
        public static readonly DependencyProperty DefaultStripProperty = DefaultStripPropertyKey.DependencyProperty;
        //internal Advent.VmcStudio.StartMenu.StartMenu m_startMenu;
        //internal ItemsControl m_strips;
        //internal ScrollViewer m_stripsScroll;
        public static readonly DependencyProperty StartMenuManagerProperty;
        private DispatcherTimer timer;

        // Methods
        static StartMenu()
        {
            StartMenuManagerProperty = DependencyProperty.Register("StartMenuManager", typeof(StartMenuManager), typeof(Advent.VmcStudio.StartMenu.StartMenu), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
            {
                Advent.VmcStudio.StartMenu.StartMenu menu = (Advent.VmcStudio.StartMenu.StartMenu)sender;
                if (args.OldValue != null)
                {
                    StartMenuManager newValue = (StartMenuManager)args.NewValue;
                    newValue.Strips.CollectionChanged -= new NotifyCollectionChangedEventHandler(menu.Strips_CollectionChanged);
                }
                if (args.NewValue != null)
                {
                    StartMenuManager manager2 = (StartMenuManager)args.NewValue;
                    manager2.Strips.CollectionChanged += new NotifyCollectionChangedEventHandler(menu.Strips_CollectionChanged);
                }
                menu.UpdateStrips();
            }));
        }

        public StartMenu()
        {
            this.InitializeComponent();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
        }

        private void RepeatButtonDragEnter(object sender, DragEventArgs e)
    {
        RepeatButton button = (RepeatButton) sender;
        this.timer = new DispatcherTimer(DispatcherPriority.Background, base.Dispatcher);
        this.timer.Interval = TimeSpan.FromMilliseconds((double) button.Interval);
        this.timer.Tick += (EventHandler)delegate
        {
            ((RoutedCommand)button.Command).Execute(button.CommandParameter, (IInputElement)button);
        };
        this.timer.Start();
    }

        private void RepeatButtonDragLeave(object sender, DragEventArgs e)
        {
            this.timer.Stop();
        }

        public void ScrollToEnd()
        {
            this.m_stripsScroll.ScrollToEnd();
        }

        public void ScrollToTop()
        {
            this.m_stripsScroll.ScrollToTop();
        }

        private void Strips_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateStrips();
        }

        internal void UpdateStrips()
        {
            IMenuStrip strip = null;
            foreach (IMenuStrip strip2 in this.StartMenuManager.Strips)
            {
                if (strip2.IsEnabled && ((strip == null) || (strip2.Priority < strip.Priority)))
                {
                    strip = strip2;
                }
            }
            base.SetValue(DefaultStripPropertyKey, strip);
        }

        // Properties
        public IMenuStrip DefaultStrip
        {
            get
            {
                return (IMenuStrip)base.GetValue(DefaultStripProperty);
            }
        }

        public StartMenuManager StartMenuManager
        {
            get
            {
                return (StartMenuManager)base.GetValue(StartMenuManagerProperty);
            }
            set
            {
                base.SetValue(StartMenuManagerProperty, value);
            }
        }

        public ItemsControl Strips
        {
            get
            {
                return this.m_strips;
            }
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerNonUserCode]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 3:
                    ((UIElement)target).DragEnter += new DragEventHandler(this.RepeatButtonDragEnter);
                    ((UIElement)target).DragLeave += new DragEventHandler(this.RepeatButtonDragLeave);
                    ((UIElement)target).Drop += new DragEventHandler(this.RepeatButtonDragLeave);
                    break;
                case 4:
                    ((UIElement)target).DragEnter += new DragEventHandler(this.RepeatButtonDragEnter);
                    ((UIElement)target).DragLeave += new DragEventHandler(this.RepeatButtonDragLeave);
                    ((UIElement)target).Drop += new DragEventHandler(this.RepeatButtonDragLeave);
                    break;
            }
        }
    }


}

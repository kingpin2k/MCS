using Advent.Common.Interop;
using Advent.Common.UI;
using Advent.MediaCenter.StartMenu;
using Advent.VmcStudio;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Advent.VmcStudio.StartMenu
{
    public partial class QuickLink : UserControl, IComponentConnector
    {
        // Fields
        //private bool _contentLoaded;
        private const double ADORNER_OPACITY_LINK = 0.5;
        private const double ADORNER_OPACITY_STRIP = 0.1;
        public static readonly DependencyProperty IsDefaultLinkProperty;
        private static readonly DependencyPropertyKey IsDefaultLinkPropertyKey;
        public static readonly DependencyProperty LinkProperty;
        //internal Image m_backgroundImage;
        private Adorner m_currentAdorner;
        //internal ImageButton m_deleteButton;
        //internal Image m_focusImage;
        //internal CheckBox m_isEnabledCheck;
        private Adorner m_lastAdorner;
        //internal Image m_linkImage;
        //internal QuickLink m_quickLink;
        //internal EditableTextBlock m_titleText;
        public static readonly DependencyProperty NonFocusImageProperty;
        public static readonly DependencyProperty NormalisedNonFocusImageProperty;
        public static readonly DependencyProperty StripProperty;

        // Methods
        static QuickLink()
        {
            LinkProperty = DependencyProperty.Register("Link", typeof(IQuickLink), typeof(QuickLink), new PropertyMetadata((sender, args) => ((QuickLink)sender).UpdateIsDefault()));
            IsDefaultLinkPropertyKey = DependencyProperty.RegisterReadOnly("IsDefaultLink", typeof(bool), typeof(QuickLink), new PropertyMetadata(false));
            IsDefaultLinkProperty = IsDefaultLinkPropertyKey.DependencyProperty;
            StripProperty = DependencyProperty.Register("Strip", typeof(MenuStrip), typeof(QuickLink), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
            {
                QuickLink link = (QuickLink)sender;
                DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(MenuStrip.DefaultLinkProperty, typeof(MenuStrip));
                if (args.OldValue != null)
                {
                    descriptor.RemoveValueChanged(args.OldValue, new EventHandler(link.DefaultLinkPropertyChanged));
                    MenuStrip oldValue = (MenuStrip)args.OldValue;
                    oldValue.MouseEnter -= new MouseEventHandler(link.Strip_MouseEnter);
                    oldValue.MouseLeave -= new MouseEventHandler(link.Strip_MouseLeave);
                }
                if (args.NewValue != null)
                {
                    descriptor.AddValueChanged(args.NewValue, new EventHandler(link.DefaultLinkPropertyChanged));
                    link.UpdateIsDefault();
                    MenuStrip newValue = (MenuStrip)args.NewValue;
                    newValue.MouseEnter += new MouseEventHandler(link.Strip_MouseEnter);
                    newValue.MouseLeave += new MouseEventHandler(link.Strip_MouseLeave);
                }
            }));
            NormalisedNonFocusImageProperty = DependencyProperty.Register("NormalisedNonFocusImage", typeof(ImageSource), typeof(QuickLink));
            NonFocusImageProperty = DependencyProperty.Register("NonFocusImage", typeof(ImageSource), typeof(QuickLink), new PropertyMetadata(delegate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
            {
                QuickLink link = (QuickLink)sender;
                ImageSource imageSource = (ImageSource)args.NewValue;
                ImageSource image = link.Link.Image;
                if (((imageSource != null) && (image != null)) && ((imageSource.Width != image.Width) || (imageSource.Height != image.Height)))
                {
                    double x = (image.Width - imageSource.Width) / 2.0;
                    double y = (image.Height - imageSource.Height) / 2.0;
                    DrawingVisual visual = new DrawingVisual();
                    using (DrawingContext context = visual.RenderOpen())
                    {
                        context.DrawImage(imageSource, new Rect(x, y, imageSource.Width, imageSource.Height));
                    }
                    RenderTargetBitmap bitmap = new RenderTargetBitmap((int)image.Width, (int)image.Height, 96.0, 96.0, PixelFormats.Default);
                    bitmap.Render(visual);
                    link.SetValue(NormalisedNonFocusImageProperty, bitmap);
                }
                else
                {
                    link.SetValue(NormalisedNonFocusImageProperty, imageSource);
                }
            }));
        }

        public QuickLink()
        {
            this.InitializeComponent();
            Binding binding = new Binding("Link.NonFocusImage")
            {
                Source = this
            };
            BindingOperations.SetBinding(this, NonFocusImageProperty, binding);
            base.MouseEnter += new MouseEventHandler(this.QuickLink_MouseEnter);
            base.MouseLeave += new MouseEventHandler(this.QuickLink_MouseLeave);
        }


        private void anim_Completed(object sender, EventArgs e)
        {
            AdornerLayer.GetAdornerLayer(this.Strip.StartMenu).Remove(this.m_lastAdorner);
        }

        private void DefaultLinkPropertyChanged(object sender, EventArgs args)
        {
            this.UpdateIsDefault();
        }

        private MenuStrip GetOriginalStrip()
        {
            IMenuStrip originalStrip = this.Link.OriginalStrip;
            if (originalStrip != null)
            {
                return (VisualTreeHelper.GetChild(this.Strip.StartMenu.Strips.ItemContainerGenerator.ContainerFromItem(originalStrip), 0) as MenuStrip);
            }
            return null;
        }

        private void HideOriginalStrip()
        {
            if (this.m_currentAdorner != null)
            {
                this.m_lastAdorner = this.m_currentAdorner;
                DoubleAnimation animation = this.MakeAnimation(0.0);
                animation.Completed += new EventHandler(this.anim_Completed);
                this.m_currentAdorner.BeginAnimation(UIElement.OpacityProperty, animation);
            }
        }

        private void m_deleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Strip.Strip.QuickLinks.Remove(this.Link);
        }

        private DoubleAnimation MakeAnimation(double toValue)
        {
            return new DoubleAnimation(toValue, new Duration(TimeSpan.FromSeconds(0.2))) { AccelerationRatio = 0.2 };
        }

        private void OnDrag(object sender, BeginDragEventArgs e)
        {
            IDataObject dataObject = Advent.Common.Interop.DataObject.CreateDataObject();
            VmcStudioUtil.DragDropObject = new QuickLinkDrag(this.Link);
            try
            {
                dataObject.DoDragDrop(this, e.DragPoint, DragDropEffects.Move);
            }
            finally
            {
                VmcStudioUtil.DragDropObject = null;
            }
        }

        private void QuickLink_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.m_currentAdorner != null)
            {
                this.m_currentAdorner.BeginAnimation(UIElement.OpacityProperty, this.MakeAnimation(0.5));
            }
        }

        private void QuickLink_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.m_currentAdorner != null)
            {
                this.m_currentAdorner.BeginAnimation(UIElement.OpacityProperty, this.MakeAnimation(0.1));
            }
        }

        private void QuickLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Strip.Strip.CanSetLinkPriority)
            {
                int num = 1;
                foreach (IQuickLink link in this.Strip.Strip.QuickLinks)
                {
                    if (link == this.Link)
                    {
                        link.Priority = 0;
                    }
                    else
                    {
                        link.Priority = num;
                        num++;
                    }
                }
                this.Strip.UpdateLinks();
            }
            e.Handled = true;
        }

        private void ShowOriginalStrip()
        {
            MenuStrip originalStrip = this.GetOriginalStrip();
            if ((originalStrip != null) && (originalStrip != this.Strip))
            {
                Point point = originalStrip.TranslatePoint(new Point(0.0, 0.0), this.Strip);
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.Strip.StartMenu);
                this.m_currentAdorner = new ArrowAdorner(this, (int)((Math.Abs(point.Y) - originalStrip.DesiredSize.Height) + (originalStrip.DesiredSize.Height / 2.0)), new SolidColorBrush(VmcStudioUtil.Application.CommonResources.HighlightColor));
                this.m_currentAdorner.Opacity = 0.0;
                adornerLayer.Add(this.m_currentAdorner);
                DoubleAnimation animation = this.MakeAnimation(base.IsMouseOver ? 0.5 : 0.1);
                this.m_currentAdorner.BeginAnimation(UIElement.OpacityProperty, animation);
            }
        }

        private void Strip_MouseEnter(object sender, MouseEventArgs e)
        {
            this.ShowOriginalStrip();
        }

        private void Strip_MouseLeave(object sender, MouseEventArgs e)
        {
            this.HideOriginalStrip();
        }

        private void UpdateIsDefault()
        {
            base.SetValue(IsDefaultLinkPropertyKey, this.Strip.DefaultLink == this.Link);
        }

        // Properties
        public IQuickLink Link
        {
            get
            {
                return (IQuickLink)base.GetValue(LinkProperty);
            }
            set
            {
                base.SetValue(LinkProperty, value);
            }
        }

        public ImageSource NormalisedNonFocusImage
        {
            get
            {
                return (ImageSource)base.GetValue(NormalisedNonFocusImageProperty);
            }
        }

        public MenuStrip Strip
        {
            get
            {
                return (MenuStrip)base.GetValue(StripProperty);
            }
            set
            {
                base.SetValue(StripProperty, value);
            }
        }
    }


}

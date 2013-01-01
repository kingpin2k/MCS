using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Advent.VmcStudio
{
    public partial class ImageButton : UserControl, IComponentConnector
    {
        // Fields
        public static readonly DependencyProperty HighlightBackgroundProperty = DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(ImageButton));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton));
        
        // Events
        public event RoutedEventHandler Click
        {
            add
            {
                this.m_button.AddHandler(ButtonBase.ClickEvent, value);
            }
            remove
            {
                this.m_button.RemoveHandler(ButtonBase.ClickEvent, value);
            }
        }

        // Methods
        public ImageButton()
        {
            this.InitializeComponent();
        }

        // Properties
        public ICommand Command
        {
            get
            {
                return this.m_button.Command;
            }
            set
            {
                this.m_button.Command = value;
            }
        }

        public object CommandParameter
        {
            get
            {
                return this.m_button.CommandParameter;
            }
            set
            {
                this.m_button.CommandParameter = value;
            }
        }

        public Brush HighlightBackground
        {
            get
            {
                return (Brush)base.GetValue(HighlightBackgroundProperty);
            }
            set
            {
                base.SetValue(HighlightBackgroundProperty, value);
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)base.GetValue(ImageSourceProperty);
            }
            set
            {
                base.SetValue(ImageSourceProperty, value);
            }
        }
    }


}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Advent.VmcStudio
{
    public partial class EditableTextBlock : UserControl, IComponentConnector
    {
        // Fields
        public static readonly DependencyProperty ButtonOpacityProperty = DependencyProperty.Register("ButtonOpacity", typeof(double), typeof(EditableTextBlock), new PropertyMetadata(1.0));
        public static readonly DependencyProperty IsEditEnabledProperty = DependencyProperty.Register("IsEditEnabled", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(true));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock));

        // Methods
        public EditableTextBlock()
        {
            this.InitializeComponent();
        }

        private void HideTextBox()
        {
            this.m_textBox.Visibility = Visibility.Collapsed;
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            this.m_textBox.Visibility = Visibility.Visible;
            this.m_textBox.Focus();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.HideTextBox();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.HideTextBox();
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.HideTextBox();
        }

        // Properties
        public double ButtonOpacity
        {
            get
            {
                return (double)base.GetValue(ButtonOpacityProperty);
            }
            set
            {
                base.SetValue(ButtonOpacityProperty, value);
            }
        }

        public ImageButton EditButton
        {
            get
            {
                return null;// this.m_editButton;
            }
        }

        public bool IsEditEnabled
        {
            get
            {
                return (bool)base.GetValue(IsEditEnabledProperty);
            }
            set
            {
                base.SetValue(IsEditEnabledProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)base.GetValue(TextProperty);
            }
            set
            {
                base.SetValue(TextProperty, value);
            }
        }
    }


}

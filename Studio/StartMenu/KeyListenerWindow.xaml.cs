using Advent.Common.UI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Advent.VmcStudio.StartMenu
{
    public partial class KeyListenerWindow : Window, IComponentConnector
    {
        // Fields
        /*
        private bool _contentLoaded;
        internal Grid m_grid;
        internal TextBlock m_text;
         * */

        private GlobalHook hook;

        // Methods
        public KeyListenerWindow()
        {
            this.InitializeComponent();
            this.Key = Keys.None;
            int captionHeight = SystemInformation.CaptionHeight;
            this.m_grid.Margin = new Thickness((double)captionHeight, 0.0, (double)captionHeight, (double)captionHeight);
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            this.Key = e.KeyData;
            e.Handled = true;
            base.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.hook.Stop(false, true, false);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.hook = new GlobalHook(false, true);
            this.hook.KeyDown += new KeyEventHandler(this.hook_KeyDown);
        }

        // Properties
        public Keys Key { get; private set; }

        public string Message
        {
            get
            {
                return this.m_text.Text;
            }
            set
            {
                this.m_text.Text = value;
            }
        }
    }



}

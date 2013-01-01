using Advent.MediaCenter.Theme;
using Advent.VmcStudio.Theme.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
namespace Advent.VmcStudio.Theme.View
{
    public partial class SoundResourceThemeItemView : UserControl, IComponentConnector
    {
        
        internal SoundResourceModel SoundResource
        {
            get
            {
                return (SoundResourceModel)base.DataContext;
            }
        }
        internal SoundResourceThemeItem SoundResourceItem
        {
            get
            {
                return (SoundResourceThemeItem)this.SoundResource.ThemeItem;
            }
        }
        public SoundResourceThemeItemView()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SoundResourceThemeItem soundResourceItem = this.SoundResourceItem;
            soundResourceItem.Load();
            soundResourceItem.Sound.Play();
        }

    }
}

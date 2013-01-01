using Advent.Common.Interop;
using Advent.Common.UI;
using Advent.MediaCenter.StartMenu;
using Advent.MediaCenter.StartMenu.OEM;
using Advent.VmcStudio;
using Advent.VmcStudio.StartMenu.Views;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Advent.VmcStudio.StartMenu
{
    public partial class EntryPoint : UserControl, IComponentConnector
    {
        // Fields
        //private bool _contentLoaded;
        //internal Image m_image;
        public static readonly DependencyProperty OemEntryPointProperty = DependencyProperty.Register("OemEntryPoint", typeof(Advent.MediaCenter.StartMenu.OEM.EntryPoint), typeof(Advent.MediaCenter.StartMenu.OEM.EntryPoint));

        // Methods
        public EntryPoint()
        {
            this.InitializeComponent();
        }


        private void OnDrag(object sender, BeginDragEventArgs e)
        {
            OemQuickLink link = new OemQuickLink(this.OemEntryPoint.Manager);
            link.BeginInit();
            link.Application = this.OemEntryPoint.Application;
            link.EntryPoint = this.OemEntryPoint;
            link.EndInit();
            IDataObject dataObject = Advent.Common.Interop.DataObject.CreateDataObject();
            VmcStudioUtil.DragDropObject = new QuickLinkDrag(link);
            try
            {
                dataObject.DoDragDrop(this, e.DragPoint, DragDropEffects.Move);
            }
            finally
            {
                VmcStudioUtil.DragDropObject = null;
            }
        }

       
        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EntryPointDocumentView.OpenDocument(this.OemEntryPoint);
        }

        // Properties
        public Advent.MediaCenter.StartMenu.OEM.EntryPoint OemEntryPoint
        {
            get
            {
                return (Advent.MediaCenter.StartMenu.OEM.EntryPoint)base.GetValue(OemEntryPointProperty);
            }
            set
            {
                base.SetValue(OemEntryPointProperty, value);
            }
        }
    }

 



}

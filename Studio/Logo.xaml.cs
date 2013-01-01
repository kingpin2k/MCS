using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace Advent.VmcStudio
{
    public partial class Logo : UserControl, IComponentConnector
    {
        /*
        internal Ellipse GreenOuter;
        internal Ellipse GreenInner;
        internal Ellipse RedOuter;
        internal Ellipse RedInner;
        internal Ellipse BlueOuter;
        internal Ellipse BlueInner;
        private bool _contentLoaded;
        */

        public Logo()
        {
            this.InitializeComponent();
        }
    }
}

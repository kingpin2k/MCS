using Advent.MediaCenter.StartMenu;
using System.Windows;
using System.Windows.Controls;

namespace Advent.VmcStudio.StartMenu
{
    public class QuickLinkTargetImage : Image
    {
        public static readonly DependencyProperty LinkProperty = DependencyProperty.Register("Link", typeof(IQuickLink), typeof(QuickLinkTargetImage));

        public IQuickLink Link
        {
            get
            {
                return (IQuickLink)this.GetValue(QuickLinkTargetImage.LinkProperty);
            }
            set
            {
                this.SetValue(QuickLinkTargetImage.LinkProperty, (object)value);
            }
        }

        static QuickLinkTargetImage()
        {
        }

        public QuickLinkTargetImage()
        {
            this.PreviewDragEnter += new DragEventHandler(this.QuickLinkTargetImage_PreviewDragEnter);
            this.PreviewDragLeave += new DragEventHandler(this.QuickLinkTargetImage_PreviewDragLeave);
            this.PreviewDrop += new DragEventHandler(this.QuickLinkTargetImage_PreviewDrop);
        }

        private void QuickLinkTargetImage_PreviewDrop(object sender, DragEventArgs e)
        {
        }

        private void QuickLinkTargetImage_PreviewDragEnter(object sender, DragEventArgs e)
        {
        }

        private void QuickLinkTargetImage_PreviewDragLeave(object sender, DragEventArgs e)
        {
        }
    }
}

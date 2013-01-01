


using System.ComponentModel;
using System.Windows.Media;

namespace Advent.MediaCenter.StartMenu
{
    public interface IQuickLink : ISupportInitialize
    {
        string Title { get; set; }

        ImageSource Image { get; }

        ImageSource NonFocusImage { get; }

        int Priority { get; set; }

        bool IsValid { get; }

        bool IsEnabled { get; set; }

        bool CanSetEnabled { get; }

        bool CanDelete { get; }

        bool CanEditTitle { get; }

        IMenuStrip OriginalStrip { get; }
    }
}




using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Advent.MediaCenter.StartMenu
{
    public interface IMenuStrip : ISupportInitialize
    {
        ObservableCollection<IQuickLink> QuickLinks { get; }

        string Title { get; set; }

        int Priority { get; set; }

        bool IsEnabled { get; set; }

        bool CanSetLinkPriority { get; }

        bool CanSetPriority { get; }

        bool CanSetEnabled { get; }

        bool CanEditTitle { get; }

        bool CanDelete { get; }

        bool CanAddQuickLink(IQuickLink link);

        bool CanSwapWith(IMenuStrip strip);
    }
}

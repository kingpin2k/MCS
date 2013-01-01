


using Advent.MediaCenter.StartMenu.OEM;
using System.ComponentModel;

namespace Advent.MediaCenter.StartMenu
{
    public interface IPartnerQuickLink : IQuickLink, ISupportInitialize
    {
        OemQuickLink OemQuickLink { get; set; }
    }
}

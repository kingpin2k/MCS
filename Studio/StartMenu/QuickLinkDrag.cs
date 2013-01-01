using Advent.MediaCenter.StartMenu;

namespace Advent.VmcStudio.StartMenu
{
    public class QuickLinkDrag
    {
        private readonly IQuickLink link;

        public IQuickLink Link
        {
            get
            {
                return this.link;
            }
        }

        public QuickLinkDrag(IQuickLink link)
        {
            this.link = link;
        }
    }
}

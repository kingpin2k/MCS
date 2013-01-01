using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.VmcStudio;
using Microsoft.MediaCenter;
using System.Windows.Media;

namespace Advent.VmcStudio.StartMenu.Presenters
{
    public class MediaCenterPagePresenter
    {
        private string imageResource;
        private string nonFocusImageResource;

        public string Name { get; private set; }

        public PageId? Page { get; private set; }

        public MediaCenterPagePresenter(string name, PageId? page, string imageResourceUri, string nonFocusImageResourceUri)
        {
            this.Name = name;
            this.Page = page;
            this.imageResource = imageResourceUri;
            this.nonFocusImageResource = nonFocusImageResourceUri;
        }

        public ImageSource GetImage()
        {
            if (this.imageResource == null)
                return (ImageSource)null;
            else
                return MediaCenterUtil.GetImageResource((IResourceLibraryCache)VmcStudioUtil.Application.CommonResources.LibraryCache, this.imageResource);
        }

        public ImageSource GetNonFocusImage()
        {
            if (this.nonFocusImageResource == null)
                return (ImageSource)null;
            else
                return MediaCenterUtil.GetImageResource((IResourceLibraryCache)VmcStudioUtil.Application.CommonResources.LibraryCache, this.nonFocusImageResource);
        }
    }
}

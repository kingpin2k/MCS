using Advent.VmcExecute;
using Microsoft.MediaCenter;
using System.Windows.Media;

namespace Advent.VmcStudio.StartMenu.Presenters
{
    internal abstract class MediaInfoPresenter
    {
        public abstract string Name { get; }

        public abstract ImageSource Image { get; }

        public abstract string Path { get; }

        public MediaInfo MediaInfo { get; private set; }

        protected MediaInfoPresenter(MediaInfo media)
        {
            this.MediaInfo = media;
        }

        public static MediaInfoPresenter Create(MediaInfo media)
        {
            MediaType? mediaType = media.MediaType;
            if ((mediaType.HasValue ? (int)mediaType.GetValueOrDefault() : -1) == 4)
                return (MediaInfoPresenter)new DvdMediaInfoPresenter(media);
            else
                return (MediaInfoPresenter)new FileMediaInfoPresenter(media);
        }
    }
}

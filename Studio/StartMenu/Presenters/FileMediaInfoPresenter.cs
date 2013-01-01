using Advent.Common.Interop;
using Advent.VmcExecute;
using System.IO;
using System.Windows.Media;

namespace Advent.VmcStudio.StartMenu.Presenters
{
    internal class FileMediaInfoPresenter : MediaInfoPresenter
    {
        private ImageSource image;

        public override string Name
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(this.Path);
            }
        }

        public override ImageSource Image
        {
            get
            {
                if (this.image == null)
                    this.image = Shell.GenerateThumbnail(this.MediaInfo.Url);
                return this.image;
            }
        }

        public override string Path
        {
            get
            {
                return this.MediaInfo.Url;
            }
        }

        public FileMediaInfoPresenter(MediaInfo media)
            : base(media)
        {
        }
    }
}

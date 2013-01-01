using Advent.VmcExecute;
using System;
using System.Windows.Media;

namespace Advent.VmcStudio.StartMenu.Presenters
{
    internal class DvdMediaInfoPresenter : MediaInfoPresenter
    {
        private const string DvdScheme = "dvd://";

        public override string Name
        {
            get
            {
                return "Play DVD";
            }
        }

        public override ImageSource Image
        {
            get
            {
                return (ImageSource)null;
            }
        }

        public override string Path
        {
            get
            {
                if (this.MediaInfo.Url.StartsWith("dvd://", StringComparison.OrdinalIgnoreCase))
                    return string.Format("{0}:\\", (object)this.MediaInfo.Url.Substring("dvd://".Length, 1).ToUpper());
                else
                    return (string)null;
            }
        }

        public DvdMediaInfoPresenter(MediaInfo media)
            : base(media)
        {
        }
    }
}

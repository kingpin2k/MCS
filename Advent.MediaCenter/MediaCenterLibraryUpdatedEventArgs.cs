


using System;

namespace Advent.MediaCenter
{
    public class MediaCenterLibraryUpdatedEventArgs : EventArgs
    {
        public string File { get; private set; }

        public MediaCenterLibraryUpdatedEventArgs(string file)
        {
            this.File = file;
        }
    }
}

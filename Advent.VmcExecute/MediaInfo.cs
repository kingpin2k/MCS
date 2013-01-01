
using Advent.Common.UI;
using Microsoft.MediaCenter;
using System.ComponentModel;

namespace Advent.VmcExecute
{
    public class MediaInfo : NotifyPropertyChangedBase
    {
        private MediaType? mediaType;
        private string url;

        [DefaultValue(null)]
        public MediaType? MediaType
        {
            get
            {
                return this.mediaType;
            }
            set
            {
                MediaType? nullable1 = this.mediaType;
                MediaType? nullable2 = value;
                if ((nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? true : (nullable1.HasValue != nullable2.HasValue ? true : false)) == false)
                    return;
                this.mediaType = value;
                this.OnPropertyChanged("MediaType");
            }
        }

        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                if (!(this.url != value))
                    return;
                this.url = value;
                this.OnPropertyChanged("Url");
            }
        }
    }
}

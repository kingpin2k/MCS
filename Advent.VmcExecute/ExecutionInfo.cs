using Microsoft.MediaCenter;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Advent.VmcExecute
{
    [XmlRoot]
    public class ExecutionInfo
    {
        [DefaultValue(null)]
        public PageId? Page { get; set; }

        [DefaultValue(null)]
        public List<MediaInfo> Media { get; set; }

        [DefaultValue(null)]
        public List<Keys> CloseKeys { get; set; }

        [DefaultValue(null)]
        public List<Keys> KillKeys { get; set; }

        [XmlAttribute]
        [DefaultValue(true)]
        public bool MinimiseMediaCenter { get; set; }

        [DefaultValue(false)]
        [XmlAttribute]
        public bool RequiresDirectX { get; set; }

        public string FileName { get; set; }

        public string Arguments { get; set; }

        public ExecutionInfo()
        {
            this.MinimiseMediaCenter = true;
        }
    }
}

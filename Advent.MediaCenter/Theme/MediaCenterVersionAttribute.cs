


using System;
using System.Diagnostics;

namespace Advent.MediaCenter.Theme
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal class MediaCenterVersionAttribute : Attribute
    {
        public int MajorVersion { get; set; }

        public int MinorVersion { get; set; }

        public int BuildVersion { get; set; }

        public int BuildVersionMin { get; set; }

        public int BuildVersionMax { get; set; }

        public Type ApplicatorType { get; set; }

        public MediaCenterVersionAttribute(Type applicator)
        {
            this.ApplicatorType = applicator;
            this.MajorVersion = -1;
            this.MinorVersion = -1;
            this.BuildVersion = -1;
            this.BuildVersionMin = -1;
            this.BuildVersionMax = -1;
        }

        public bool AppliesToVersion(FileVersionInfo fileVersion)
        {
            if ((this.MajorVersion < 0 || this.MajorVersion == fileVersion.ProductMajorPart) && (this.MinorVersion < 0 || this.MinorVersion == fileVersion.ProductMinorPart) && (this.BuildVersion < 0 || this.BuildVersion == fileVersion.ProductBuildPart))
                return this.IsInRange(this.BuildVersionMin, this.BuildVersionMax, fileVersion.ProductBuildPart);
            else
                return false;
        }

        private bool IsInRange(int min, int max, int val)
        {
            if (min >= 0 && val < min)
                return false;
            if (max >= 0)
                return val <= max;
            else
                return true;
        }
    }
}

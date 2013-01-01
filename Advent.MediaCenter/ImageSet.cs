


using Advent.Common.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Xml;

namespace Advent.MediaCenter
{
    internal class ImageSet
    {
        private readonly IDictionary<string, ImageSource> images;
        private readonly IDictionary<string, string> imageSources;
        private readonly string defaultImageName;
        private readonly string setName;
        private readonly IResourceLibraryCache cache;

        public string Name
        {
            get
            {
                return this.setName;
            }
        }

        public string DefaultImageName
        {
            get
            {
                return this.defaultImageName;
            }
        }

        public ImageSource this[string id]
        {
            get
            {
                ImageSource imageResource;
                if (!this.images.TryGetValue(id, out imageResource))
                {
                    string uri;
                    if (this.imageSources.TryGetValue(id, out uri))
                    {
                        imageResource = MediaCenterUtil.GetImageResource(this.cache, uri);
                        this.images[id] = imageResource;
                        if (imageResource == null)
                            Trace.TraceWarning("Could not find image set resource. Image set: {0} Image ID: {1} Resource: {2}", (object)this.setName, (object)id, (object)uri);
                    }
                    else if (this.setName != "Radio.FM.Preset" && id != "Default")
                        Trace.TraceWarning("Could not find image set resource. Image set: {0} Image ID: {1}", (object)this.setName, (object)id);
                }
                return imageResource;
            }
        }

        internal ImageSet(IResourceLibraryCache cache, string setName, string defaultImageName, IDictionary<string, string> imageSources)
        {
            this.cache = cache;
            this.setName = setName;
            this.defaultImageName = defaultImageName;
            this.images = (IDictionary<string, ImageSource>)new Dictionary<string, ImageSource>();
            this.imageSources = imageSources;
        }

        internal static ImageSet FromXml(IResourceLibraryCache cache, XmlElement element)
        {
            string attribute1 = element.GetAttribute("Name");
            string attribute2 = element.GetAttribute("DefaultImageName");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (XmlElement xmlElement in element.GetElementsByTagName("Image"))
            {
                string attribute3 = xmlElement.GetAttribute("Name");
                string attribute4 = xmlElement.GetAttribute("Source");
                if (string.IsNullOrEmpty(attribute4))
                    attribute4 = xmlElement.GetAttribute("Image");
                if (attribute3 != null && attribute4 != null)
                    dictionary[attribute3] = attribute4;
            }
            return new ImageSet(cache, attribute1, attribute2, (IDictionary<string, string>)dictionary);
        }
    }
}




using Advent.Common.IO;
using Advent.MediaCenter;

namespace Advent.MediaCenter.Theme
{
    internal class ResourceThemeItemApplicator : IThemeItemApplicator
    {
        public void Apply(IThemeItem themeItem, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            ResourceThemeItem resourceThemeItem = (ResourceThemeItem)themeItem;
            byte[] data = resourceThemeItem.Save(false);
            if (data == null || data.Length <= 0)
                return;
            ResourceExtensions.Update(writeCache[resourceThemeItem.DllName].GetResource(resourceThemeItem.ResourceName, (object)resourceThemeItem.ResourceType), data);
        }
    }
}

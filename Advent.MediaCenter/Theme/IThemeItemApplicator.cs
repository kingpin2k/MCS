


using Advent.MediaCenter;

namespace Advent.MediaCenter.Theme
{
    internal interface IThemeItemApplicator
    {
        void Apply(IThemeItem themeItem, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache);
    }
}




using Advent.MediaCenter;
using Advent.MediaCenter.Mcml;
using Advent.MediaCenter.Theme;
using Advent.MediaCenter.Theme.Default;

namespace Advent.MediaCenter.Theme.TVPack
{
    internal class AnimationsApplicator : Default.AnimationsApplicator
    {
        protected override void DisableBackgroundAnimations(MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            McmlDocument mcml = MediaCenterUtil.GetMcml(readCache["Microsoft.MediaCenter.Shell.dll"].GetResource("PAGEBACKGROUND.MCML", (object)23));
            PropertiesElement themeUiProperties = McmlUtilities.GetThemeUIProperties(mcml, "PageBackground");
            PropertyElement themeProperty = McmlUtilities.GetThemeProperty(themeUiProperties, "StaticSource");
            McmlUtilities.GetThemeProperty(themeUiProperties, "AnimatedSource").Value = themeProperty.Value;
            MediaCenterUtil.UpdateMcml(writeCache["Microsoft.MediaCenter.Shell.dll"].GetResource("PAGEBACKGROUND.MCML", (object)23), mcml);
        }
    }
}

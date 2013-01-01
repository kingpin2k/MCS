


using Advent.MediaCenter;
using Advent.MediaCenter.Mcml;
using Advent.MediaCenter.Theme;

namespace Advent.MediaCenter.Theme.Default
{
    internal class AnimationsApplicator : IThemeItemApplicator
    {
        public void Apply(IThemeItem themeItem, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            if (!((AnimationsItem)themeItem).IsBackgroundAnimationDisabled)
                return;
            this.DisableBackgroundAnimations(readCache, writeCache);
        }

        protected virtual void DisableBackgroundAnimations(MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            McmlDocument mcml = MediaCenterUtil.GetMcml(readCache["Microsoft.MediaCenter.Shell.dll"].GetResource("PAGEBACKGROUND.MCML", (object)10));
            PropertiesElement themeUiProperties = McmlUtilities.GetThemeUIProperties(mcml, "PageBackground");
            PropertyElement themeProperty1 = McmlUtilities.GetThemeProperty(themeUiProperties, "StaticSource");
            McmlUtilities.GetThemeProperty(themeUiProperties, "AnimatedSource").Value = themeProperty1.Value;
            PropertyElement themeProperty2 = McmlUtilities.GetThemeProperty(themeUiProperties, "StaticImage");
            McmlUtilities.GetThemeProperty(themeUiProperties, "AnimatedImage").Value = themeProperty2.Value;
            MediaCenterUtil.UpdateMcml(writeCache["Microsoft.MediaCenter.Shell.dll"].GetResource("PAGEBACKGROUND.MCML", (object)10), mcml);
        }
    }
}

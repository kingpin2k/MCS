


using Advent.MediaCenter;
using Advent.MediaCenter.Mcml;
using Advent.MediaCenter.Theme;

namespace Advent.MediaCenter.Theme.Default
{
    internal class StartMenuApplicator : IThemeItemApplicator
    {
        protected virtual int DocumentResourceType
        {
            get
            {
                return 23;
            }
        }

        public void Apply(IThemeItem themeItem, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            StartMenuThemeItem startMenuThemeItem = (StartMenuThemeItem)themeItem;
            this.UpdateStartMenuCategory(startMenuThemeItem, readCache, writeCache);
            this.UpdateQuickLinks(startMenuThemeItem, readCache, writeCache);
            this.UpdateStartMenuOverlay(startMenuThemeItem, readCache, writeCache);
        }

        protected virtual void UpdateQuickLinks(StartMenuThemeItem item, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            McmlDocument mcml = MediaCenterUtil.GetMcml(readCache["Microsoft.MediaCenter.Shell.dll"].GetResource("STARTMENUQUICKLINK.MCML", (object)this.DocumentResourceType));
            PropertiesElement themeUiProperties = McmlUtilities.GetThemeUIProperties(mcml, "BaseQuickLinkContent");
            McmlUtilities.UpdateFontElement(item.NonFocusedQuickLinkFont, themeUiProperties.GetProperty("Font") as FontElement, item.Theme);
            McmlUtilities.UpdateFontElement(item.FocusedQuickLinkFont, themeUiProperties.GetProperty("FocusFont") as FontElement, item.Theme);
            McmlUtilities.UpdateColorElement(item.FocusedQuickLinkColor, themeUiProperties.GetProperty("FocusColor"), item.Theme);
            McmlUtilities.UpdateColorElement(item.NonFocusedQuickLinkColor, themeUiProperties.GetProperty("NoFocusColor"), item.Theme);
            MediaCenterUtil.UpdateMcml(writeCache["Microsoft.MediaCenter.Shell.dll"].GetResource("STARTMENUQUICKLINK.MCML", (object)this.DocumentResourceType), mcml);
        }

        protected virtual void UpdateStartMenuCategory(StartMenuThemeItem item, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            McmlDocument mcml = MediaCenterUtil.GetMcml(readCache["Microsoft.MediaCenter.Shell.dll"].GetResource("STARTMENUCATEGORY.MCML", (object)this.DocumentResourceType));
            PropertiesElement themeUiProperties = McmlUtilities.GetThemeUIProperties(mcml, "StartMenuCategoryView");
            McmlUtilities.UpdateFontElement(item.StripTitleFont, themeUiProperties.GetProperty("Font") as FontElement, item.Theme);
            McmlUtilities.UpdateColorElement(item.FocusedStripTitleColor, themeUiProperties.GetProperty("FocusColor"), item.Theme);
            McmlUtilities.UpdateColorElement(item.NonFocusedStripTitleColor, themeUiProperties.GetProperty("NoFocusColor"), item.Theme);
            MediaCenterUtil.UpdateMcml(writeCache["Microsoft.MediaCenter.Shell.dll"].GetResource("STARTMENUCATEGORY.MCML", (object)this.DocumentResourceType), mcml);
        }

        protected virtual void UpdateStartMenuOverlay(StartMenuThemeItem item, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            McmlDocument mcml = MediaCenterUtil.GetMcml(readCache["Microsoft.MediaCenter.Shell.dll"].GetResource("STARTMENUOVERLAY.MCML", (object)this.DocumentResourceType));
            PropertiesElement themeUiProperties = McmlUtilities.GetThemeUIProperties(mcml, "StartMenuOverlay");
            if (!string.IsNullOrEmpty(item.StartMenuText))
                themeUiProperties.GetProperty("StartText").Value = item.StartMenuText;
            McmlUtilities.UpdateFontElement(item.StartMenuTextFont, themeUiProperties.GetProperty("StartTextFont") as FontElement, item.Theme);
            McmlUtilities.UpdateColorElement(item.StartMenuTextColor, themeUiProperties.GetProperty("StartTextColor"), item.Theme);
            MediaCenterUtil.UpdateMcml(writeCache["Microsoft.MediaCenter.Shell.dll"].GetResource("STARTMENUOVERLAY.MCML", (object)this.DocumentResourceType), mcml);
        }
    }
}

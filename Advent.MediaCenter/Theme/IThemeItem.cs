


using Advent.MediaCenter;
using System;
using System.ComponentModel;

namespace Advent.MediaCenter.Theme
{
    public interface IThemeItem : ISupportInitialize
    {
        bool IsDirty { get; }

        bool IsLoaded { get; }

        string Name { get; }

        MediaCenterTheme Theme { get; set; }

        event EventHandler IsDirtyChanged;

        void Load();

        void Apply(MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache);

        void ClearDirty();
    }
}

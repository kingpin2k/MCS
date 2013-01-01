using Advent.MediaCenter.StartMenu;
using Advent.VmcStudio;
using System.ComponentModel;

namespace Advent.VmcStudio.StartMenu
{
    internal class StartMenuDocument : VmcDocument
    {
        private VmcStudioApp app;

        public StartMenuManager StartMenuManager
        {
            get
            {
                return this.app.StartMenuManager;
            }
        }

        public StartMenuDocument(VmcStudioApp app)
        {
            this.app = app;
            this.Name = "Start Menu";
            this.IsDirty = this.app.StartMenuManager.IsDirty;
            this.app.StartMenuManager.PropertyChanged += new PropertyChangedEventHandler(this.StartMenuManager_PropertyChanged);
        }

        private void StartMenuManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsDirty = this.app.StartMenuManager.IsDirty;
        }
    }
}

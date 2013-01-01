using Advent.MediaCenter.StartMenu.OEM;
using Advent.VmcStudio;
using System;
using System.ComponentModel;

namespace Advent.VmcStudio.StartMenu.Presenters
{
    internal class EntryPointDocument : VmcDocument
    {
        private Advent.MediaCenter.StartMenu.OEM.EntryPoint entryPoint;

        public EntryPointPresenter Presenter { get; private set; }

        public EntryPointDocument(Advent.MediaCenter.StartMenu.OEM.EntryPoint oemEntryPoint)
        {
            this.entryPoint = oemEntryPoint;
            this.IsDirty = oemEntryPoint.IsDirty;
            this.entryPoint.IsDirtyChanged += (EventHandler)delegate
            {
                this.IsDirty = this.entryPoint.IsDirty;
            };
            this.Presenter = new EntryPointPresenter(this.entryPoint);
            this.Presenter.PropertyChanged += new PropertyChangedEventHandler(this.PresenterPropertyChanged);
            this.UpdatePresenterProperties();
        }

        public void Save()
        {
            this.entryPoint.Save();
        }

        private void PresenterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdatePresenterProperties();
        }

        private void UpdatePresenterProperties()
        {
            this.Name = this.Presenter.Title;
        }
    }
}

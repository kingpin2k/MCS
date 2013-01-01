using Advent.VmcStudio;
using System.ComponentModel;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ThemeEditDocument : VmcDocument
    {
        public ThemeModel Theme { get; private set; }

        public ThemeEditDocument(ThemeModel theme)
        {
            theme.Theme.PropertyChanged += new PropertyChangedEventHandler(this.Theme_PropertyChanged);
            this.Theme = theme;
            this.UpdateDocument();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            this.Theme.Theme.Dispose();
        }

        private void Theme_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName == "Name") && !(e.PropertyName == "IsDirty"))
                return;
            this.UpdateDocument();
        }

        private void UpdateDocument()
        {
            this.Name = this.Theme.Theme.Name;
            this.IsDirty = this.Theme.Theme.IsDirty;
        }
    }
}

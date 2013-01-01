using Advent.Common.UI;
using System;

namespace Advent.VmcStudio
{
    internal abstract class VmcDocument : NotifyPropertyChangedBase, IDisposable
    {
        private string name;
        private bool isDirty;

        public string Name
        {
            get
            {
                return this.name;
            }
            protected set
            {
                if (!(this.name != value))
                    return;
                this.name = value;
                this.OnPropertyChanged("Name");
                this.OnPropertyChanged("Title");
            }
        }

        public string Title
        {
            get
            {
                if (!this.IsDirty)
                    return this.Name;
                else
                    return this.Name + "*";
            }
        }

        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }
            protected set
            {
                if (this.isDirty == value)
                    return;
                this.isDirty = value;
                this.OnPropertyChanged("IsDirty");
                this.OnPropertyChanged("Title");
            }
        }

        ~VmcDocument()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}

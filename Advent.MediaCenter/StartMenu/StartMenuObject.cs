
using Advent.Common.IO;
using System.ComponentModel;
using System.Windows;

namespace Advent.MediaCenter.StartMenu
{
    public class StartMenuObject : DependencyObject, ISupportInitialize
    {
        private readonly StartMenuManager startMenuManager;
        private bool isDirty;
        private bool inInit;

        public StartMenuManager Manager
        {
            get
            {
                return this.startMenuManager;
            }
        }

        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }
            set
            {
                if (this.isDirty == value)
                    return;
                this.isDirty = value;
                if (!this.isDirty || this.Manager == null)
                    return;
                this.Manager.IsDirty = true;
            }
        }

        protected bool IsInitInProgress
        {
            get
            {
                return this.inInit;
            }
        }

        public StartMenuObject(StartMenuManager smm)
        {
            this.startMenuManager = smm;
        }

        public virtual void BeginInit()
        {
            this.inInit = true;
        }

        public virtual void EndInit()
        {
            this.inInit = false;
            this.IsDirty = false;
        }

        internal virtual void Save(IResourceLibrary ehres)
        {
            this.IsDirty = false;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (this.inInit || !this.CanSetDirty(e.Property))
                return;
            this.IsDirty = true;
        }

        protected virtual bool CanSetDirty(DependencyProperty property)
        {
            return !property.ReadOnly;
        }
    }
}

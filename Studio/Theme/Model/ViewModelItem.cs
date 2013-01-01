using Advent.Common.UI;
using System.ComponentModel;

namespace Advent.VmcStudio.Theme.Model
{
    public abstract class ViewModelItem<T, K> : NotifyPropertyChangedBase where T : INotifyPropertyChanged
    {
        private K originalValue;
        private K defaultValue;

        public abstract K Value { get; }

        public T Item { get; private set; }

        public K DefaultValue
        {
            get
            {
                return this.defaultValue;
            }
            set
            {
                this.defaultValue = value;
                this.OnPropertyChanged("IsDefault");
            }
        }

        public bool IsDirty
        {
            get
            {
                return !this.IsEqual(this.Value, this.originalValue);
            }
        }

        public bool IsDefault
        {
            get
            {
                return this.IsEqual(this.Value, this.DefaultValue);
            }
        }

        public ViewModelItem(T item)
        {
            this.Item = item;
            this.originalValue = this.Clone(this.Value);
            this.Item.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
        }

        public abstract K Clone(K obj);

        public virtual bool IsEqual(K a, K b)
        {
            if ((object)a == null && (object)b == null)
                return true;
            if ((object)a != null)
                return a.Equals((object)b);
            else
                return false;
        }

        public void ClearDirty()
        {
            this.originalValue = this.Clone(this.Value);
            this.OnPropertyChanged("IsDirty");
        }

        protected virtual void OnValueChanged()
        {
            this.OnPropertyChanged("Value");
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("IsDefault");
            this.OnValueChanged();
            this.OnPropertyChanged("IsDirty");
        }
    }
}

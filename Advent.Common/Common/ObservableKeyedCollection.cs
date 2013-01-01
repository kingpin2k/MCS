using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Advent.Common
{
    public abstract class ObservableKeyedCollection<TKey, TValue> : ObservableCollection<TValue>
    {
        private readonly Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

        protected Dictionary<TKey, TValue> Dictionary
        {
            get
            {
                return this.dict;
            }
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                return this.dict[key];
            }
        }

        protected override void InsertItem(int index, TValue item)
        {
            base.InsertItem(index, item);
            TKey key = this.GetKey(item);
            TValue obj;
            if (this.dict.TryGetValue(key, out obj))
                this.Remove(obj);
            this.dict[key] = item;
        }

        protected override void RemoveItem(int index)
        {
            this.dict.Remove(this.GetKey(base[index]));
            base.RemoveItem(index);
        }

        protected abstract TKey GetKey(TValue obj);
    }
}

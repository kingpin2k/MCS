
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Advent.Common
{
    public static class CollectionUtilities
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (values == null)
                throw new ArgumentNullException("values");
            foreach (T obj in values)
                collection.Add(obj);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (values == null)
                throw new ArgumentNullException("values");
            foreach (T obj in values)
                collection.Remove(obj);
        }

        public static void InsertRange<T>(this IList<T> collection, int index, IEnumerable<T> values)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (values == null)
                throw new ArgumentNullException("values");
            foreach (T obj in values)
            {
                collection.Insert(index, obj);
                ++index;
            }
        }

        public static void StopReplicatingChanges(this INotifyCollectionChanged from, NotifyCollectionChangedEventHandler handle)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            from.CollectionChanged -= handle;
        }

        public static NotifyCollectionChangedEventHandler StartReplicatingChangesTo<TTo>(this INotifyCollectionChanged from, ICollection<TTo> to)
        {
            return CollectionUtilities.StartReplicatingChangesTo<TTo>(from, to, false);
        }

        public static NotifyCollectionChangedEventHandler StartReplicatingChangesTo<TTo>(this INotifyCollectionChanged from, ICollection<TTo> to, bool preserveOrder)
        {
            return CollectionUtilities.StartReplicatingChangesTo<TTo>(from, to, preserveOrder, (Func<object, TTo>)(o => (TTo)o));
        }

        public static NotifyCollectionChangedEventHandler StartReplicatingChangesTo<TTo>(this INotifyCollectionChanged from, ICollection<TTo> to, bool preserveOrder, Func<object, TTo> selector)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");
            NotifyCollectionChangedEventHandler changedEventHandler = (NotifyCollectionChangedEventHandler)((sender, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (object item_0 in (IEnumerable)e.OldItems)
                        to.Remove(selector(item_0));
                }
                if (e.NewItems == null)
                    return;
                List<TTo> local_1 = new List<TTo>();
                foreach (object item_1 in (IEnumerable)e.NewItems)
                    local_1.Add(selector(item_1));
                IList<TTo> local_3;
                if (preserveOrder && (local_3 = to as IList<TTo>) != null)
                    CollectionUtilities.InsertRange<TTo>(local_3, e.NewStartingIndex, (IEnumerable<TTo>)local_1);
                else
                    CollectionUtilities.AddRange<TTo>(to, (IEnumerable<TTo>)local_1);
            });
            from.CollectionChanged += changedEventHandler;
            return changedEventHandler;
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            if (collection != null)
                return collection.Count == 0;
            else
                return true;
        }

        public static void BindTo<TFrom, TTo>(this ObservableCollection<TFrom> from, ICollection<TTo> to) where TFrom : TTo
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");
            to.Clear();
            CollectionUtilities.AddRange<TTo>(to, Enumerable.Cast<TTo>((IEnumerable)from));
            CollectionUtilities.StartReplicatingChangesTo<TTo>((INotifyCollectionChanged)from, to, true);
        }
    }
}

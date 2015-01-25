using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatticeUtils.Collections
{
    /// <summary>
    /// A ReadOnlyCollection that works with non-list collections.
    /// </summary>
    /// <typeparam name="T">the type of element in the collection</typeparam>
    public class ReadOnlyCollection<T> : ICollection<T>, System.Collections.ICollection//, IReadOnlyCollection<T>
    {
        private readonly ICollection<T> collection;

        /// <summary>
        /// Constructs a read-only collection backed by the specified collection.
        /// </summary>
        /// <param name="collection">the backing collection</param>
        /// <exception cref="System.ArgumentNullException">the collection is null</exception>
        public ReadOnlyCollection(ICollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            this.collection = collection;
        }

        /// <inheritdoc />
        public int Count
        {
            get { return collection.Count; }
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        #region IEnumerator

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)collection).GetEnumerator();
        }

        #endregion

        #region ICollection

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            ((System.Collections.ICollection)collection).CopyTo(array, index);
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return ((System.Collections.ICollection)collection).IsSynchronized; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return ((System.Collections.ICollection)collection).SyncRoot; }
        }

        #endregion

        #region ICollection<T>

        /// <summary>
        /// Always true.
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException">always</exception>
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}

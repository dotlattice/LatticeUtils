using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatticeUtils
{
    /// <summary>
    /// Methods based on generic <c>List</c> methods that work with all generic collections.
    /// </summary>
    public static class CollectionUtils
    {
        #region Based on List Methods

        /// <summary>
        /// Adds the specified values to the end of the collection.
        /// </summary>
        /// <param name="collection">the collection to which to append the values</param>
        /// <param name="values">the values to append</param>
        /// <exception cref="System.ArgumentNullException">collection or values is null</exception>
        public static void AddRange<T>(ICollection<T> collection, IEnumerable<T> values)
        {
            if (collection is List<T>)
            {
                var list = (List<T>)collection;
                list.AddRange(values);
                return;
            }

            if (collection == null) throw new ArgumentNullException("collection");
            if (values == null) throw new ArgumentNullException("values");

            foreach (var element in values)
            {
                collection.Add(element);
            }
        }

        /// <summary>
        ///  Removes the element at the specified index from the collection.
        /// </summary>
        /// <remarks>
        /// If the element at the specified index is in the collection more than once, then one copy 
        /// will be removed but there is no guarantee that it will be the copy at the specified index.
        /// </remarks>
        /// <param name="collection">the collection from which to remove an element</param>
        /// <param name="index">the zero-based index of the element to remove</param>
        /// <exception cref="System.ArgumentNullException">collection is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0 or greater than the Count of the collection</exception>
        public static void RemoveAt<T>(ICollection<T> collection, int index)
        {
            if (collection is IList<T>)
            {
                var list = (IList<T>)collection;
                list.RemoveAt(index);
                return;
            }

            if (collection == null) throw new ArgumentNullException("collection");
            if (index < 0 || index >= collection.Count) throw new ArgumentOutOfRangeException("index", index, string.Format("index is out of range (0, {0}]", collection.Count));

            var element = collection.ElementAt(index);
            collection.Remove(element);
        }

        /// <summary>
        /// Removes all elements in the collection that match the specified predicate.
        /// </summary>
        /// <param name="collection">the collection from which to remove elements</param>
        /// <param name="predicate">a predicate that defines the conditions of the elements to remove</param>
        /// <exception cref="System.ArgumentNullException">collection or predicate is null</exception>
        public static void RemoveAll<T>(ICollection<T> collection, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            if (collection is List<T>)
            {
                var list = (List<T>)collection;
                list.RemoveAll(t => predicate(t));
                return;
            }

            if (collection == null) throw new ArgumentNullException("collection");
            var elementsToRemove = collection.Where(predicate).ToArray();
            foreach (var element in elementsToRemove)
            {
                collection.Remove(element);
            }
        }

        /// <summary>
        /// Removes a range of elements from the collection.
        /// </summary>
        /// <remarks>
        /// If any of the elements in the specified range are in the collection more than once, then one copy 
        /// of each will be removed but there is no guarantee that it will be the copies in the specified range.
        /// </remarks>
        /// <param name="collection">the collection from which to remove elements</param>
        /// <param name="index">the zero-based starting index of the range of elements to remove</param>
        /// <param name="count">the number of elements to remove</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0 or count is less than 0</exception>
        /// /// <exception cref="System.ArgumentException">index and count do not denote a valid range of elements in the System.Collections.Generic.List<T>.</exception>
        public static void RemoveRange<T>(ICollection<T> collection, int index, int count)
        {
            if (collection is List<T>)
            {
                var list = (List<T>)collection;
                list.RemoveRange(index, count);
                return;
            }
            if (collection is IList<T>)
            {
                var list = (IList<T>)collection;
                foreach (var i in Enumerable.Range(index, count).Reverse())
                {
                    list.RemoveAt(i);
                }
                return;
            }

            if (collection == null) throw new ArgumentNullException("collection");
            if (index < 0 ) throw new ArgumentOutOfRangeException("index", index, "index cannot be negative");
            if (count < 0) throw new ArgumentOutOfRangeException("count", index, "count cannot be negative");
            if (index + count > collection.Count) throw new ArgumentException(string.Format("index {0} and count {1} are invalid for collection with size {2}", index, count, collection.Count));
            
            var elementsToRemove = collection.Skip(index).Take(count).ToArray();
            foreach (var element in elementsToRemove)
            {
                collection.Remove(element);
            }
        }

        /// <summary>
        /// Returns a read-only wrapper for the specified collection.
        /// </summary>
        /// <param name="collection">the collection to wrap</param>
        /// <returns>the read-only collection</returns>
        /// <exception cref="System.ArgumentNullException">collection is null</exception>
        public static ICollection<T> AsReadOnly<T>(ICollection<T> collection)
        {
            if (collection is List<T>)
            {
                var list = (List<T>)collection;
                return list.AsReadOnly();
            }
            if (collection is System.Collections.ObjectModel.ReadOnlyCollection<T>)
            {
                return collection;
            }
            if (collection is LatticeUtils.Collections.ReadOnlyCollection<T>)
            {
                return collection;
            }
            
            return new LatticeUtils.Collections.ReadOnlyCollection<T>(collection);
        }

        /// <summary>
        /// Returns the index of a specific item in the enumerable.
        /// </summary>
        /// <param name="enumerable">the enumerable in which to search</param>
        /// <param name="item">the item to locate</param>
        /// <returns>the index of item if found, otherwise -1</returns>
        /// <exception cref="System.ArgumentNullException">enumerable is null</exception>
        public static int IndexOf<T>(IEnumerable<T> enumerable, T item)
        {
            if (enumerable is IList<T>)
            {
                var list = (IList<T>)enumerable;
                return list.IndexOf(item);
            }

            if (enumerable == null) throw new ArgumentNullException("enumerable");

            int indexCounter = 0;
            if (item == null)
            {
                foreach (var e in enumerable)
                {
                    if (e == null)
                    {
                        return indexCounter;
                    }
                    indexCounter++;
                }
            }
            else
            {
                foreach (var e in enumerable)
                {
                    if (e != null && e.Equals(item))
                    {
                        return indexCounter;
                    }
                    indexCounter++;
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches for an element that matches the specified predicate, 
        /// and returns the zero-based index of the first occurrence in the enumerable.
        /// </summary>
        /// <param name="enumerable">the enumerable in which to search</param>
        /// <param name="predicate">the predicate that defines the conditions of the element to search for</param>
        /// <returns>the zero-based index of the first occurrence of an element that matches the predicate or –1 if there is no match</returns>
        /// <exception cref="System.ArgumentNullException">enumerable or predicate is null</exception>
        public static int FindIndex<T>(IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (enumerable is List<T>)
            {
                var list = (List<T>)enumerable;
                return list.FindIndex(e => predicate(e));
            }

            if (enumerable == null) throw new ArgumentNullException("enumerable");

            int indexCounter = 0;
            foreach (var e in enumerable)
            {
                if (predicate(e))
                {
                    return indexCounter;
                }
                indexCounter++;
            }

            return -1;
        }

        /// <summary>
        /// Copies a range of elements from the collection to a target array, starting at the specified 
        /// indexes of the source collection and target array.
        /// </summary>
        /// <param name="sourceEnumerable">the source enumerable from which to copy elements</param>
        /// <param name="sourceIndex">the zero-based index in the source collection at which copying begins</param>
        /// <param name="targetArray">the target array to copy elements into</param>
        /// <param name="targetArrayIndex">the zero-based index in target array at which copying begins </param>
        /// <param name="count">the number of elements to copy</param>
        /// <exception cref="System.ArgumentNullException">collection or array is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index, arrayIndex, or count is less than 0</exception>
        /// <exception cref="System.ArgumentException">
        /// index is greater than or equal to the source collection count 
        /// or there is not enough room to copy the elements into the target array
        /// </exception>
        public static void CopyTo<T>(IEnumerable<T> sourceEnumerable, int sourceIndex, T[] targetArray, int targetArrayIndex, int count)
        {
            if (sourceEnumerable == null) throw new ArgumentNullException("sourceEnumerable");
            if (targetArray == null) throw new ArgumentNullException("targetArray");

            if (sourceEnumerable is List<T>)
            {
                var list = (List<T>)sourceEnumerable;
                list.CopyTo(sourceIndex, targetArray, targetArrayIndex, count);
                return;
            }
            if (sourceEnumerable is Array)
            {
                var sourceArray = (Array)sourceEnumerable;
                Array.Copy(sourceArray, sourceIndex, targetArray, targetArrayIndex, count);
                return;
            }
            if (sourceEnumerable is ICollection<T>)
            {
                var collection = (ICollection<T>)sourceEnumerable;
                if (collection.Count == count - sourceIndex)
                {
                    collection.CopyTo(targetArray, targetArrayIndex);
                    return;
                }
            }

            if (sourceIndex < 0) throw new ArgumentOutOfRangeException("index", sourceIndex, "index cannot be negative");
            if (targetArrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex", targetArrayIndex, "arrayIndex cannot be negative");
            if (count < 0) throw new ArgumentOutOfRangeException("count", count, "count cannot be negative");

            var elementsToCopy = sourceEnumerable.Skip(sourceIndex).Take(count).ToArray();
            if (elementsToCopy.Length != count || targetArrayIndex + count > targetArray.Length)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }
            elementsToCopy.CopyTo(targetArray, targetArrayIndex);
        }

        /// <summary>
        /// Performs the specified action on each element of the enumerable.
        /// </summary>
        /// <param name="enumerable">the enumerable to loop through</param>
        /// <param name="action">the action to perform on each element in the enumerable</param>
        /// <exception cref="System.ArgumentNullException">enumerable or action is null</exception>
        public static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (enumerable is List<T>)
            {
                var list = (List<T>)enumerable;
                list.ForEach(action);
                return;
            }

            if (enumerable == null) throw new ArgumentNullException("enumerable");
            foreach (var e in enumerable)
            {
                action(e);
            }
        }

        #endregion

        #region Custom

        /// <summary>
        /// Groups an enumerable into pages with a maximum size.
        /// </summary>
        /// <remarks>
        /// The returned enumerable uses deferred execution, so it will only pull one page from the given enumerable at a time.
        /// </remarks>
        /// <typeparam name="T">the type of element in the enumerable</typeparam>
        /// <param name="enumerable">the enumerable to paginate</param>
        /// <param name="pageSize">the maximum number of items per page (every page with the possible exception of the last one will have this size)</param>
        /// <returns>an enumerable of page collections</returns>
        public static IEnumerable<ICollection<T>> Paginate<T>(IEnumerable<T> enumerable, int pageSize)
        {
            if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize", pageSize, "pageSize must be positive");

            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var pageList = new List<T>(capacity: pageSize);
                    pageList.Add(enumerator.Current);
                    for (int i = 1; i < pageSize && enumerator.MoveNext(); i++)
                    {
                        pageList.Add(enumerator.Current);
                    }
                    yield return pageList;
                }
            }
        }

        /// <summary>
        /// Groups an enumerable into page groupings with a maximum size.
        /// </summary>
        /// <remarks>
        /// The returned enumerable uses deferred execution, so it will only pull one page from the given enumerable at a time.
        /// </remarks>
        /// <typeparam name="T">the type of element in the enumerable</typeparam>
        /// <param name="enumerable">the enumerable to paginate</param>
        /// <param name="pageSize">the maximum number of items per page (every page with the possible exception of the last one will have this size)</param>
        /// <returns>an enumerable of page groupings; the index in the groupings is zero-based</returns>
        public static IEnumerable<IGrouping<int, T>> PaginateIntoGroupings<T>(IEnumerable<T> enumerable, int pageSize)
        {
            var pages = Paginate(enumerable, pageSize);

            int pageCounter = 0;
            foreach (var page in pages)
            {
                yield return new LatticeUtils.Collections.Grouping<int, T>(pageCounter, page);
                pageCounter++;
            }
        }

        #endregion
    }
}

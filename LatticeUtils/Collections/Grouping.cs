using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LatticeUtils.Collections
{
    /// <summary>
    /// A generic collection of objects that all share a key value.
    /// </summary>
    /// <typeparam name="TKey">the type of the key for the grouping</typeparam>
    /// <typeparam name="TElement">the type of each element in the grouping</typeparam>
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly TKey key;
        private readonly IEnumerable<TElement> elements;

        /// <summary>
        /// Constructs a grouping with the specified key and elements.
        /// </summary>
        /// <param name="key">the key value of this grouping</param>
        /// <param name="elements">the elements in this grouping</param>
        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            if (elements == null) throw new ArgumentNullException("elements");
            this.key = key;
            this.elements = elements;
        }

        /// <summary>
        /// The key value shared by all of the elements in this grouping.
        /// </summary>
        public TKey Key
        {
            get { return key; }
        }

        /// <inheritdoc />
        public IEnumerator<TElement> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)elements).GetEnumerator();
        }
    }
}

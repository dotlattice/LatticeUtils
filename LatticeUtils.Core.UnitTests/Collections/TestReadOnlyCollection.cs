using LatticeUtils.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.Core.UnitTests.Collections
{
    public class TestReadOnlyCollection
    {
        [Test]
        public void IsReadOnly_ReturnsTrue()
        {
            ICollection<int> readOnlyCollection = new ReadOnlyCollection<int>(new List<int>());
            Assert.IsTrue(readOnlyCollection.IsReadOnly);
        }

        [Test]
        public void Add_ThrowsNotSupportedException()
        {
            ICollection<int> readOnlyCollection = new ReadOnlyCollection<int>(new List<int>());
            Assert.Throws<NotSupportedException>(() => readOnlyCollection.Add(2));
        }

        [Test]
        public void Remove_ThrowsNotSupportedException()
        {
            ICollection<int> readOnlyCollection = new ReadOnlyCollection<int>(new List<int> { 2 });
            Assert.Throws<NotSupportedException>(() => readOnlyCollection.Remove(2));
        }

        [Test]
        public void Clear_ThrowsNotSupportedException()
        {
            ICollection<int> readOnlyCollection = new ReadOnlyCollection<int>(new List<int> { 2 });
            Assert.Throws<NotSupportedException>(() => readOnlyCollection.Clear());
        }

        [Test]
        public void AddToBackingListModifiesReadOnlyCollection()
        {
            var list = new List<int>();
            var readOnlyCollection = new ReadOnlyCollection<int>(list);

            Assert.AreEqual(0, readOnlyCollection.Count);
            list.Add(2);

            Assert.AreEqual(1, readOnlyCollection.Count);
            Assert.AreEqual(2, readOnlyCollection.Single());
        }
    }
}

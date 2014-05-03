using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.Core.UnitTests
{
    public class TestCollectionUtils
    {
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(LinkedList<int>))]
        public void AddRange(Type type)
        {
            var collection = CreateIntCollection(type);
            CollectionUtils.AddRange(collection, new[] { 1, 2, 3 });

            Assert.AreEqual(3, collection.Count);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, collection);
        }

        [Test]
        public void AddRange_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.AddRange<int>(null, null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.AddRange(null, new[] { 1, 2, 3 }));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.AddRange(new LinkedList<int>(), null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.AddRange(new List<int>(), null));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(LinkedList<int>))]
        public void RemoveAt(Type type)
        {
            ICollection<int> collection = CreateIntCollection(type, new[] { 1, 2, 3 });
            CollectionUtils.RemoveAt(collection, 1);

            Assert.AreEqual(2, collection.Count);
            CollectionAssert.AreEqual(new[] { 1, 3 }, collection);
        }

        [Test]
        public void RemoveAt_Duplicates_List()
        {
            ICollection<int> collection = new List<int> { 2, 1, 2, 3 };
            CollectionUtils.RemoveAt(collection, 2);
            CollectionAssert.AreEqual(new[] { 2, 1, 3 }, collection);
        }

        [Test]
        public void RemoveAt_Duplicates_LinkedList()
        {
            ICollection<int> collection = new LinkedList<int>(new[] { 2, 1, 2, 3 });
            CollectionUtils.RemoveAt(collection, 2);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, collection);
        }

        [Test]
        public void RemoveAt_IListMock()
        {
            var collectionMock = new Mock<IList<int>>();
            collectionMock.Setup(x => x.RemoveAt(1)).Verifiable();

            CollectionUtils.RemoveAt(collectionMock.Object, 1);

            collectionMock.Verify();
        }

        [Test]
        public void RemoveAt_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.RemoveAt<int>(null, 0));
        }

        [Test]
        public void RemoveAt_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveAt<int>(new LinkedList<int>(), -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveAt<int>(new LinkedList<int>(), 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveAt<int>(new List<int>(), -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveAt<int>(new List<int>(), 1));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(LinkedList<int>))]
        public void RemoveRange(Type type)
        {
            ICollection<int> collection = CreateIntCollection(type, new[] { 1, 2, 3, 4, 5, 6 });
            CollectionUtils.RemoveRange(collection, 3, 2);

            Assert.AreEqual(4, collection.Count);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 6 }, collection);
        }

        [Test]
        public void RemoveRange_Duplicates_List()
        {
            ICollection<int> collection = new List<int> { 3, 1, 2, 3, 4, 5, 6 };
            CollectionUtils.RemoveRange(collection, 3, 2);

            Assert.AreEqual(5, collection.Count);
            CollectionAssert.AreEqual(new[] { 3, 1, 2, 5, 6 }, collection);
        }

        [Test]
        public void RemoveRange_Duplicates_LinkedList()
        {
            ICollection<int> collection = new LinkedList<int>(new[] { 3, 1, 2, 3, 4, 5, 6 });
            CollectionUtils.RemoveRange(collection, 3, 2);

            Assert.AreEqual(5, collection.Count);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 5, 6 }, collection);
        }

        [Test]
        public void RemoveRange_IListMock()
        {
            var collectionMock = new Mock<IList<int>>();
            collectionMock.Setup(x => x.RemoveAt(3)).Verifiable();
            collectionMock.Setup(x => x.RemoveAt(4)).Verifiable();

            CollectionUtils.RemoveRange(collectionMock.Object, 3, 2);

            collectionMock.Verify();
        }

        [Test]
        public void RemoveRange_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.RemoveRange<int>(null, 0, 1));
        }

        [Test]
        public void RemoveRange_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveRange<int>(new List<int>(), -1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveRange<int>(new List<int>(new[] { 1 }), 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveRange<int>(new List<int>(new[] { 1 }), -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveRange<int>(new LinkedList<int>(), -1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveRange<int>(new LinkedList<int>(new[] { 1 }), 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.RemoveRange<int>(new LinkedList<int>(new[] { 1 }), -1, 1));
        }

        [Test]
        public void RemoveRange_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => CollectionUtils.RemoveRange<int>(new List<int>(), 99, 99));
            Assert.Throws<ArgumentException>(() => CollectionUtils.RemoveRange<int>(new List<int>(new[] { 1, 2 }), 1, 2));
            Assert.Throws<ArgumentException>(() => CollectionUtils.RemoveRange<int>(new LinkedList<int>(), 99, 99));
            Assert.Throws<ArgumentException>(() => CollectionUtils.RemoveRange<int>(new LinkedList<int>(new[] { 1, 2 }), 1, 2));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(LinkedList<int>))]
        public void RemoveAll(Type type)
        {
            ICollection<int> collection = CreateIntCollection(type, new[] { 2, 1, 2, 3 });
            CollectionUtils.RemoveAll(collection, e => e == 2);

            Assert.AreEqual(2, collection.Count);
            CollectionAssert.AreEqual(new[] { 1, 3 }, collection);
        }

        [Test]
        public void RemoveAll_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.RemoveAll<int>(null, null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.RemoveAll<int>(null, e => true));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.RemoveAll<int>(new LinkedList<int>(), null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.RemoveAll<int>(new List<int>(), null));
        }

        [Test]
        public void AsReadOnly_List()
        {
            var collection = new List<int>(new[] { 1, 2, 3});
            var readOnlyCollection = CollectionUtils.AsReadOnly<int>(collection);
            Assert.AreEqual(typeof(System.Collections.ObjectModel.ReadOnlyCollection<int>), readOnlyCollection.GetType());
            CollectionAssert.AreEqual(collection, readOnlyCollection);
        }

        [Test]
        public void AsReadOnly_LinkedList()
        {
            var collection = new LinkedList<int>(new[] { 1, 2, 3 });
            var readOnlyCollection = CollectionUtils.AsReadOnly<int>(collection);
            Assert.AreEqual(typeof(LatticeUtils.Collections.ReadOnlyCollection<int>), readOnlyCollection.GetType());
            CollectionAssert.AreEqual(collection, readOnlyCollection);
        }

        [Test]
        public void AsReadOnly_ObjectModelReadOnlyCollection()
        {
            var inputReadOnlyCollection = new List<int>(new[] { 1, 2, 3 }).AsReadOnly();
            var outputReadOnlyCollection = CollectionUtils.AsReadOnly<int>(inputReadOnlyCollection);
            Assert.AreSame(inputReadOnlyCollection, outputReadOnlyCollection);
        }

        [Test]
        public void AsReadOnly_LatticeUtilsReadOnlyCollection()
        {
            var inputReadOnlyCollection = new LatticeUtils.Collections.ReadOnlyCollection<int>(new[] { 1, 2, 3 });
            var outputReadOnlyCollection = CollectionUtils.AsReadOnly<int>(inputReadOnlyCollection);
            Assert.AreSame(inputReadOnlyCollection, outputReadOnlyCollection);
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(LinkedList<int>))]
        public void IndexOf(Type type)
        {
            IEnumerable<int> enumerable = CreateIntCollection(type, new[] { 10, 20, 30 });
            Assert.AreEqual(0, CollectionUtils.IndexOf(enumerable, 10));
            Assert.AreEqual(1, CollectionUtils.IndexOf(enumerable, 20));
            Assert.AreEqual(2, CollectionUtils.IndexOf(enumerable, 30));
            Assert.AreEqual(-1, CollectionUtils.IndexOf(enumerable, 40));
        }

        [Test]
        public void IndexOf_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.IndexOf<int>(null, 0));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(LinkedList<int>))]
        public void FindIndex(Type type)
        {
            IEnumerable<int> enumerable = CreateIntCollection(type, new[] { 10, 20, 30, 20 });
            Assert.AreEqual(0, CollectionUtils.FindIndex(enumerable, e => e == 10));
            Assert.AreEqual(1, CollectionUtils.FindIndex(enumerable, e => e == 20));
            Assert.AreEqual(2, CollectionUtils.FindIndex(enumerable, e => e == 30));
            Assert.AreEqual(-1, CollectionUtils.FindIndex(enumerable, e => e == 40));
        }

        [Test]
        public void FindIndex_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.FindIndex<int>(null, e => e == 2));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.FindIndex(new List<int>(), null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.FindIndex(new LinkedList<int>(), null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.FindIndex(new List<int>(new[] { 1 }), null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.FindIndex(new LinkedList<int>(new[] { 1 }), null));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(LinkedList<int>))]
        public void CopyTo(Type type)
        {
            IEnumerable<int> enumerable = CreateIntCollection(type, new[] { 1, 2, 3, 4 });

            var targetArray = new int[2];
            CollectionUtils.CopyTo(enumerable, 1, targetArray, 0, 2);
            CollectionAssert.AreEqual(new[] { 2, 3 }, targetArray);
        }

        [Test]
        public void CopyTo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.CopyTo<int>(null, 0, null, 0, 1));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.CopyTo<int>(null, 0, new int[1], 0, 1));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.CopyTo(new List<int>(new[] { 1 }), 0, null, 0, 1));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.CopyTo(new List<int>(), 0, null, 0, 1));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.CopyTo(new LinkedList<int>(new[] { 1 }), 0, null, 0, 1));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.CopyTo(new LinkedList<int>(), 0, null, 0, 1));
        }

        [Test]
        public void CopyTo_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.CopyTo(new List<int>(new[] { 1 }), -1, new int[1], 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.CopyTo(new List<int>(new[] { 1 }), 0, new int[1], -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.CopyTo(new List<int>(new[] { 1 }), 0, new int[1], 0, -1));

            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.CopyTo(new LinkedList<int>(new[] { 1 }), -1, new int[1], 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.CopyTo(new LinkedList<int>(new[] { 1 }), 0, new int[1], -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionUtils.CopyTo(new LinkedList<int>(new[] { 1 }), 0, new int[1], 0, -1));
        }

        [Test]
        public void CopyTo_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => CollectionUtils.CopyTo(new List<int>(new[] { 1 }), 0, new int[1], 0, 2));
            Assert.Throws<ArgumentException>(() => CollectionUtils.CopyTo(new List<int>(new[] { 1, 2 }), 0, new int[1], 0, 2));
            Assert.Throws<ArgumentException>(() => CollectionUtils.CopyTo(new List<int>(new[] { 1, 2 }), 0, new int[2], 1, 2));
            Assert.Throws<ArgumentException>(() => CollectionUtils.CopyTo(new List<int>(new[] { 1, 2 }), 1, new int[2], 0, 2));

            Assert.Throws<ArgumentException>(() => CollectionUtils.CopyTo(new LinkedList<int>(new[] { 1 }), 0, new int[1], 0, 2));
            Assert.Throws<ArgumentException>(() => CollectionUtils.CopyTo(new LinkedList<int>(new[] { 1, 2 }), 0, new int[1], 0, 2));
            Assert.Throws<ArgumentException>(() => CollectionUtils.CopyTo(new LinkedList<int>(new[] { 1, 2 }), 0, new int[2], 1, 2));
            Assert.Throws<ArgumentException>(() => CollectionUtils.CopyTo(new LinkedList<int>(new[] { 1, 2 }), 1, new int[2], 0, 2));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(LinkedList<int>))]
        public void ForEach(Type type)
        {
            IEnumerable<int> enumerable = CreateIntCollection(type, new[] { 1, 2, 3, 4 });

            int sum = 0;
            CollectionUtils.ForEach(enumerable, i => sum += i);
            Assert.AreEqual(10, sum);
        }

        public void ForEach_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.ForEach<int>(null, i => { }));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.ForEach(new List<int>(), null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.ForEach(new List<int>(new[] { 1 }), null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.ForEach(new LinkedList<int>(), null));
            Assert.Throws<ArgumentNullException>(() => CollectionUtils.ForEach(new LinkedList<int>(new[] { 1 }), null));
        }

        #region Helpers

        private ICollection<int> CreateIntCollection(Type type, int[] initialValues = null)
        {
            var collection = (ICollection<int>)Activator.CreateInstance(type);
            if (initialValues != null)
            {
                CollectionUtils.AddRange(collection, initialValues);
            }
            return collection;
        }

        #endregion
    }
}

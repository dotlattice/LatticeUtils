using LatticeUtils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.UnitTests
{
    public class TestTypeUtils
    {
        [TestCase(typeof(object), true)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(string), true)]
        public void IsNullable(Type type, bool expected)
        {
            Assert.AreEqual(expected, TypeUtils.IsNullable(type));
        }

        [TestCase(typeof(object), typeof(object))]
        [TestCase(typeof(int), typeof(int))]
        [TestCase(typeof(int?), typeof(int))]
        [TestCase(typeof(string), typeof(string))]
        public void UnwrapNullable(Type type, Type expected)
        {
            Assert.AreEqual(expected, TypeUtils.UnwrapNullable(type));
        }

        [TestCase(typeof(string), default(string))]
        [TestCase(typeof(int), default(int))]
        [TestCase(typeof(Nullable<int>), null)]
        public void Default(Type type, object expected)
        {
            Assert.AreEqual(expected, TypeUtils.Default(type));
        }

        [TestCase(typeof(object), false)]
        [TestCase(typeof(byte), true)]
        [TestCase(typeof(sbyte), true)]
        [TestCase(typeof(short), true)]
        [TestCase(typeof(ushort), true)]
        [TestCase(typeof(int), true)]
        [TestCase(typeof(uint), true)]
        [TestCase(typeof(long), true)]
        [TestCase(typeof(ulong), true)]
        [TestCase(typeof(float), false)]
        [TestCase(typeof(double), false)]
        [TestCase(typeof(decimal), false)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(string), false)]
        public void IsIntegral(Type type, bool expected)
        {
            Assert.AreEqual(expected, TypeUtils.IsIntegral(type));
        }

        [TestCase(typeof(object), false)]
        [TestCase(typeof(byte), true)]
        [TestCase(typeof(sbyte), true)]
        [TestCase(typeof(short), true)]
        [TestCase(typeof(ushort), true)]
        [TestCase(typeof(int), true)]
        [TestCase(typeof(uint), true)]
        [TestCase(typeof(long), true)]
        [TestCase(typeof(ulong), true)]
        [TestCase(typeof(float), true)]
        [TestCase(typeof(double), true)]
        [TestCase(typeof(decimal), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(float?), true)]
        [TestCase(typeof(double?), true)]
        [TestCase(typeof(decimal?), true)]
        [TestCase(typeof(string), false)]
        public void IsNumeric(Type type, bool expected)
        {
            Assert.AreEqual(expected, TypeUtils.IsNumeric(type));
        }

        [TestCase(typeof(object), false)]
        [TestCase(typeof(int), false)]
        [TestCase(typeof(IEnumerable<int>), true)]
        [TestCase(typeof(IEnumerable<>), true)]
        [TestCase(typeof(ICollection<int>), true)]
        [TestCase(typeof(IDictionary<string, string>), true)]
        [TestCase(typeof(IDictionary<,>), true)]
        [TestCase(typeof(string), true)]
        [TestCase(typeof(DateTime), false)]
        [TestCase(typeof(MultipleEnumerableImplementations), true)]
        public void ImplementsGenericInterface_GenericEnumerable(Type type, bool expected)
        {
            Assert.AreEqual(expected, TypeUtils.ImplementsGenericInterface(type, typeof(IEnumerable<>)));
        }

        [TestCase(typeof(object), null)]
        [TestCase(null, typeof(IEnumerable<>))]
        [TestCase(null, null)]
        public void ImplementsGenericInterface_ThrowsArgumentNullException(Type type, Type genericInterfaceType)
        {
            Assert.Throws<ArgumentNullException>(() => TypeUtils.ImplementsGenericInterface(type, genericInterfaceType));
        }

        [TestCase(typeof(object), typeof(string))]
        [TestCase(typeof(object), typeof(IEnumerable<int>))]
        public void ImplementsGenericInterface_ThrowsArgumentException(Type type, Type genericInterfaceType)
        {
            Assert.Throws<ArgumentException>(() => TypeUtils.ImplementsGenericInterface(type, genericInterfaceType));
        }

        [Test]
        public void GetInterfacesAndSelf_GenericList()
        {
            var interfaces = TypeUtils.GetInterfacesAndSelf(typeof(List<>)).ToList();
            Assert.AreEqual(8, interfaces.Count);
            CollectionAssert.AreEqual(typeof(List<>).GetInterfaces(), interfaces);
        }

        [Test]
        public void GetInterfacesAndSelf_ListInt()
        {
            var interfaces = TypeUtils.GetInterfacesAndSelf(typeof(List<int>)).ToList();
            Assert.AreEqual(8, interfaces.Count);
            CollectionAssert.AreEqual(typeof(List<int>).GetInterfaces(), interfaces);
        }

        [Test]
        public void GetInterfacesAndSelf_IEnumerable()
        {
            var interfaces = TypeUtils.GetInterfacesAndSelf(typeof(System.Collections.IEnumerable)).ToList();
            Assert.AreEqual(1, interfaces.Count);
            Assert.AreEqual(typeof(System.Collections.IEnumerable), interfaces.Single());
        }

        [Test]
        public void GetInterfacesAndSelf_GenericIEnumerable()
        {
            var interfaces = TypeUtils.GetInterfacesAndSelf(typeof(IEnumerable<>)).ToList();
            Assert.AreEqual(2, interfaces.Count);
            Assert.AreEqual(typeof(IEnumerable<>), interfaces.ElementAt(0));
            Assert.AreEqual(typeof(System.Collections.IEnumerable), interfaces.ElementAt(1));
        }

        [Test]
        public void GetInterfacesAndSelf_IEnumerableInt()
        {
            var interfaces = TypeUtils.GetInterfacesAndSelf(typeof(IEnumerable<int>)).ToList();
            Assert.AreEqual(2, interfaces.Count);
            Assert.AreEqual(typeof(IEnumerable<int>), interfaces.ElementAt(0));
            Assert.AreEqual(typeof(System.Collections.IEnumerable), interfaces.ElementAt(1));
        }

        [Test]
        public void GetInterfacesAndSelf_MultipleEnumerableImplementations()
        {
            var interfaces = TypeUtils.GetInterfacesAndSelf(typeof(MultipleEnumerableImplementations)).ToList();
            Assert.AreEqual(3, interfaces.Count);
            Assert.AreEqual(typeof(IEnumerable<int>), interfaces.ElementAt(0));
            Assert.AreEqual(typeof(System.Collections.IEnumerable), interfaces.ElementAt(1));
            Assert.AreEqual(typeof(IEnumerable<string>), interfaces.ElementAt(2));
        }

        #region Inner classes

        /// <summary>
        /// A class that implements IEnumerable<> twice with two different type parameters.
        /// </summary>
        private class MultipleEnumerableImplementations : IEnumerable<int>, IEnumerable<string>
        {
            IEnumerator<string> IEnumerable<string>.GetEnumerator() { throw new NotImplementedException(); }
            IEnumerator<int> IEnumerable<int>.GetEnumerator() { throw new NotImplementedException(); }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
        }

        #endregion
    }
}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.Core.UnitTests
{
    public class TestAnonymousTypeUtils
    {
        [Test]
        public void CreateType_SameTypeTwice()
        {
            var typeDictionary = new Dictionary<string, Type>
            {
                { "a", typeof(int)  }
            };
            var t1 = AnonymousTypeUtils.CreateType(typeDictionary.ToList());
            var t2 = AnonymousTypeUtils.CreateType(typeDictionary.ToList());
            Assert.AreSame(t1, t2);
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void CreateType(bool isMutable, bool isPropertyOrderSignificant)
        {
            var typeDictionary = new Dictionary<string, Type>
            {
                { "b", typeof(string) },
                { "a", typeof(int) }
            };
            Type anonymousType;
            if (isMutable)
            {
                anonymousType = AnonymousTypeUtils.CreateMutableType(typeDictionary, isPropertyOrderSignificant: isPropertyOrderSignificant);
            }
            else
            {
                anonymousType = AnonymousTypeUtils.CreateType(typeDictionary, isPropertyOrderSignificant: isPropertyOrderSignificant);
            }

            Assert.AreEqual(2, anonymousType.GetProperties().Length);

            var indexA = isPropertyOrderSignificant ? 1 : 0;
            var indexB = isPropertyOrderSignificant ? 0 : 1;
            Assert.AreEqual("a", anonymousType.GetProperties().ElementAt(indexA).Name);
            Assert.AreEqual("b", anonymousType.GetProperties().ElementAt(indexB).Name);

            var propertyA = anonymousType.GetProperty("a");
            Assert.AreEqual(typeof(int), propertyA.PropertyType);
            Assert.IsTrue(propertyA.CanRead);
            Assert.AreEqual(isMutable, propertyA.CanWrite);

            var propertyB = anonymousType.GetProperty("b");
            Assert.AreEqual("b", propertyB.Name);
            Assert.AreEqual(typeof(string), propertyB.PropertyType);
            Assert.IsTrue(propertyB.CanRead);
            Assert.AreEqual(isMutable, propertyB.CanWrite);

            var constructors = anonymousType.GetConstructors();
            Assert.AreEqual(1, constructors.Length);

            var constructorParameters = constructors.Single().GetParameters();
            Assert.AreEqual(2, constructorParameters.Length);

            Assert.AreEqual("a", constructorParameters.ElementAt(indexA).Name);
            Assert.AreEqual("b", constructorParameters.ElementAt(indexB).Name);

            string expectedTypeName;
            if (isMutable)
            {
                if (isPropertyOrderSignificant)
                    expectedTypeName = "<>f__LatticeUtilsMutableAnonymousTypeFC2E7EE73C4F69821B908A4DF7585D1018EE7D9A`2";
                else
                    expectedTypeName = "<>f__LatticeUtilsMutableAnonymousType5D8B1241B0484DD20C2CFECA6F692BECFBAB5D18`2";
            }
            else
            {
                if (isPropertyOrderSignificant)
                    expectedTypeName = "<>f__LatticeUtilsAnonymousTypeFC2E7EE73C4F69821B908A4DF7585D1018EE7D9A`2";
                else
                    expectedTypeName = "<>f__LatticeUtilsAnonymousType5D8B1241B0484DD20C2CFECA6F692BECFBAB5D18`2";
            }

            Assert.AreEqual(expectedTypeName, anonymousType.Name);
        }

        [Test]
        public void CreateType_CommaInPropertyName()
        {
            // If we combine the property names with a comma delimiter (without any escaping), 
            // then these two types would have the same property string in their name.
            var t1 = AnonymousTypeUtils.CreateType(new Dictionary<string, Type>
            {
                { "a", typeof(int) },
                { "b,c", typeof(int) }
            });
            var t2 = AnonymousTypeUtils.CreateType(new Dictionary<string, Type>
            {
                { "a,b", typeof(int) },
                { "c", typeof(int) },
            });
            Assert.AreNotSame(t1, t2);
            Assert.AreNotEqual(t1, t2);
        }

        [Test]
        public void CreateObject_Properties()
        {
            var obj = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "b", "test" },
                { "a", 1 },
            });

            var propertyA = obj.GetType().GetProperty("a");
            Assert.AreEqual(1, propertyA.GetValue(obj));

            var propertyB = obj.GetType().GetProperty("b");
            Assert.AreEqual("test", propertyB.GetValue(obj));
        }

        [Test]
        public void CreateObject_NullValues()
        {
            var obj = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "a", null },
                { "b", null },
            });

            var propertyA = obj.GetType().GetProperty("a");
            Assert.AreEqual(propertyA.PropertyType, typeof(object));
            Assert.IsNull(propertyA.GetValue(obj));

            var propertyB = obj.GetType().GetProperty("b");
            Assert.AreEqual(propertyB.PropertyType, typeof(object));
            Assert.IsNull(propertyB.GetValue(obj));            
        }

        [Test]
        public void CreateObject_Equals_EqualObjects()
        {
            var valueDictionary = new Dictionary<string, object>
            {
                { "b", "test" },
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            };

            var objA = AnonymousTypeUtils.CreateObject(valueDictionary);
            var objB = AnonymousTypeUtils.CreateObject(valueDictionary);

            Assert.AreNotSame(objA, objB);
            Assert.AreEqual(objA, objB);
            Assert.IsTrue(objA.Equals(objA));
            Assert.IsTrue(objB.Equals(objB));
            Assert.IsTrue(objA.Equals(objB));
            Assert.IsTrue(objB.Equals(objA));
        }

        [Test]
        public void CreateObject_Equals_DifferentObjectSameType()
        {
            var objA = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "b", "test" },
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });

            var objB = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "b", "test2" },
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });

            Assert.AreNotSame(objA, objB);
            Assert.IsTrue(objA.Equals(objA));
            Assert.IsTrue(objB.Equals(objB));
            Assert.IsFalse(objA.Equals(objB));
            Assert.IsFalse(objB.Equals(objA));
        }

        [Test]
        public void CreateObject_Equals_DifferentObjectDifferentType()
        {
            var objA = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "b", "test" },
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });

            var objB = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "b", "test2" },
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
            });

            Assert.AreNotSame(objA, objB);
            Assert.IsTrue(objA.Equals(objA));
            Assert.IsTrue(objB.Equals(objB));
            Assert.IsFalse(objA.Equals(objB));
            Assert.IsFalse(objB.Equals(objA));
        }

        [Test]
        public void CreateObject_Equals_RealAnonymousType()
        {
            var objA = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "a", 1 },
                { "b", "test" },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });

            var objB = new { a = 1, b = "test", c = new DateTime(2014, 1, 1), d = new object() };

            Assert.AreNotSame(objA, objB);
            Assert.IsTrue(objA.Equals(objA));
            Assert.IsFalse(objA.Equals(objB));
            Assert.IsFalse(objB.Equals(objA));
        }


        [Test]
        public void CreateObject_Equals_Null()
        {
            var objA = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "a", 1 },
                { "b", "test" },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });

            object objB = null;

            Assert.IsFalse(objA.Equals(objB));
        }

        [Test]
        public void CreateObject_Equals_EmptyObject()
        {
            var objA = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "a", 1 },
                { "b", "test" },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });

            var objB = new object();

            Assert.IsFalse(objA.Equals(objB));
        }

        [Test]
        public void CreateObject_GetHashCode_EqualObjects()
        {
            var valueDictionary = new Dictionary<string, object>
            {
                { "b", "test" },    
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            };

            var objA = AnonymousTypeUtils.CreateObject(valueDictionary);
            var objB = AnonymousTypeUtils.CreateObject(valueDictionary);

            Assert.AreNotSame(objA, objB);
            Assert.AreEqual(objA.GetHashCode(), objB.GetHashCode());
        }

        [Test]
        public void CreateObject_GetHashCode_NotEqualObjects()
        {
            var objA = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "b", "test" },    
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });
            var objB = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "b", "test2" },    
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });

            Assert.AreNotSame(objA, objB);
            Assert.AreNotEqual(objA.GetHashCode(), objB.GetHashCode());
        }

        [Test]
        public void CreateObject_GetHashCode_SameAsRealAnonymousObject()
        {
            var objA = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "a", 1 },
                { "b", "test" },
                { "c", new DateTime(2014, 1, 1) },
            });
            var objB = new { 
                a = 1, 
                b = "test", 
                c = new DateTime(2014, 1, 1) 
            };

            Assert.AreNotSame(objA, objB);
            Assert.AreEqual(objA.GetHashCode(), objB.GetHashCode());
        }

        [Test]
        public void CreateObject_ToString()
        {
            var obj = AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
            {
                { "b", "test" },    
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
            });
            var expectedString = "{ a = 1, b = test, c = 1/1/2014 12:00:00 AM, d = System.Object }";
            Assert.AreEqual(expectedString, obj.ToString());
        }
    }
}

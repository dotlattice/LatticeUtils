using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LatticeUtils.UnitTests
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

        [TestCase(true)]
        [TestCase(false)]
        public void CreateType(bool isMutable)
        {
            var typeDictionary = new Dictionary<string, Type>
            {
                { "b", typeof(string) },
                { "a", typeof(int) }
            };
            Type anonymousType;
            if (isMutable)
            {
                anonymousType = AnonymousTypeUtils.CreateMutableType(typeDictionary);
            }
            else
            {
                anonymousType = AnonymousTypeUtils.CreateType(typeDictionary);
            }

            Assert.AreEqual(2, anonymousType.GetProperties().Length);

            var propertyA = anonymousType.GetProperty("a");
            Assert.IsNotNull(propertyA);
            Assert.AreEqual(typeof(int), propertyA.PropertyType);
            Assert.IsTrue(propertyA.CanRead);
            Assert.AreEqual(isMutable, propertyA.CanWrite);

            var propertyB = anonymousType.GetProperty("b");
            Assert.IsNotNull(propertyB);
            Assert.AreEqual("b", propertyB.Name);
            Assert.AreEqual(typeof(string), propertyB.PropertyType);
            Assert.IsTrue(propertyB.CanRead);
            Assert.AreEqual(isMutable, propertyB.CanWrite);

            var constructors = anonymousType.GetConstructors();
            Assert.AreEqual(1, constructors.Length);

            var constructorParameters = constructors.Single().GetParameters();
            Assert.AreEqual(2, constructorParameters.Length);

            Assert.AreEqual("b", constructorParameters.ElementAt(0).Name);
            Assert.AreEqual("a", constructorParameters.ElementAt(1).Name);

            string expectedTypeName;
            if (isMutable)
            {
                expectedTypeName = "<>f__LatticeUtilsMutableAnonymousTypeFC2E7EE73C4F69821B908A4DF7585D1018EE7D9A`2";
            }
            else
            {
                expectedTypeName = "<>f__LatticeUtilsAnonymousTypeFC2E7EE73C4F69821B908A4DF7585D1018EE7D9A`2";
            }

            Assert.AreEqual(expectedTypeName, anonymousType.Name);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateGenericTypeDefinition(bool isMutable)
        {
            var typeNames = new[] { "b", "a" };

            Type anonymousGenericTypeDefinition;
            if (isMutable)
            {
                anonymousGenericTypeDefinition = AnonymousTypeUtils.CreateMutableGenericTypeDefinition(typeNames);
            }
            else
            {
                anonymousGenericTypeDefinition = AnonymousTypeUtils.CreateGenericTypeDefinition(typeNames);
            }

            Assert.AreEqual(2, anonymousGenericTypeDefinition.GetProperties().Length);

            var propertyA = anonymousGenericTypeDefinition.GetProperty("a");
            Assert.IsNotNull(propertyA);
            Assert.IsTrue(propertyA.PropertyType.IsGenericParameter);
            Assert.IsTrue(propertyA.CanRead);
            Assert.AreEqual(isMutable, propertyA.CanWrite);

            var propertyB = anonymousGenericTypeDefinition.GetProperty("b");
            Assert.IsNotNull(propertyB);
            Assert.AreEqual("b", propertyB.Name);
            Assert.IsTrue(propertyB.PropertyType.IsGenericParameter);
            Assert.IsTrue(propertyB.CanRead);
            Assert.AreEqual(isMutable, propertyB.CanWrite);

            var constructors = anonymousGenericTypeDefinition.GetConstructors();
            Assert.AreEqual(1, constructors.Length);

            var constructorParameters = constructors.Single().GetParameters();
            Assert.AreEqual(2, constructorParameters.Length);

            Assert.AreEqual("b", constructorParameters.ElementAt(0).Name);
            Assert.AreEqual("a", constructorParameters.ElementAt(1).Name);

            string expectedTypeName;
            if (isMutable)
            {
                expectedTypeName = "<>f__LatticeUtilsMutableAnonymousTypeFC2E7EE73C4F69821B908A4DF7585D1018EE7D9A`2";
            }
            else
            {
                expectedTypeName = "<>f__LatticeUtilsAnonymousTypeFC2E7EE73C4F69821B908A4DF7585D1018EE7D9A`2";
            }

            Assert.AreEqual(expectedTypeName, anonymousGenericTypeDefinition.Name);
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

        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(10)]
        [TestCase(50)]
        [TestCase(1000)]
        public void CreateTypeDefinition_LargeNumberOfProperties(int propertyCount)
        {
            var propertyNames = Enumerable.Range(0, propertyCount).Select(i => "Property" + i.ToString()).ToList();

            var genericTypeDefinition = AnonymousTypeUtils.CreateGenericTypeDefinition(propertyNames);
            Assert.AreEqual(propertyCount, genericTypeDefinition.GetProperties().Length);
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
        public void CreateObject_Equals_EqualObjects_LargeNumberOfProperties()
        {
            var valueDictionary = new Dictionary<string, object>
            {
                { "b", "test" },
                { "a", 1 },
                { "c", new DateTime(2014, 1, 1) },
                { "d", new object() },
                { "e", 1 },
                { "f", 2 },
                { "g", 3 },
                { "h", 4 },
                { "i", 5 },
                { "j", 6 },
                { "k", 7 },
                { "l", 8 },
                { "m", 1 },
                { "n", 2 },
                { "o", 3 },
                { "p", 4 },
                { "q", 5 },
                { "r", 6 },
                { "s", 7 },
                { "t", 8 },
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
            var expectedString = "{ b = test, a = 1, c = 1/1/2014 12:00:00 AM, d = System.Object }";
            Assert.AreEqual(expectedString, obj.ToString());
        }

        [Test]
        public void CreateType_MultipleThreads()
        {
            // Our goal with this test is to have multiple threads creating the same type at the same time.
            // This should help us detect if there are issues with our locking (like in Issue #2).

            // The number of types we create will be iterationCount * threadCount * typeCount
            const int iterationCount = 100;
            const int threadCount = 8;
            const int typeCount = 5;

            for (var i = 0; i < iterationCount; i++)
            {
                // We'll generate the property names ahead of time, and then create one type per property (per task).
                var propertyNames = Enumerable.Range(0, typeCount).Select(k => string.Format("MultiThreadTestProperty{0}.{1}", i, k)).ToList();

                // Create several tasks that will all try to create the same types at once.
                var tasks = new List<Task>(threadCount);
                for (var j = 0; j < threadCount; j++)
                {
                    var task = new TaskFactory().StartNew(() =>
                    {
                        foreach (var propertyName in propertyNames)
                        {
                            AnonymousTypeUtils.CreateObject(new Dictionary<string, object> { { propertyName, "test" } });
                        }
                    });
                    tasks.Add(task);
                }

                // Wait for all of the tasks to finish
                // If anything goes wrong in the above code, the exception will be thrown here.
                Task.WaitAll(tasks.ToArray());
            }
        }
    }
}

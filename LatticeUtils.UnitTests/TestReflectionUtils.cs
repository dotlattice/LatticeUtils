using LatticeUtils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LatticeUtils.UnitTests
{
    public class TestReflectionUtils
    {
        #region Property

        [Test]
        public void Property_StringProperty()
        {
            var propertyInfo = ReflectionUtils.Property<TestClass>(x => x.StringProperty);
            Assert.AreEqual(propertyInfo.Name, "StringProperty");
        }

        [Test]
        public void Property_StringProperty_WithConversion()
        {
            var propertyInfo = ReflectionUtils.Property<TestClass>(x => (string)x.StringProperty);
            Assert.AreEqual(propertyInfo.Name, "StringProperty");
        }

        [Test]
        public void Property_StringProperty_ObjectPropertyTypeParameter_DefaultObject()
        {
            Expression<Func<object>> expression = () => default(TestClass).StringProperty;
            var propertyInfo = ReflectionUtils.Property(expression);
            Assert.AreEqual(propertyInfo.Name, "StringProperty");
        }

        [Test]
        public void Property_StringProperty_ObjectPropertyTypeParameter_NewObject()
        {
            Expression<Func<object>> expression = () => new TestClass().StringProperty;
            var propertyInfo = ReflectionUtils.Property(expression);
            Assert.AreEqual(propertyInfo.Name, "StringProperty");
        }

        [Test]
        public void Property_InternalStringProperty()
        {
            var propertyInfo = ReflectionUtils.Property<TestClass>(x => x.InternalStringProperty);
            Assert.AreEqual(propertyInfo.Name, "InternalStringProperty");
        }

        [Test]
        public void Property_PrivateStringProperty()
        {
            var propertyInfo = ReflectionUtils.Property<TestClass, string>(TestClass.CreatePrivateStringPropertyExpression());
            Assert.AreEqual(propertyInfo.Name, "PrivateStringProperty");
        }

        [Test]
        public void Property_StaticStringProperty()
        {
            var propertyInfo = ReflectionUtils.StaticProperty(() => TestClass.StaticStringProperty);
            Assert.AreEqual(propertyInfo.Name, "StaticStringProperty");
        }

        [Test]
        public void Property_StaticStringProperty_ObjectTypeParameter()
        {
            Expression<Func<object>> expression = () => TestClass.StaticStringProperty;
            var propertyInfo = ReflectionUtils.Property(expression);
            Assert.AreEqual(propertyInfo.Name, "StaticStringProperty");
        }

        [Test]
        public void Property_StringField_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ReflectionUtils.Property<TestClass>(x => x.stringField));
        }

        [Test]
        public void Property_StringMethod_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ReflectionUtils.Property<TestClass>(x => x.StringMethod()));
        }

        [Test]
        public void Property_DifferentChildClassProperty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ReflectionUtils.Property<TestClass>(x => x.ChildTestInnerClass.StringProperty));
        }

        [Test]
        public void Property_ChildPropertyWithSameTypeAsDeclaringType()
        {
            var propertyInfo = ReflectionUtils.Property<TestClass>(x => x.ChildTestClass.StringProperty);
            Assert.AreEqual(propertyInfo.Name, "StringProperty");
        }

        [Test]
        public void Property_IndexerProperty()
        {
            var propertyInfo = ReflectionUtils.Property<string>(x => x[0]);
            Assert.AreEqual(propertyInfo.Name, "Chars");
        }

        #endregion

        #region Field

        [Test]
        public void Field_StringField()
        {
            var fieldInfo = ReflectionUtils.Field<TestClass>(x => x.stringField);
            Assert.AreEqual(fieldInfo.Name, "stringField");
        }

        [Test]
        public void Field_StringField_StringReturnType()
        {
            var fieldInfo = ReflectionUtils.Field<TestClass, string>(x => x.stringField);
            Assert.AreEqual(fieldInfo.Name, "stringField");
        }

        [Test]
        public void Field_StringField_ObjectReturnType()
        {
            var fieldInfo = ReflectionUtils.Field<TestClass, object>(x => x.stringField);
            Assert.AreEqual(fieldInfo.Name, "stringField");
        }

        [Test]
        public void Field_StringField_ObjectFieldTypeParameter_DefaultObject()
        {
            Expression<Func<object>> expression = () => default(TestClass).stringField;
            var fieldInfo = ReflectionUtils.Field(expression);
            Assert.AreEqual(fieldInfo.Name, "stringField");
        }

        [Test]
        public void Field_StringField_ObjectFieldTypeParameter_NewObject()
        {
            Expression<Func<object>> expression = () => new TestClass().stringField;
            var fieldInfo = ReflectionUtils.Field(expression);
            Assert.AreEqual(fieldInfo.Name, "stringField");
        }

        [Test]
        public void Field_StringProperty_WithConversion()
        {
            var fieldInfo = ReflectionUtils.Field<TestClass>(x => (string)x.stringField);
            Assert.AreEqual(fieldInfo.Name, "stringField");
        }

        [Test]
        public void Field_InternalStringField()
        {
            var fieldInfo = ReflectionUtils.Field<TestClass>(x => x.internalStringField);
            Assert.AreEqual(fieldInfo.Name, "internalStringField");
        }

        [Test]
        public void Field_PrivateStringField()
        {
            var propertyInfo = ReflectionUtils.Field<TestClass, string>(TestClass.CreatePrivateStringFieldExpression());
            Assert.AreEqual(propertyInfo.Name, "privateStringField");
        }

        [Test]
        public void Field_StaticStringField()
        {
            var fieldInfo = ReflectionUtils.StaticField(() => TestClass.staticStringField);
            Assert.AreEqual(fieldInfo.Name, "staticStringField");
        }

        [Test]
        public void Field_StaticStringField_ObjectFieldTypeParameter()
        {
            Expression<Func<object>> expression = () => TestClass.staticStringField;
            var fieldInfo = ReflectionUtils.Field(expression);
            Assert.AreEqual(fieldInfo.Name, "staticStringField");
        }

        [Test]
        public void Field_StringProperty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ReflectionUtils.Field<TestClass>(x => x.StringProperty));
        }

        [Test]
        public void Field_StringMethod_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ReflectionUtils.Field<TestClass>(x => x.StringMethod()));
        }

        #endregion

        #region Method

        [Test]
        public void Method_StringMethod()
        {
            var methodInfo = ReflectionUtils.Method<TestClass>(x => x.StringMethod());
            Assert.AreEqual(methodInfo.Name, "StringMethod");
        }

        [Test]
        public void Method_StringMethod_StringReturnType()
        {
            var methodInfo = ReflectionUtils.Method<TestClass, string>(x => x.StringMethod());
            Assert.AreEqual(methodInfo.Name, "StringMethod");
        }

        [Test]
        public void Method_StringMethod_ObjectReturnType()
        {
            var methodInfo = ReflectionUtils.Method<TestClass, object>(x => x.StringMethod());
            Assert.AreEqual(methodInfo.Name, "StringMethod");
        }

        [Test]
        public void Method_StringMethod_ActionExpression()
        {
            Expression<Action<TestClass>> expression = (x) => x.StringMethod();
            var methodInfo = ReflectionUtils.Method(expression);
            Assert.AreEqual(methodInfo.Name, "StringMethod");
        }

        [Test]
        public void Method_StringMethod_NonGenericActionExpression_DefaultObject()
        {
            Expression<Action> expression = () => default(TestClass).StringMethod();
            var methodInfo = ReflectionUtils.Method(expression);
            Assert.AreEqual(methodInfo.Name, "StringMethod");
        }

        [Test]
        public void Method_StringMethod_NonGenericActionExpression_NewObject()
        {
            Expression<Action> expression = () => new TestClass().StringMethod();
            var methodInfo = ReflectionUtils.Method(expression);
            Assert.AreEqual(methodInfo.Name, "StringMethod");
        }

        [Test]
        public void Method_StringMethod_WithConversion()
        {
            var methodInfo = ReflectionUtils.Method<TestClass>(x => (string)x.StringMethod());
            Assert.AreEqual(methodInfo.Name, "StringMethod");
        }

        [Test]
        public void Method_VoidMethod()
        {
            var methodInfo = ReflectionUtils.Method<TestClass>(x => x.VoidMethod());
            Assert.AreEqual(methodInfo.Name, "VoidMethod");
        }

        [Test]
        public void Method_StaticStringMethod_NonGenericActionExpression()
        {
            Expression<Action> expression = () => TestClass.StaticStringMethod();
            var methodInfo = ReflectionUtils.Method(expression);
            Assert.AreEqual(methodInfo.Name, "StaticStringMethod");
        }

        [Test]
        public void StaticMethod_StaticStringMethod()
        {
            var methodInfo = ReflectionUtils.StaticMethod(() => TestClass.StaticStringMethod());
            Assert.AreEqual(methodInfo.Name, "StaticStringMethod");
        }

        [Test]
        public void Method_ExtensionMethod_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ReflectionUtils.Method<string>(x => x.Count()));
        }

        [Test]
        public void StaticMethod_ExtensionMethod()
        {
            var methodInfo = ReflectionUtils.StaticMethod(() => default(string).Count());
            Assert.AreEqual(methodInfo.Name, "Count");
            Assert.AreEqual(typeof(System.Linq.Enumerable), methodInfo.DeclaringType);
            Assert.IsTrue(methodInfo.IsStatic);
        }

        [Test]
        public void StaticMethod_ExtensionMethodSelect()
        {
            var methodInfo = ReflectionUtils.StaticMethod(() => new int[0].Select((x, i) => ""));
            Assert.AreEqual(methodInfo.Name, "Select");
            Assert.AreEqual(typeof(System.Linq.Enumerable), methodInfo.DeclaringType);
            Assert.IsTrue(methodInfo.IsStatic);
            Assert.IsFalse(methodInfo.IsGenericMethodDefinition);

            var genericArguments = methodInfo.GetGenericArguments();
            Assert.AreEqual(2, genericArguments.Length);
            Assert.AreEqual(typeof(int), genericArguments.ElementAt(0));
            Assert.AreEqual(typeof(string), genericArguments.ElementAt(1));
        }
 
        #endregion

        #region Constructor

        [Test]
        public void Constructor_Default()
        {
            var constructorInfo = ReflectionUtils.Constructor(() => new TestClass());
            Assert.AreEqual(constructorInfo.Name, ".ctor");
            Assert.AreEqual(0, constructorInfo.GetParameters().Length);
        }

        [Test]
        public void Constructor_Default_ActionExpression()
        {
            Expression<Action> expression = () => new TestClass();
            var constructorInfo = ReflectionUtils.Constructor(expression);
            Assert.AreEqual(constructorInfo.Name, ".ctor");
            Assert.AreEqual(0, constructorInfo.GetParameters().Length);
        }

        [Test]
        public void Constructor_OneParameter()
        {
            var constructorInfo = ReflectionUtils.Constructor(() => new TestClass(string.Empty));
            Assert.AreEqual(constructorInfo.Name, ".ctor");
            Assert.AreEqual(1, constructorInfo.GetParameters().Length);
        }

        [Test]
        public void Constructor_TwoParameters()
        {
            var constructorInfo = ReflectionUtils.Constructor(() => new TestClass(string.Empty, 0));
            Assert.AreEqual(constructorInfo.Name, ".ctor");
            Assert.AreEqual(2, constructorInfo.GetParameters().Length);
        }


        #endregion

        #region Test Class

        private class TestClass
        {
            #region Constructors

            public TestClass() { }
            public TestClass(string stringParam) { }
            public TestClass(string stringParam, int intParam) { }

            #endregion

            #region Properties

            public string StringProperty { get; set; }
            public static string StaticStringProperty { get; set; }

            internal string InternalStringProperty { get; set; }
            private string PrivateStringProperty { get; set; }
            public static Expression<Func<TestClass, string>> CreatePrivateStringPropertyExpression()
            {
                return CreateExpression<TestClass, string>(x => x.PrivateStringProperty);
            }

            #endregion

            #region Fields

            public string stringField;
            public static string staticStringField;

            internal string internalStringField;
            private string privateStringField;
            public static Expression<Func<TestClass, string>> CreatePrivateStringFieldExpression()
            {
                return CreateExpression<TestClass, string>(x => x.privateStringField);
            }

            #endregion

            #region Methods

            public string StringMethod() { return null; }
            public string StringMethodWithParameter(int index) { return null; }
            public void VoidMethod() { }

            public static string StaticStringMethod() { return null; }
            public static string StaticStringMethodWithParameter(int index) { return null; }
            public static void StaticMethodWithoutReturn() { }

            #endregion

            #region Other

            public TestClass ChildTestClass { get; set; }
            public TestInnerClass ChildTestInnerClass { get; set; }

            private static Expression<Func<TestClass, TPropertyType>> CreateExpression<TEntity, TPropertyType>(Expression<Func<TestClass, TPropertyType>> e) { return e; }

            #endregion
        }

        private class TestInnerClass
        {
            public string StringProperty { get; set; }
        }

        #endregion
    }
}

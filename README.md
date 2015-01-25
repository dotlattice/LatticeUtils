# LatticeUtils [![Build status](https://ci.appveyor.com/api/projects/status/y6ya4k6gqltw7942)](https://ci.appveyor.com/project/dotlattice/latticeutils)

LatticeUtils is a lightweight .NET library that provides some basic utility methods.

## What Does It Do?

Basically this library is a collection of static classes that fill in some gaps in what the standard library provides.

* [ReflectionUtils](#reflectionutils) - methods that let you find members of a class using strongly typed expressions instead of string literals
* [ParseUtils](#parseutils) - a generic parse method and methods for parsing value types in one line
* [ConvertUtils](#convertutils)- a more comprehensive version of Convert.ChangeType
* [StreamUtils](#streamutils) - methods that operate on streams, including methods for converting TextReaders to streams
* [CollectionUtils](#collectionutils) - provide methods equivalent to the ones in the List&lt;T&gt; class for any Collection&lt;T&gt;
* [AnonymousTypeUtils](#anonymoustypeutils) - generate anonymous types dynamically at runtime


## Installation

There are several ways to install this library:

* Install the [NuGet package](https://www.nuget.org/packages/LatticeUtils.Core/)
* Download the assembly from the [latest release](https://github.com/dotlattice/LatticeUtils/releases/latest) and install it manually
* Copy the parts you want into your own project

This entire library is released into the [public domain](https://github.com/dotlattice/LatticeUtils/blob/master/LICENSE.md).  So you can copy anything from this library into your own project without having to worry about attribution or any of that stuff.


## ReflectionUtils

This class just consists of methods that accept expressions and return member metadata (PropertyInfo, MethodInfo, FieldInfo, or ConstructorInfo).

So why is this useful?  One of the best things is that it allows for strong references to this metadata instead of having to use a literal string.

For example, with the standard .NET library you would have to do something like this to get the Length property on the string class: 

```c#
typeof(string).GetProperty("Length")
```

With ReflectionUtils, you can use an expression to reference the property like this:

```c#
ReflectionUtils.Property<string>(s => s.Length)
```

With the string literal, if that name is wrong you will get a runtime error when that code is hit.  With the expression, you'll get a compile time error if it's wrong, and you can also get intellisense help to make it easier to get the name right in the first place.

Another thing that is much easier with expressions is when you need to find a method that has a lot of overloads or generic type parameters.  For example, finding a specific version the Enumerable.Select method is very simple with this code: 

```c#
ReflectionUtils.StaticMethod(() => new int[0].Select((x, i) => ""))
```

### Use of Expressions

The key to these helper methods is that they all accept [expressions](http://msdn.microsoft.com/en-us/library/bb397951.aspx) as a parameter.  So if you use ReflectionUtils to get a method like this:

```c#
ReflectionUtils.Method<string>(s => s.IndexOf(null, -1, -1)
```

... the IndexOf method is not actually being called.  

Instead, the compiler creates an expression tree with information about how to call the IndexOf method.  Then the helper method can use that expression tree to find the MethodInfo that you requested.


### Examples

```c#
FieldInfo emptyField = ReflectionUtils.StaticField<string>(() => string.Empty);
```
```c#
MethodInfo substringMethod = ReflectionUtils.Method<string>(s => s.Substring(0, 0));
```
```c#
MethodInfo countExtensionMethod = ReflectionUtils.StaticMethod(() => default(string).Count());
```
```c#
ConstructorInfo charArrayConstructor = ReflectionUtils.Constructor<string>(() => new String(default(char[])));
```
```c#
PropertyInfo indexedProperty = ReflectionUtils.Property<string>(x => x[0]);
```



## ParseUtils

The focus of this class is making it easier to call the standard parse methods.  It doesn't add any new functionality that isn't in the standard libraries, just an alternative API for stuff that is already there.


### Generic Type Conversion

There is already a great way to parse strings into just about any type using TypeConverters and the [TypeDescriptor.GetConverter](http://www.hanselman.com/blog/TypeConvertersTheresNotEnoughTypeDescripterGetConverterInTheWorld.aspx) method.

So why not just use TypeConverters directly?  Several reasons:

* They are kind of complicated to use.  You need to know that TypeDescriptor exists, and then once you get a converter you need to know which of the 14 Convert methods you should call.
* The convert methods all return objects, so you have to explicitly cast the result to the type you want.
* There is no TryConvert method, so a parse failure will always throw an exception.
* The exception thrown when a parse fails is often just a plain Exception instead of a FormatException or something else that can be caught without catching every possible exception.


For example, if you want to try to parse a string to a Guid you might have to do something like this:
```c#
Guid? g;
try {
	g = (Guid?)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Guid?)).ConvertFromInvariantString("not a guid");
}
catch (Exception) {
	g = default(Guid?); 
}
```

It's much simpler to do this with the ParseUtils version:

```c#
Guid? g = ParseUtils.TryParse<Guid?>("not a guid");
```

### Examples

```c#
ParseUtils.TryParse<int?>("2")
```
```c#
Guid g = ParseUtils.Parse<Guid>("2A053076-EA0F-4619-9DA3-2AB919A7609C")
```


### TryParse Methods

All of the numeric types have a TryParse method, like:

```c#
int i;
int.TryParse("2", out i); 
```

These methods work great, but that output parameter can make them annoying to use.  This is especially true when you want to parse to a nullable value, because the output parameter can't be nullable.  So them you're stuck with a temporary variable that you don't care about:

```c#
int? i;
int temp;
i = int.TryParse("2", out temp) ? temp : default(int?); 
```

So the goal of the TryParse methods in the ParseUtils class is to provide a version of these methods that work without output parameters:

```c#
int? i = ParseUtils.TryParseNullableInt("2");
```

All of these ParseUtils methods are for parsing to nullable values, but they can still be used to parse to non-nullable values in a single line when combined with the null-coalescing operator:

```c#
int i = ParseUtils.TryParseNullableInt("not a number") ?? 0;
```

## ConvertUtils

This is similar to ParseUtils, but the input value can be anything instead of just a string.  It's basically a more advanced version of the standard [Convert.ChangeType](http://msdn.microsoft.com/en-us/library/dtb69x08.aspx).

Convert.ChangeType is a great method for what it does, but it has a fairly limited range of use.  It mostly just works for converting between the different numeric types, and it only works with classes that implement IConvertible.

A combination of Convert.ChangeType and the .NET TypeConverters (as used in the ParseUtils class) can handle converting between a lot more types.  So ConvertUtils basically decides which of these methods to call based on the input and output types, along with a few custom checks for things like when no type conversion is necessary.

### Examples

```c#
ConvertUtils.ChangeType(1, typeof(decimal))
```
```c#
ConvertUtils.ChangeType("OrdinalIgnoreCase", typeof(StringComparison))
```
```c#
ConvertUtils.ChangeType(5, typeof(StringComparison))
```


## StreamUtils

This class just has a few methods to help with reading from [Streams](http://msdn.microsoft.com/en-us/library/system.io.stream.aspx).

### Converting TextReaders

The standard .NET library has a great way to convert a Stream to a TextReader using the [StreamReader](http://msdn.microsoft.com/en-us/library/vstudio/system.io.streamreader) class.  But it's missing the reverse of that, a way of converting an arbitrary TextReader to a Stream.

So the primary goal of the StreamUtils class is to provide that, a way to convert any TextReader to a Stream:
  
```c#
using (var stream = StreamUtils.FromTextReader(new StringReader(""))) {}
```

A TextReaderStream class is provided to handle this conversion, and that's what the FromTextReader methods will return if no better way of converting is found.  

So why not use TextReaderStream directly?  Because for some readers there may be a better way.  For example, a StreamReader has a property for getting its underlying stream, so the static method can return the underlying stream directly instead of creating an extra wrapper.


### Reading Everything

The TextReader class provides a ReadToEnd method, but there isn't an equivalent method for Streams.  To fix that, StreamUtils has two: 

* ReadToEnd - reads everything from the current position to the end of a Stream into a byte array (just like the TextReader version, except with bytes instead of chars)
* ReadAllBytes - reads everything in the stream no matter what the current position (like the MemoryStream.ToArray method).

### Examples
```c#
StreamUtils.FromTextReader(new StringReader("test"))
```
```c#
StreamUtils.ReadAllBytes(new MemoryStream(new byte[] { 1, 2, 3}))
```
```c#
StreamUtils.ReadToEnd(new MemoryStream(new byte[] { 1, 2, 3}))
```
 

## CollectionUtils

The generic List&lt;&gt; class gets all kinds of great methods that aren't part of any other interface or extension method class.  But what about LinkedList?  What about SortedList?  Why shouldn't these methods be available to any of the other collection classes?

So the goal in CollectionUtils is just to provide implementations those List methods that work with any ICollection&lt;&gt; (or IEnumerable&lt;&gt; where it makes sense).

### List Methods

These methods are all designed to work just like the equivalent method in the [List&lt;T&gt;](http://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx) class.

These methods work on any ICollection&lt;&gt;:

* AddRange
* RemoveAt
* RemoveAll
* RemoveRange
* AsReadOnly

And these work on any IEnumerable&lt;&gt;:

* IndexOf
* FindIndex
* CopyTo
* ForEach

### Examples

```c#
CollectionUtils.RemoveAt(new LinkedList<int>(new[] { 1, 2, 3}), 1)
```
```c#
CollectionUtils.IndexOf(new[] { 10, 20, 30 }, 10)
```



## AnonymousTypeUtils

This is probably the fanciest class in this library, but it will probably only have a few niche uses.

The built-in .NET [anonymous types](http://msdn.microsoft.com/en-us/library/bb397696.aspx) work by generating new classes at compile time:

```c#
new { a = 1, b = 2 }
```

These anonymous classes are great for all kinds of uses, but one limitation is that you have to know what properties you want the anonymous type to have at compile time.  I've come across a few situations where I need to generate an anonymous type at runtime, and there's no built-in way of doing that.

So that's what this AnonymousTypeUtils class is for.  At runtime, it can create a new type that works just like an anonymous type (including the Equals/GetHashcode and ToString methods).  Here's an example of creating that same anonymous object as above at runtime:

```c#
AnonymousTypeUtils.CreateObject(new Dictionary<string, object>
{
	{ "a", 1 },
    { "b", 2 }
});
```

### Why?

Why would anyone want to do this?  Why not just use a Dictionary&lt;&gt; or ExpandoObject instead?

The downside to an ExpandoObject or dictionary is that you can't use reflection to get the list of properties on the type.  So if you're using a library like EntityFramework that needs to use reflection on your object, you need an anonymous type.

Working with [EntityFramework](http://msdn.microsoft.com/en-us/data/ef.aspx) is actually the case that led to the creation of this class.  In EntityFramework, you can use anonymous objects in a LINQ query to limit the columns that are included in the generated SQL statement:

```c#
from product in context.Products
select new
{
    ProductId = product.ProductID,
    ProductName = product.Name
};
```

That's a great feature if you know what columns you need in advance.  But if you want the columns to be configurable or be based on user input, then you can't use a normal anonymous object for this.  However, you can use the AnonymousTypeUtils dynamically generated anonymous objects!

### Examples
Here's an example of a method that selects some properties from an IQueryable&lt;&gt; using a list of strings to pick the properties:

```c#
public IQueryable SelectProperties<T>(IQueryable<T> queryable, ISet<string> propertyNames)
{
	var properties = typeof(T).GetProperties().Where(p => propertyNames.Contains(p.Name));

	var entityParameterExpression = Expression.Parameter(typeof(T));
	var propertyExpressions = properties.Select(p => Expression.Property(entityParameterExpression, p));

	var anonymousType = AnonymousTypeUtils.CreateType(properties.ToDictionary(p => p.Name, p => p.PropertyType));
	var anonymousTypeConstructor = anonymousType.GetConstructors().Single();
	var anonymousTypeMembers = anonymousType.GetProperties().Cast<MemberInfo>().ToArray();

	// It's important to include the anonymous type members in the New expression, otherwise EntityFramework 
	// won't recognize this as the constructor of an anonymous type.
	var anonymousTypeNewExpression = Expression.New(anonymousTypeConstructor, propertyExpressions, anonymousTypeMembers);

	var selectLambdaMethod = GetExpressionLambdaMethod(entityParameterExpression.Type, anonymousType);
	var selectBodyLambdaParameters = new object[] { anonymousTypeNewExpression, new[] { entityParameterExpression } };
	var selectBodyLambdaExpression = (LambdaExpression)selectLambdaMethod.Invoke(null, selectBodyLambdaParameters);

	var selectMethod = GetQueryableSelectMethod(typeof(T), anonymousType);
	var selectedQueryable = selectMethod.Invoke(null, new object[] { queryable, selectBodyLambdaExpression }) as IQueryable;
	return selectedQueryable;
}
``` 


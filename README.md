# Xunit.Extensions.Ordering
Xunit extension for ordered (integration) testing

Nuget: https://www.nuget.org/packages/Xunit.Extensions.Ordering/

There is very limited posibility to support ordered (integration) testing in xunit without rewriting runner. 

Usage:

1. Add AssemblyInfo with only this lines of code

```c#
using Xunit;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
```

2. Add Order Attribute to test cases (classes) and facts (methods). Tests are executed in ascending order. If no order is specified default 0 is assigned. Multiple Order attributes can have same value. Their execution order in this case is deterministic but unpredictible.

```c#
[Order(1)]
public class TC2
{
	[Fact, Order(2)]
	public void M1() { Assert.Equal(2, Counter.Next()); }

	[Fact, Order(3)]
	public void M2() { Assert.Equal(3, Counter.Next()); }

	[Fact, Order(1)]
	public void M3() { Assert.Equal(1, Counter.Next()); }
}
```

3. There are limitations when you need to use collections. You have to use collection per class like in the sample bottom bcs. of litimations of Xunit (you cannot order test cases in a collection without massive rewrite of runner infrastructure of xunit)
```c#
[CollectionDefinition("COL1"), Collection("COL1"), Order(3)]
public class TC1
{
	[Fact, Order(2)]
	public void M1() { Assert.Equal(...); }

	[Fact, Order(3)]
	public void M2() { Assert.Equal(...); }

	[Fact, Order(1)]
	public void M3() { Assert.Equal(...); }
}
```
If you need to split facts into multiple test clases use partial class :-) Finally following this design there is no real difference between CollectionFixture and ClassFixture :-( 

4. If you need assembly level Fixtures in both scenarios use this https://github.com/xunit/samples.xunit/tree/master/AssemblyFixtureExample :-)

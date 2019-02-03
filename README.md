# Xunit.Extensions.Ordering
Xunit extension for ordered (integration) testing

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

2. Use Collection per Class bcs. of litimations of Xunit (you cannot order test classes in collection). Add Order Attribute to Collection/TestClass to set ordering at collection level. Add Order Attribute to Method/Fact to set ordering at Method/Fact level.
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
3. If you need to split facts into multiple test clases use partial class :-) Finally following this design there is no real difference between CollectionFixture and ClassFixture :-( If you need assembly level Fixtures use this https://github.com/xunit/samples.xunit/tree/master/AssemblyFixtureExample :-)

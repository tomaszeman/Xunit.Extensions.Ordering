# Xunit.Extensions.Ordering
Xunit extension that provides full support for ordering at all levels - test collections, test classes and test cases.
Extension provides full-featured AssemblyFixture implementation with same functionality as class and collection fixtures (including IMessageSink injection, support for IAsyncLifetime). 
Supports .NET Core 1.x, .NET Core 2.x. and may work in .NET 4.5.2+

The common scenarion where ordering is useful is integration testing if you cannot or you don't want to make each test method atomic. 

## Table of contents

**Nuget:** https://www.nuget.org/packages/Xunit.Extensions.Ordering/

1. [Test cases ordering](#test-cases-ordering)
   1. [Setup ordering](#setup-ordering)
   2. [Ordering test classes and cases](#ordering-test-classes-and-cases)
   3. [Ordering test classes in collection](#ordering-test-classes-in-collection)
   4. [Ordering test collection](#ordering-test-collection)
   5. [Mixing test classes in collections and test classes without explicit collection assignement](#mixing-test-classes-in-collections-and-test-classes-without-explicit-collection-assignement)
   6. [Notes](#notes)
2. [AssemblyFixture](#assemblyFixture)
   1. [Setup Fixture](#setup-fixture)
   2. [Basic usage](#basic-usage)
   3. [Multiple assembly fixtures](#multiple-assembly-fixtures)
   4. [IAsyncLifetime](#iasyncLifetime)
   5. [IAssemblyFixture\<TFixture\>](#iassemblyfixturetfixture)
   5. [Notes about AssemblyFixture implementation](#notes-about-assemblyfixture)

## Test cases ordering

### Setup ordering  

Add *AssemblyInfo.cs* with only following lines of code

```csharp
using Xunit;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
```

### Ordering test classes and cases

Add `Order` Attribute to test classes and methods. Tests are executed in ascending order. If no `Order` attribute is specified default 0 is assigned. Multiple `Order` attributes can have same value. Their execution order in this case is deterministic but unpredictible.

```csharp
[Order(1)]
public class TC2
{
	[Fact, Order(2)]
	public void M1() { /* ... */ }

	[Fact, Order(3)]
	public void M2() { /* ... */ }

	[Fact, Order(1)]
	public void M3() { /* ... */ }
}
```

### Ordering test classes in collection  

You can order test classes in collections by adding `Order` attribute too but you have to use patched test framework by add following line to AssemblyInfo.cs

```csharp
using Xunit;

[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]
```

Now you can order collections like this

```csharp
[CollectionDefinition("C1")]
public class Collection1 { }
```
```csharp
[Collection("C1"), Order(2)]
public class TC3
{
	[Fact, Order(1)]
	public void M1() { /* 3 */ }

	[Fact, Order(2)]
	public void M2() { /* 4 */ }
}
```
```csharp
[Collection("C1"), Order(1)]
public partial class TC5
{
	[Fact, Order(2)]
	public void M1() { /* 2 */ }

	[Fact, Order(1)]
	public void M2() { /* 1 */ }

}
```

### Ordering test collection  

You can order test collections by adding `Order` attribute too definition collection class

```csharp
[CollectionDefinition("C1"), Order(3)]
public class Collection3 { }
```
 ```csharp
[CollectionDefinition("C2"), Order(1)]
public class Collection3 { }
```

### Mixing test classes in collections and test classes without explicit collection assignement

Test classes without explicitely assigned collection are collections implicitely in Xunit (collection per class). So if you mix test classes with assigned collection and test classes without assigned collection they are on the same level and `Order` is applied following this logic.  

```csharp
[CollectionDefinition("C1"), Order(3)]
public class Collection3 { }
```
```csharp
[CollectionDefinition("C2"), Order(1)]
public class Collection3 { }
```
```csharp
[Order(2)]
public class TC2
{
	[Fact]
	public void M1() { /* 4 */ }
}
```
```csharp
[Collection("C1")]
public class TC3
{
	[Fact]
	public void M1() { /* 5 */ }
}
```
```csharp
[Collection("C2"), Order(2)]
public partial class TC5
{
	[Fact]
	public void M1() { /* 3 */ }
}
```
```csharp
[Collection("C2"), Order(1)]
public partial class TC5
{
	[Fact, Order(2)]
	public void M1() { /* 2 */ }

	[Fact, Order(1)]
	public void M2() { /* 1 */ }
}
```

### Checking continuity of order indexes and detection of duplicates

You can enable warning messages about continuity and duplicates of Order indexes by enabling `diagnosticMessages`.
 
1. Create `xnuit.runner.json` in root of your test project 
	
```json
{
	"$schema": "https://xunit.github.io/schema/current/xunit.runner.schema.json",
	"diagnosticMessages": true
}
```
	
2. Set *"Copy to output directory"* for this file in visual studio to *"Copy if newer"*
3. In the *Output* Visual Studio window choose *"Tests"* option in the *"Show output from"* dropdown or just run *dotnet test* from *Package Manager Console*
4. You will see warnings like 
	
```text
Missing test collection order sequence from '4' to '39'.
Missing test case order '1' in test class 'Xunit.Extensions.Ordering.Tests.TC6'.
Missing test classes order sequence from '3' to '29' for collection 'C1'.
Missing test case order sequence from '2' to '19' in test class 'Xunit.Extensions.Ordering.Tests.TC5'.
 ```
### Notes

There is no guarantee of order for `Theory` test method invocation but this is expected behavior.

```csharp
[Theory, Order(4)]
[InlineData(15)]
[InlineData(16)]
[InlineData(17)]
public void Method(int expectedOrder) { Assert.Equal(expectedOrder, Counter.Next()); }
```
## AssemblyFixture

Assembly fixtures are instantiated ones per test run. Assembly fixtures fully support `IAsyncLifetime` interface, injection of `IMessageSink`.

### Setup fixture

Add `AssemblyInfo.cs` with only following lines of code

```csharp
using Xunit;
using Xunit.Extensions.Ordering;

[assembly: AssemblyFixture(typeof(AsmFixture1))]
[assembly: AssemblyFixture(typeof(AsmFixture2))]
```

### Basic usage

```csharp
public class TC
{
	private readonly AsmFixture1 _fixture;

	public TC(AsmFixture1 fixture)
	{
		_fixture = fixture;
	}
}
```

### Multiple assembly fixtures

```csharp
public class TC
{
	private readonly AsmFixture1 _fixture1;
	private readonly AsmFixture2 _fixture2;
	private readonly ITestOutputHelper _output;

	public TC(AsmFixture1 fixture1, ITestOutputHelper output, AsmFixture2 fixture2)
	{
		_fixture1 = fixture1;
		_fixture2 = fixture2;
		_output = output;
	}
}
```

### IAsyncLifetime

```csharp
public class AsmFixture : IAsyncLifetime
{
	public IMessageSink MesssageSink { get; }
	public bool Initialized { get; private set; } = false;

	public AsmFixture(IMessageSink messsageSink)
	{
		MesssageSink = messsageSink;
	}

	public async Task InitializeAsync()
	{
		await Task.Run(() => { Initialized = true; });
	}

	public async Task DisposeAsync()
	{
		await Task.Run(
			() => MesssageSink.OnMessage(
				new DiagnosticMessage("Disposed async.")));
	}
}
```
### IAssemblyFixture\<TFixture\>

You can use `IAssemblyFixture<TFixture>` as marekr interface. Assembly fixtures are currently injected to constructor regardless of this interface. I will add later option for smart resolving and instantiation of assembly fixtures only required by target set of test cases.

```csharp
public class TC : 
	IAssemblyFixture<AsmFixture1>,
	IAssemblyFixture<AsmFixture2>
{
	private readonly AsmFixture1 _fixture1;
	private readonly AsmFixture2 _fixture2;

	public TC(AsmFixture1 fixture1, AsmFixture2 fixture2)
	{
		_fixture1 = fixture1;
		_fixture2 = fixture2;
	}
}
```

### Notes about AssemblyFixture 

The reason why I don't split this functionality into two packages is that I need to rewrite TestFramework for ordering puposes and AssemblyFixtures are often used side by side with ordering and integration testing. 

*Kick started by [Xunit example](https://github.com/xunit/samples.xunit/tree/master/AssemblyFixtureExample) by Brad Wilson. I've presered his original comments where it was applicable.*

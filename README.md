# Xunit.Extensions.Ordering
Xunit extension that provides full support for ordering at all levels - **test collections**, **test classes** and **test cases**. Integration testing is the common scenario where ordering is useful.

Extension also provides full-featured **AssemblyFixture** implementation with same functionality as class and collection fixtures (including IMessageSink injection, support for IAsyncLifetime). 

**Supports:** *.NET Core 1.x, .NET Core 2.x.* and *.NET 4.5.2+*

**Nuget:** https://www.nuget.org/packages/Xunit.Extensions.Ordering/

## Table of contents

1. [Test cases ordering](#test-cases-ordering)
   1. [Setup ordering](#setup-ordering)
   2. [Ordering classes and cases](#ordering-classes-and-cases)
   3. [Ordering classes in collection](#ordering-classes-in-collection)
   4. [Ordering collections](#ordering-collections)
   5. [Mixing test classes with and without explicit collection assignement](#mixing-test-classes-with-and-without-explicit-collection-assignement)
   6. [Checking continuity and duplicates](#checking-continuity-and-duplicates)
   7. [Notes](#notes)
2. [AssemblyFixture](#assemblyFixture)
   1. [Setup Fixture](#setup-fixture)
   2. [Basic usage](#basic-usage)
   3. [Multiple assembly fixtures](#multiple-assembly-fixtures)
   4. [IAsyncLifetime](#iasynclifetime)
   5. [Notes about AssemblyFixture implementation](#notes-about-assemblyfixture)

## Test cases ordering
### Setup ordering  
Add `AssemblyInfo.cs` with only following lines of code
```csharp
using Xunit;
//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
```
### Ordering classes and cases
Add `Order` attribute to test classes and methods. Tests are executed in ascending order. If no `Order` attribute is specified default 0 is assigned. Multiple `Order` attributes can have same value. Their execution order is in this case deterministic but unpredictible.
```csharp
[Order(1)]
public class TC2
{
	[Fact, Order(2)]
	public void M1() { /*...*/ }

	[Fact, Order(3)]
	public void M2() { /*...*/ }

	[Fact, Order(1)]
	public void M3() { /*...*/ }
}
```
### Ordering classes in collection  
You can order test classes in collections by adding `Order` attribute but you have to use patched test framework by adding following lines to `AssemblyInfo.cs`
```csharp
using Xunit;

[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]
```
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

[Collection("C1"), Order(1)]
public partial class TC5
{
	[Fact, Order(2)]
	public void M1() { /* 2 */ }

	[Fact, Order(1)]
	public void M2() { /* 1 */ }
}
```
### Ordering collections  
You can order test collections by adding `Order` attribute too definition collection class
```csharp
[CollectionDefinition("C1"), Order(3)]
public class Collection3 { }

[CollectionDefinition("C2"), Order(1)]
public class Collection3 { }
```
### Mixing test classes with and without explicit collection assignement
Test classes without explicitely assigned collection are collections implicitely in Xunit (collection per class).
If you mix both types of collections they are on the same level and `Order` is applied following this logic.  
```csharp
[CollectionDefinition("C1"), Order(3)]
public class Collection3 { }

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

[Collection("C1")]
public class TC3
{
	[Fact]
	public void M1() { /* 5 */ }
}

[Collection("C2"), Order(2)]
public partial class TC5
{
	[Fact]
	public void M1() { /* 3 */ }
}

[Collection("C2"), Order(1)]
public partial class TC5
{
	[Fact, Order(2)]
	public void M1() { /* 2 */ }

	[Fact, Order(1)]
	public void M2() { /* 1 */ }
}
```
### Checking continuity and duplicates
You can enable warning messages about continuity and duplicate order indexes.
1. Create `xnuit.runner.json` file in root of your test project 
```json
{
	"$schema": "https://xunit.github.io/schema/current/xunit.runner.schema.json",
	"diagnosticMessages": true
}
```
2. Set *"Copy to output directory"* for this file to *"Copy if newer"*
3. In the *Output* window choose *"Tests"* option in the *"Show output from"* dropdown or just run *dotnet test* from *Package Manager Console*
4. You'll start getting warnings like 
```text
Missing test collection order sequence from '4' to '39'.
Missing test case order '1' in test class 'Xunit.Extensions.Ordering.Tests.TC6'.
Missing test classes order sequence from '3' to '29' for collection 'C1'.
Missing test case order sequence from '2' to '19' in test class 'Xunit.Extensions.Ordering.Tests.TC5'.
 ```
### Notes
There is no guarantee for `Theory` method execution order what is expected behavior.
```csharp
[Theory, Order(4)]
[InlineData(15)]
[InlineData(16)]
[InlineData(17)]
public void Method(int expectedOrder) { Assert.Equal(expectedOrder, Counter.Next()); }
```
## AssemblyFixture
Assembly fixtures are instantiated ones per test run. Assembly fixtures fully support `IAsyncLifetime` interface, injection of `IMessageSink`.
There are two ways how register fixtures - using `AssemblyFixture` attribute at assembly level or by using `IAssemblyFixture<TFixture>` interface at test class level.
You can mix both approaches but I strongly recommend `IAssemblyFixture<TFixture>` interface way.
### Basic usage
#### Using AssemblyFixture attribute
```csharp
[assembly: AssemblyFixture(typeof(AssFixture1))]
[assembly: AssemblyFixture(typeof(AssFixture2), typeof(AssFixture3))]
```
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
#### Using `IAssemblyFixture<TFixture>`
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
### `IAsyncLifetime`
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
### Notes about AssemblyFixture 
I cannot split this functionality into two packages bcs. I need own TestFramework for ordering puposes. AssemblyFixtures are often used side by side with ordering. 

*Kick started by [Xunit example](https://github.com/xunit/samples.xunit/tree/master/AssemblyFixtureExample) by Brad Wilson. I've presered his original comments where it was applicable.*

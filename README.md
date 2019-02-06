# Xunit.Extensions.Ordering
Xunit extension for ordering test collections, test classes and test cases. It's useful for integration testing using Xunit if you cannot or you don't want to make each test method atomic.

Nuget: https://www.nuget.org/packages/Xunit.Extensions.Ordering/

There is very limiting space for adding support of ordered (integration) testing into xunit without rewriting runner. 

## Usage:

### Setup  

Add *AssemblyInfo.cs* with only following lines of code

```c#
using Xunit;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
```

### Ordering test classes and cases

Add *Order* Attribute to test classes and methods. Tests are executed in ascending order. If no *Order* attribute is specified default 0 is assigned. Multiple *Order* attributes can have same value. Their execution order in this case is deterministic but unpredictible.

```c#
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

You can order test classes in collections by adding *Order* attribute too but you have to use patched test framework by add following line to AssemblyInfo.cs

```c#
using Xunit;

[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]
```

Now you can order collections like this

```c#
[CollectionDefinition("C1")]
public class Collection1 { }
```
```c#
[Collection("C1"), Order(2)]
public class TC3
{
	[Fact, Order(1)]
	public void M1() { /* 3 */ }

	[Fact, Order(2)]
	public void M2() { /* 4 */ }
}
```
```c#
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

You can order test collections by adding *Order* attribute too definition collection class

```c#
[CollectionDefinition("C1"), Order(3)]
public class Collection3 { }
```
 ```c#
[CollectionDefinition("C2"), Order(1)]
public class Collection3 { }
```

### Mixing test classes in collections and test classes without explicit collection assignement

Test classes without explicitely assigned collection are collections implicitely in Xunit (collection per class). So if you mix test classes with assigned collection and test classes without assigned collection they are on the same level and *Order* is applied following this logic.  

```c#
[CollectionDefinition("C1"), Order(3)]
public class Collection3 { }
```
 ```c#
[CollectionDefinition("C2"), Order(1)]
public class Collection3 { }
```
```c#
[Order(2)]
public class TC2
{
	[Fact]
	public void M1() { /* 4 */ }
}
```
```c#
[Collection("C1")]
public class TC3
{
	[Fact]
	public void M1() { /* 5 */ }
}
```
```c#
[Collection("C2"), Order(2)]
public partial class TC5
{
	[Fact]
	public void M1() { /* 3 */ }
}
```
```c#
[Collection("C2"), Order(1)]
public partial class TC5
{
	[Fact, Order(2)]
	public void M1() { /* 2 */ }

	[Fact, Order(1)]
	public void M2() { /* 1 */ }
}
```

### Checking continuity of order indexes and detection and duplicities

You can enable warning messages about continuity and duplicates of Order indexes by enabling *diagnosticMessages*.
 
	1. Create xnuit.runner.json in root of your test project 
	
```json
{
	"$schema": "https://xunit.github.io/schema/current/xunit.runner.schema.json",
	"diagnosticMessages": true
}
```
	
	2. Set *"Copy to output directory"* for this file in visual studio to *"Copy if newer"*
	3. In the *Output* Visual Studio window choose *"Tests"* option in the *"Show output from"* dropdown
	4. You will see warnings like 
	
```console
Missing test case order sequence from '3' to '19' for tc [Xunit.Extensions.Ordering.Tests.TC1.M2]
 ```

# Xunit.Extensions.Ordering
Xunit extension for ordered (integration) testing

Nuget: https://www.nuget.org/packages/Xunit.Extensions.Ordering/

There is very limiting space for adding support of ordered (integration) testing into xunit without rewriting runner. 

## Usage:

1. Add *AssemblyInfo.cs* with only following lines of code

```c#
using Xunit;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
```

2. Add *Order* Attribute to test classes and methods. Tests are executed in ascending order. If no *Order* attribute is specified default 0 is assigned. Multiple *Order* attributes can have same value. Their execution order in this case is deterministic but unpredictible.

```c#
[Order(1)]
public class TC2
{
	[Fact, Order(2)]
	public void M1() { //... }

	[Fact, Order(3)]
	public void M2() { //... }

	[Fact, Order(1)]
	public void M3() { //... }
}
```
2. You can order collection and test classes in collections too, but you have to reference patched test framework from AssemblyInfo.cs

```c#
using Xunit;

[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]
```

And you can use Order with collections too

```c#
[CollectionDefinition("3"), Order(2)]
public class Collection3 { }
```
```c#
[Collection("3"), Order(2)]
public class TC3
{
	[Fact, Order(1)]
	public void M1() { //... }

	[Fact, Order(2)]
	public void M2() { //... }
}
```
```c#
[Collection("3"), Order(1)]
public partial class TC5
{
	[Fact, Order(2)]
	public void M1() { //... }

	[Fact, Order(1)]
	public void M2() { //... }

}
```

3. You can enable warning messaging about continuity and duplicates of Order indexes by enabling *diagnosticMessages*.
 
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

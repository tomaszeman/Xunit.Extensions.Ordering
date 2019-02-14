using Xunit;
using Xunit.Extensions.Ordering;
using Xunit.Extensions.Ordering.Tests;
using Xunit.Extensions.Ordering.Tests.Fixtures;

//[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]
//[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]
//[assembly: TestClassOrderer("Xunit.Extensions.Ordering.TestClassOrderer", "Xunit.Extensions.Ordering")]

[assembly: AssemblyFixture(typeof(AssemblyFixture4))]
[assembly: AssemblyFixture(typeof(AssemblyFixture5), typeof(AssemblyFixture6))]
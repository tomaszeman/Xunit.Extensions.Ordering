using Xunit;
using Xunit.Extensions.Ordering;
using Xunit.Extensions.Ordering.Tests;
using Xunit.Extensions.Ordering.Tests.Fixtures;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

[assembly: AssemblyFixture(typeof(AssemblyFixture1))]
[assembly: AssemblyFixture(typeof(AssemblyFixture2))]
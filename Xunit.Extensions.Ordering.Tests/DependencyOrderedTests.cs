using System.Collections.Generic;

namespace Xunit.Extensions.Ordering.Tests
{
    [Trait("Ordered", "Dependency")]
    [TestCaseOrderer(TestDependencyOrderer.TypeName, TestDependencyOrderer.AssemblyName)]
    public partial class DependencyOrderTests 
    {
        public static List<int> ExecutionOrder { get; set; } = new List<int>();

        [Fact]
        [TestDependency("DependencyOrderedTest1")]
        public void DependencyOrderedTest0()
        {
            ExecutionOrder.Add(0);
            Assert.Contains(1, ExecutionOrder);
            Assert.Contains(2, ExecutionOrder);
            Assert.Contains(3, ExecutionOrder);
        }

        [Fact]
        [TestDependency("DependencyOrderedTest3")]
        public void DependencyOrderedTest2()
        {
            ExecutionOrder.Add(2);
            Assert.DoesNotContain(0, ExecutionOrder);
            Assert.DoesNotContain(1, ExecutionOrder);
            Assert.Contains(3, ExecutionOrder);
        }
    }

    public partial class DependencyOrderTests
    {
        [Fact]        
        public void DependencyOrderedTest3()
        {
            ExecutionOrder.Add(3);
            Assert.DoesNotContain(0, ExecutionOrder);
            Assert.DoesNotContain(1, ExecutionOrder);
            Assert.DoesNotContain(2, ExecutionOrder);
        }

        [Fact]
        [TestDependency("DependencyOrderedTest2")]
        public void DependencyOrderedTest1()
        {
            ExecutionOrder.Add(1);
            Assert.DoesNotContain(0, ExecutionOrder);
            Assert.Contains(2, ExecutionOrder);
            Assert.Contains(3, ExecutionOrder);
        }
    }
}

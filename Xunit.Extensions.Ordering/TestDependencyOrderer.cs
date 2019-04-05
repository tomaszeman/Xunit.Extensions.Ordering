using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
    /// <summary>
    /// Custom test case orderer which builds order based on TestDependencyAttribute data
    /// </summary>
    public class TestDependencyOrderer : ITestCaseOrderer, ITestCollectionOrderer
    {
        public const string TypeName = "Xunit.Extensions.Ordering.TestDependencyOrderer";

        public const string AssemblyName = "Xunit.Extensions.Ordering";
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            var enumerable = testCases.ToList();
            if (enumerable.Count() > 1)
            {
                var y = enumerable.TSort(
                    x => x.TestMethod.Method
                        .GetCustomAttributes((typeof(TestDependencyAttribute).AssemblyQualifiedName)).FirstOrDefault()
                        ?.GetNamedArgument<IReadOnlyList<string>>("Tests"), x => x.DisplayName);
                return y;
            }

            return enumerable;
        }

        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
        {
            var y = testCollections.TSort(
                x => x.CollectionDefinition.GetCustomAttributes((typeof(TestDependencyAttribute).AssemblyQualifiedName))
                    .FirstOrDefault()?.GetNamedArgument<IReadOnlyList<string>>("Tests"), x => x.DisplayName);
            return y;
        }
    }
}

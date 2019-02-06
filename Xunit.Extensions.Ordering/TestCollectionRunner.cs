using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public class TestCollectionRunner : XunitTestCollectionRunner
	{
		public TestCollectionRunner(
			ITestCollection testCollection,
			IEnumerable<IXunitTestCase> testCases,
			IMessageSink diagnosticMessageSink,
			IMessageBus messageBus,
			ITestCaseOrderer testCaseOrderer,
			ExceptionAggregator aggregator,
			CancellationTokenSource cancellationTokenSource)
			: base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource) {}

		protected async override Task<RunSummary> RunTestClassesAsync()
		{
			var groups = TestCases
				.GroupBy(tc => tc.TestMethod.TestClass, TestClassComparer.Instance);

			if(TestCaseOrderer is TestCaseOrderer orderer)
				groups= orderer.OrderTestClasses(groups);
			
			var summary = new RunSummary();

			foreach (IGrouping<ITestClass, IXunitTestCase> testCasesByClass in groups)
			{
				summary.Aggregate(
					await RunTestClassAsync(
						testCasesByClass.Key,
						(IReflectionTypeInfo)testCasesByClass.Key.Class,
						testCasesByClass));

				if (CancellationTokenSource.IsCancellationRequested)
					break;
			}

			return summary;
		}
	}
}

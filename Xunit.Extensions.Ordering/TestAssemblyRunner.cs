using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public class TestAssemblyRunner : XunitTestAssemblyRunner
	{
		readonly Dictionary<Type, object> assemblyFixtureMappings = new Dictionary<Type, object>();

		public TestAssemblyRunner(ITestAssembly testAssembly,
			IEnumerable<IXunitTestCase> testCases,
			IMessageSink diagnosticMessageSink,
			IMessageSink executionMessageSink,
			ITestFrameworkExecutionOptions executionOptions)
			: base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions) {}

		protected override Task<RunSummary> RunTestCollectionAsync(
			IMessageBus messageBus,
			ITestCollection testCollection,
			IEnumerable<IXunitTestCase> testCases,
			CancellationTokenSource cancellationTokenSource)
		{
			return 
				new TestCollectionRunner(
					assemblyFixtureMappings,
					testCollection,
					testCases,
					DiagnosticMessageSink,
					messageBus,
					TestCaseOrderer,
					new ExceptionAggregator(Aggregator), 
					cancellationTokenSource)
				.RunAsync();
		}
	}
}

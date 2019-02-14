using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	/// <summary>
	/// Xunit.Extensions.Ordering test customized collection runner.
	/// </summary>
	public class TestCollectionRunner : XunitTestCollectionRunner
	{
		protected Dictionary<Type, object> AssemblyFixtureMappings { get; } = new Dictionary<Type, object>();
		public ITestClassOrderer TestClassOrderer { get; }

		///<inheritdoc />
		public TestCollectionRunner(
			Dictionary<Type, object> assemblyFixtureMappings,
			ITestCollection testCollection,
			IEnumerable<IXunitTestCase> testCases,
			IMessageSink diagnosticMessageSink,
			IMessageBus messageBus,
			ITestClassOrderer testClassOrderer,
			ITestCaseOrderer testCaseOrderer,
			ExceptionAggregator aggregator,
			CancellationTokenSource cancellationTokenSource)
			: base(

				  testCollection,
				  testCases,
				  diagnosticMessageSink,
				  messageBus,
				  testCaseOrderer,
				  aggregator,
				  cancellationTokenSource)
		{
			AssemblyFixtureMappings = assemblyFixtureMappings;
			TestClassOrderer = testClassOrderer;
		}

		///<inheritdoc />
		protected async override Task<RunSummary> RunTestClassesAsync()
		{		
			var summary = new RunSummary();

			foreach (ITestClassWithCases tc in 
				TestClassOrderer.OrderTestClasses(
					TestCases
						.GroupBy(
							tc => tc.TestMethod.TestClass,
							(k, g) => new TestClassWithTestCase(k, g.ToArray()),
							TestClassComparer.Instance)))
			{
				summary.Aggregate(
					await RunTestClassAsync(
						tc,
						(IReflectionTypeInfo)tc.Class,
						tc.TestCases));

				if (CancellationTokenSource.IsCancellationRequested)
					break;
			}

			return summary;
		}

		///<inheritdoc />
		protected override Task<RunSummary> RunTestClassAsync(
			ITestClass testClass,
			IReflectionTypeInfo @class,
			IEnumerable<IXunitTestCase> testCases)
		{
			// Don't want to use .Concat + .ToDictionary because of the possibility of overriding types,
			// so instead we'll just let collection fixtures override assembly fixtures.
			var combinedFixtures = new Dictionary<Type, object>(AssemblyFixtureMappings);
			foreach (KeyValuePair<Type, object> kvp in CollectionFixtureMappings)
				combinedFixtures[kvp.Key] = kvp.Value;

			return 
				new XunitTestClassRunner(
					testClass,
					@class,
					testCases,
					DiagnosticMessageSink,
					MessageBus,
					TestCaseOrderer,
					new ExceptionAggregator(Aggregator),
					CancellationTokenSource,
					combinedFixtures)
				.RunAsync();
		}
	}
}

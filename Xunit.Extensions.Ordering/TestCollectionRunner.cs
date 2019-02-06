using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public class TestCollectionRunner : XunitTestCollectionRunner
	{
		protected OrdererBase Orderer { get; } = new OrdererBase();

		public TestCollectionRunner(Dictionary<Type, object> assemblyFixtureMappings,
			ITestCollection testCollection,
			IEnumerable<IXunitTestCase> testCases,
			IMessageSink diagnosticMessageSink,
			IMessageBus messageBus,
			ITestCaseOrderer testCaseOrderer,
			ExceptionAggregator aggregator,
			CancellationTokenSource cancellationTokenSource)
			: base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
		{
		}

		protected async override Task<RunSummary> RunTestClassesAsync()
		{
			var summary = new RunSummary();

			foreach (IXunitTestCase tc in 
				TestCases
					.GroupBy(tc => tc.TestMethod.TestClass.Class.Name)
					.OrderBy(g => Orderer.ExtractOrderFromAttribute(g.First().TestMethod.TestClass.Class.GetCustomAttributes(typeof(OrderAttribute))))
					.SelectMany(g => TestCaseOrderer.OrderTestCases(g)))
			{
				summary.Aggregate(
					await base.RunTestClassAsync(
						tc.TestMethod.TestClass,
						(IReflectionTypeInfo)tc.TestMethod.TestClass.Class,
						new[] { tc }));

				if (CancellationTokenSource.IsCancellationRequested)
					break;

			}

			return summary;
		}

		protected virtual int Order(ITestClass tc)
		{
			ITypeInfo type = tc.Class;

			return ExtractOrderFromAttribute(type.GetCustomAttributes(typeof(OrderAttribute)));
		}

		protected virtual int ExtractOrderFromAttribute(IEnumerable<IAttributeInfo> attributes)
		{
			IAttributeInfo orderAttribute = attributes.FirstOrDefault();

			if (orderAttribute == null)
				return 0;

			return (int)orderAttribute.GetConstructorArguments().First();
		}


	}
}

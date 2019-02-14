using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	/// <summary>
	/// Test class and test case orderer that sorts according to <see cref="OrderAttribute"/>.
	/// </summary>
	public class TestCaseOrderer : OrdererBase, ITestCaseOrderer
	{
		///<inheritdoc />
		public TestCaseOrderer(IMessageSink diagnosticSink)
			: base(diagnosticSink) { }

		///<inheritdoc />
		public virtual IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) 
			where TTestCase : ITestCase
		{
			int lastOrder = 0;

			foreach (var g in
				testCases
					.GroupBy(tc => GetCaseOrder(tc))
					.OrderBy(g => g.Key))
			{
				int count = g.Count();

				if (count > 1)
					DiagnosticSink.OnMessage(
						new DiagnosticMessage(
							g.Key == 0
								? "Found {0} test cases with unassigned or order 0. '{2}'"
								: "Found {0} duplicate order '{1}' for test cases '{2}'",
							count,
							g.Key,
							string.Join("', '", g.Select(tc => $"{tc.TestMethod.TestClass.Class.Name}.{tc.TestMethod.Method.Name}"))));

				if (lastOrder < g.Key - 1)
				{
					int lower = lastOrder + 1;
					int upper = g.Key - 1;
					
					DiagnosticSink.OnMessage(
						new DiagnosticMessage(
							lower == upper
								? "Missing test case order '{0}' in test class '{2}'."
								: "Missing test case order sequence from '{0}' to '{1}' in test class '{2}'.",
							lower,
							upper,
							string.Join("], [", g.Select(tc => tc.TestMethod.TestClass.Class.Name))));
				}

				lastOrder = g.Key;
			}

			return testCases.OrderBy(tc => GetCaseOrder(tc));
		}

		/// <summary>
		/// Orders test class and test cases inside this classes
		/// </summary>
		/// <param name="testCaseGroups"></param>
		/// <returns></returns>
		public virtual IEnumerable<IGrouping<ITestClass, IXunitTestCase>> 
			OrderTestClasses(IEnumerable<IGrouping<ITestClass, IXunitTestCase>> testCaseGroups)
		{
			int lastOrder = 0;

			//nasty check if we are in class without collection defined
			if (testCaseGroups.First().Key.TestCollection.CollectionDefinition != null)
				foreach (var g in
				testCaseGroups
					.GroupBy(g => GetClassOrder(g.Key))
					.OrderBy(g => g.Key))
				{
					int count = g.Count();

					if (count > 1)
						DiagnosticSink.OnMessage(
							new DiagnosticMessage(
								g.Key == 0
									? "Found {0} test classes with unassigned or order 0. '{2}'"
									: "Found {0} duplicates of order '{1}' on test collection '{2}'",
								count,
								g.Key,
								string.Join("', '", g.Select(tc => tc.Key.Class.Name))));

					if (lastOrder < g.Key - 1)
					{
						int lower = lastOrder + 1;
						int upper = g.Key - 1;

						DiagnosticSink.OnMessage(
							new DiagnosticMessage(
								lower == upper
									? "Missing test classes order '{0}' for collection '{2}'."
									: "Missing test classes order sequence from '{0}' to '{1}' for collection '{2}'.",
								lower,
								upper,
								GetCollectionName(g.First().Key.TestCollection)));
					}

					lastOrder = g.Key;
				}

			return testCaseGroups.OrderBy(g => GetClassOrder(g.Key));
		}

		protected virtual int GetClassOrder(ITestClass tc)
		{
			return ExtractOrderFromAttribute(
				tc.Class.GetCustomAttributes(typeof(OrderAttribute)));
		}

		protected virtual int GetCaseOrder(ITestCase tc)
		{
			return ExtractOrderFromAttribute(
				tc.TestMethod.Method.GetCustomAttributes(typeof(OrderAttribute)));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public class TestCaseOrderer : OrdererBase, ITestCaseOrderer
	{
		public TestCaseOrderer(IMessageSink diagnosticSink)
			: base(diagnosticSink) { }

		public virtual IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) 
			where TTestCase : ITestCase
		{
			int lastOrder = 0;

			foreach (var g in
				testCases
					.GroupBy(
						tc => Order(tc),
						(key, g) => new { Order = key, TC = g })
					.OrderBy(g => g.Order))
			{
				int count = g.TC.Count();
				string tcs = string.Join("], [", g.TC.Select(tc => tc.DisplayName));

				if (count > 1)
					DiagnosticSink.OnMessage(
						new DiagnosticMessage(
							g.Order == 0
								? "Found {0} test cases with unassigned or '0' order [{2}]"
								: "Found {0} duplicates of order '{1}' on test cases [{2}]",
							count,
							g.Order,
							tcs));

				if (lastOrder < g.Order - 1)
				{
					int lower = lastOrder + 1, upper = g.Order - 1;
					
					DiagnosticSink.OnMessage(
						new DiagnosticMessage(
							lower == upper
								? "Missing test case order '{0}' for tc '{2}'."
								: "Missing test case order sequence from '{0}' to '{1}' for tc [{2}].",
							lower,
							upper,
							tcs));
				}

				lastOrder = g.Order;
			}

			return testCases.OrderBy(tc => Order(tc));		
		}

		protected virtual int Order(ITestCase tc)
		{
			return ExtractOrderFromAttribute(
				tc.TestMethod.Method.GetCustomAttributes(typeof(OrderAttribute)));
		}
	}
}

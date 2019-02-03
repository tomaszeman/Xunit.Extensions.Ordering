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
		public virtual IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) 
			where TTestCase : ITestCase
		{
			return testCases.OrderBy(tc => GetMethodOrder(tc));		
		}

		protected virtual int GetMethodOrder(ITestCase tc)
		{
			return ExtractOrderFromAttribute(
				tc.TestMethod.Method.GetCustomAttributes(typeof(OrderAttribute)));
		}
	}
}

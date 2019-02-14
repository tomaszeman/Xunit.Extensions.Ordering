using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public class TestClassOrderer : OrdererBase, ITestClassOrderer
	{
		public TestClassOrderer(IMessageSink diagnosticSink) 
			: base(diagnosticSink) {}

		public virtual IEnumerable<TTestClass> OrderTestClasses<TTestClass>(IEnumerable<TTestClass> testClasses)
			where TTestClass : ITestClass
		{
			int lastOrder = 0;

			//Hack: CollectionBehavior CollectionPerAssembly !!!!! is a problem
			if (testClasses.First().TestCollection.CollectionDefinition != null)
				foreach (var g in
				testClasses
					.GroupBy(tc => GetClassOrder(tc))
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
								string.Join("', '", g.Select(tc => tc.Class.Name))));

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
								GetCollectionName(g.First().TestCollection)));
					}

					lastOrder = g.Key;
				}

			return testClasses.OrderBy(tc => GetClassOrder(tc));
		}

		protected virtual int GetClassOrder(ITestClass tc)
		{
			return ExtractOrderFromAttribute(
				tc.Class.GetCustomAttributes(typeof(OrderAttribute)));
		}

	}
}

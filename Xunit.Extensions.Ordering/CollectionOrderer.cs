using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public class CollectionOrderer : OrdererBase, ITestCollectionOrderer
	{
		public CollectionOrderer(IMessageSink diagnosticSink)
			: base(diagnosticSink) { }

		public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
		{
			int lastOrder = 0;

			foreach (var g in
				testCollections
					.GroupBy(
						tc => GetOrder(tc),
						(key, g) => new { Order = key, TC = g })
					.OrderBy(g => g.Order))
			{
				int count = g.TC.Count();
				
				if (count > 1)
				{
					string cols = string.Join(
							"], [",
							g.TC.Select(
								tc => tc.CollectionDefinition != null 
								? tc.DisplayName 
								: TypeNameFromDisplayName(tc)));

					DiagnosticSink.OnMessage(
						new DiagnosticMessage(
							g.Order == 0
								? "Found {0} collections with unassigned or '0' order [{2}]"
								: "Found {0} duplicates of order '{1}' on collections [{2}]",
							count,
							g.Order,
							cols));
				}

				if (lastOrder < g.Order - 1)
				{
					int lower = lastOrder + 1, upper = g.Order - 1;

					DiagnosticSink.OnMessage(
						new DiagnosticMessage(
							lower == upper
								? "Missing collection order '{0}'."
								: "Missing collection order sequence from '{0}' to '{1}'.",
							lower,
							upper));
				}

				lastOrder = g.Order;
			}

			return testCollections.OrderBy(c => GetOrder(c));
		}

		protected virtual int GetOrder(ITestCollection col)
		{
			ITypeInfo type = 
				col.CollectionDefinition 
				?? col.TestAssembly.Assembly.GetType(TypeNameFromDisplayName(col));

			return ExtractOrderFromAttribute(type.GetCustomAttributes(typeof(OrderAttribute)));
		}

		protected virtual string TypeNameFromDisplayName(ITestCollection col)
		{
			return 
				col
					.DisplayName
					.Substring(col.DisplayName.LastIndexOf(' ') + 1);
		} 
	}

}
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Xunit.Extensions.Ordering
{
	public class CollectionOrderer : OrdererBase, ITestCollectionOrderer
	{
		public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
		{
			return testCollections.OrderBy(c => Order(c));
		}

		public int Order(ITestCollection col)
		{
			ITypeInfo type = col.CollectionDefinition;

			if (type == null)
			{
				string typeName = col
					.DisplayName
					.Substring(col.DisplayName.LastIndexOf(' '));

				type= col.TestAssembly.Assembly.GetType(typeName);
			}
			
			return ExtractOrderFromAttribute(type.GetCustomAttributes(typeof(OrderAttribute)));
		}
	}

}
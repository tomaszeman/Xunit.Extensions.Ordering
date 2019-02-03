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
			
			return type == null 
				? 0 
				: ExtractOrderFromAttribute(type.GetCustomAttributes(typeof(OrderAttribute)));
		}
	}

}
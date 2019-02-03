using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;

namespace Xunit.Extensions.Ordering
{
	public abstract class OrdererBase
	{
		protected virtual int ExtractOrderFromAttribute(IEnumerable<IAttributeInfo> attributes)
		{
			IAttributeInfo orderAttribute = attributes.FirstOrDefault();

			if (orderAttribute == null)
				return 0;

			return (int)orderAttribute.GetConstructorArguments().First();
		}
	}
}

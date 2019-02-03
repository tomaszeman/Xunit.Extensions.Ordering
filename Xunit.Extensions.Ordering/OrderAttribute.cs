using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Extensions.Ordering
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class OrderAttribute : Attribute
	{
		private readonly int _order;

		public OrderAttribute(int order)
		{
			_order = order;
		}
	}
}

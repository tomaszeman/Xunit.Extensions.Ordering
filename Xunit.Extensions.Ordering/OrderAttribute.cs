using System;

namespace Xunit.Extensions.Ordering
{
	/// <summary>
	/// Decorate you test collections, test classes and test cases with this attribute to order test execution.
	/// Tests are executed in ascending order. If no `Order` attribute is specified default 0 is assigned. If multiple <see cref="OrderAttribute"/> at same level have same value their execution order against each other is deterministic but unpredictible.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class OrderAttribute : Attribute
	{
		private readonly int _order;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="order">Order index to order test collections, test classes and test cases. Tests are executed in ascending order.</param>
		public OrderAttribute(int order)
		{
			_order = order;
		}
	}
}

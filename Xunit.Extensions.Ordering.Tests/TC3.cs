using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("3"), Order(2)]
	public class TC3
	{
		[Fact, Order(1)]
		public void M1() { Assert.Equal(6, Counter.Next()); }

		[Fact, Order(2)]
		public void M2() { Assert.Equal(7, Counter.Next()); }

	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Extensions.Ordering.Tests
{
	[CollectionDefinition("1"), Collection("1"), Order(3)]
	public class TC1
	{
		[Fact, Order(2)]
		public void M1() { Assert.Equal(9, Counter.Next()); }

		[Fact, Order(20)]
		public void M2() { Assert.Equal(10, Counter.Next()); }

		[Fact, Order(1)]
		public void M3() { Assert.Equal(8, Counter.Next()); }
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("2")]
	public class TC2
	{
		[Fact, Order(2)]
		public void M1() { Assert.Equal(2, Counter.Next()); }

		[Fact, Order(3)]
		public void M2() { Assert.Equal(3, Counter.Next()); }

		[Fact, Order(1)]
		public void M3() { Assert.Equal(1, Counter.Next()); }
	}
}

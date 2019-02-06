using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("3"), Order(3)]
	public partial class TC4
	{
		[Fact, Order(2)]
		public void M1() { Assert.Equal(9, Counter.Next()); }

		[Fact, Order(1)]
		public void M2() { Assert.Equal(8, Counter.Next()); }

	}
}

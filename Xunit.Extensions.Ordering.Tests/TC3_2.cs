using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Extensions.Ordering.Tests
{
	public partial class TC3
	{
		[Fact, Order(4)]
		public void M3() { Assert.Equal(7, Counter.Next()); }

		[Fact, Order(3)]
		public void M4() { Assert.Equal(6, Counter.Next()); }

	}
}

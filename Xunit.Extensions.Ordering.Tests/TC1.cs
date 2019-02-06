namespace Xunit.Extensions.Ordering.Tests
{
	[Order(3)]
	public class TC1
	{
		[Fact, Order(2)]
		public void M1() { Assert.Equal(13, Counter.Next()); }

		[Fact, Order(3)]
		public void M2() { Assert.Equal(14, Counter.Next()); }

		[Fact, Order(1)]
		public void M3() { Assert.Equal(12, Counter.Next()); }
	}
}

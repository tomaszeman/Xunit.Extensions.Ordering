namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("3"), Order(1)]
	public partial class TC5
	{
		[Fact, Order(2)]
		public void M1() { Assert.Equal(5, Counter.Next()); }

		[Fact, Order(1)]
		public void M2() { Assert.Equal(4, Counter.Next()); }

	}
}

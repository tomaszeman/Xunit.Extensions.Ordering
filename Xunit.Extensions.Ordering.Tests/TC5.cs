namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("C1"), Order(1)]
	public partial class TC5
	{
		[Fact, Order(2)]
		public void M1() { Assert.Equal(7, Counter.Next()); }

		[Fact, Order(1)]
		public void M2() { Assert.Equal(6, Counter.Next()); }

	}
}

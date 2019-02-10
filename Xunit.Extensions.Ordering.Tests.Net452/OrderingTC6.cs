namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("C2")]
	public partial class OrderingTC6
	{
		[Fact, Order(2)]
		public void M1() { Assert.Equal(5, Counter.Next()); }

		[Fact]
		public void M2() { Assert.Equal(4, Counter.Next()); }

	}
}

using Xunit.Extensions.Ordering.Tests.Fixtures;

namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("C1"), Order(2)]
	public class OrderingTC3 : IClassFixture<ClassFixture>
	{
		private readonly ClassFixture _classFixture;

		public OrderingTC3(ClassFixture classFixture)
		{
			_classFixture = classFixture;
		}

		[Fact, Order(1)]
		public void M1()
		{
			Assert.Equal(8, Counter.Next());
			Assert.Equal(1, ClassFixture.Count);
		}

		[Fact, Order(2)]
		public void M2()
		{
			Assert.Equal(9, Counter.Next());
			Assert.Equal(1, ClassFixture.Count);
		}
    }
}

using Xunit.Extensions.Ordering.Tests.Fixtures;

namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("C1"), Order(30)]
	public partial class OrderingTC4 : IClassFixture<ClassFixture>
	{
		private readonly ClassFixture _classFixture;

		public OrderingTC4(ClassFixture classFixture)
		{
			_classFixture = classFixture;
		}
	
		[Fact, Order(2)]
		public void M1()
		{
			Assert.Equal(11, Counter.Next());
			Assert.Equal(2, ClassFixture.Count);
		}

		[Fact, Order(1)]
		public void M2()
		{
			Assert.Equal(10, Counter.Next());
			Assert.Equal(2, ClassFixture.Count);
		}
    }
}

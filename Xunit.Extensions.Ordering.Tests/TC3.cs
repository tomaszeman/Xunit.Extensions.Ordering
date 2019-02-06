namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("3"), Order(2)]
	public class TC3 : IClassFixture<ClassFixture>
	{
		private readonly ClassFixture _classFixture;

		public TC3(ClassFixture classFixture)
		{
			_classFixture = classFixture;
		}

		[Fact, Order(1)]
		public void M1()
		{
			Assert.Equal(6, Counter.Next());
			Assert.Equal(1, ClassFixture.Count);
		}

		[Fact, Order(2)]
		public void M2()
		{
			Assert.Equal(7, Counter.Next());
			Assert.Equal(1, ClassFixture.Count);
		}

	}
}

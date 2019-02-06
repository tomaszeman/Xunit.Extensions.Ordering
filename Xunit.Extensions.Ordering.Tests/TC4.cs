namespace Xunit.Extensions.Ordering.Tests
{
	[Collection("C1"), Order(3)]
	public partial class TC4 : IClassFixture<ClassFixture>
	{
		private readonly ClassFixture _classFixture;

		public TC4(ClassFixture classFixture)
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

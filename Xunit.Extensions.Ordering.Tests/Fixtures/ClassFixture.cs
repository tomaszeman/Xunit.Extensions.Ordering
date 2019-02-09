namespace Xunit.Extensions.Ordering.Tests.Fixtures
{
	public class ClassFixture
	{
		public ClassFixture() { Count++; }

		public static int Count { get; private set; }
	}
}

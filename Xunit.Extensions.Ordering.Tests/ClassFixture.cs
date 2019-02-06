namespace Xunit.Extensions.Ordering.Tests
{
	public class ClassFixture
	{
		public ClassFixture() { Count++; }

		public static int Count { get; private set; }
	}
}

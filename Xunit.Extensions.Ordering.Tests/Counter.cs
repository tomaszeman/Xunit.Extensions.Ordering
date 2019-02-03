namespace Xunit.Extensions.Ordering.Tests
{
	static class Counter
	{
		public static int Count { get; private set; }

		public static int Next()
		{
			return ++Count;
		}
	}
}

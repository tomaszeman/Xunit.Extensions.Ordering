namespace Xunit.Extensions.Ordering.Tests
{
	[CollectionDefinition("C1"), Order(2)]
	public class Collection1 { }

	[CollectionDefinition("C2"), Order(1)]
	public class Collection2 { }
}

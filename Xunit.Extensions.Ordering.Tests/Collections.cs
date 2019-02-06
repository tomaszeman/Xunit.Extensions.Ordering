namespace Xunit.Extensions.Ordering.Tests
{
	[CollectionDefinition("C1"), Order(3)]
	public class Collection1 { }

	[CollectionDefinition("C2"), Order(2)]
	public class Collection2 { }
}

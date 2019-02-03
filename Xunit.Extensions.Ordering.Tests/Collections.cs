using System;
using Xunit;

namespace Xunit.Extensions.Ordering.Tests
{
	[CollectionDefinition("2"), Order(1)]
	public class Collection2 { }
	[CollectionDefinition("3"), Order(2)]
	public class Collection3 { }
}

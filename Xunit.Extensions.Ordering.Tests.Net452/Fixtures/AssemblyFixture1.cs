using System;
using System.Collections.Generic;
using System.Text;

namespace Xunit.Extensions.Ordering.Tests.Fixtures
{
	public class AssemblyFixture1
	{
		public AssemblyFixture1() { Count++; }

		public static int Count { get; private set; }
	}
}

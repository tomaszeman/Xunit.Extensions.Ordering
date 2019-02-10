using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering.Tests.Fixtures
{
	public class AssemblyFixture6
	{
		public static int Count { get; private set; }

		public AssemblyFixture6() { Count++; }
	}
}

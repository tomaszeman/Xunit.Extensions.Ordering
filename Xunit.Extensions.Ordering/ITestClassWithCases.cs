using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public interface ITestClassWithCases : ITestClass
	{
		IEnumerable<IXunitTestCase> TestCases { get; }
	}
}

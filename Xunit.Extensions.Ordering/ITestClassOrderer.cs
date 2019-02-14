using System.Collections.Generic;
using Xunit.Abstractions;

namespace Xunit.Extensions.Ordering
{
	public interface ITestClassOrderer
	{
		IEnumerable<TTestClass> OrderTestClasses<TTestClass>(IEnumerable<TTestClass> testClasses) where TTestClass : ITestClass;
	}
}
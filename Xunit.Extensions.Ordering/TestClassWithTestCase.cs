using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	public class TestClassWithTestCase : ITestClassWithCases
	{
		public ITestClass TestClass { get; }

		public IEnumerable<IXunitTestCase> TestCases { get; }

		public ITypeInfo Class => TestClass.Class;

		public ITestCollection TestCollection => TestClass.TestCollection;

		public TestClassWithTestCase(ITestClass testClass, IEnumerable<IXunitTestCase> testCases)
		{
			TestClass = testClass;
			TestCases = testCases;
		}

		public void Deserialize(IXunitSerializationInfo info)
		{
			TestClass.Deserialize(info);
		}

		public void Serialize(IXunitSerializationInfo info)
		{
			TestClass.Serialize(info);
		}
	}
}

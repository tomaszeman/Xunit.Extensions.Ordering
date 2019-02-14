using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	/// <summary>
	/// Xunit.Extensions.Ordering test framework.
	/// </summary>
	public class TestFramework : XunitTestFramework
	{
		///<inheritdoc />
		public TestFramework(IMessageSink messageSink)
			: base(messageSink) {}

		///<inheritdoc />
		protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
		{
			return new TestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
		}
	}
}

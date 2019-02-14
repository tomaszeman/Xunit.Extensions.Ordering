using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	/// <summary>
	/// Xunit.Extensions.Ordering test framework executor.
	/// </summary>
	public class TestFrameworkExecutor : XunitTestFrameworkExecutor
	{
		///<inheritdoc />
		public TestFrameworkExecutor(
			AssemblyName assemblyName,
			ISourceInformationProvider sourceInformationProvider,
			IMessageSink diagnosticMessageSink)
			: base(assemblyName, sourceInformationProvider, diagnosticMessageSink) {}

		///<inheritdoc />
		protected override async void RunTestCases(
			IEnumerable<IXunitTestCase> testCases,
			IMessageSink executionMessageSink,
			ITestFrameworkExecutionOptions executionOptions)
		{
			using (var assemblyRunner = 
				new TestAssemblyRunner(
					TestAssembly,
					testCases,
					DiagnosticMessageSink,
					executionMessageSink,
					executionOptions))
				await assemblyRunner.RunAsync();
		}
	}
}

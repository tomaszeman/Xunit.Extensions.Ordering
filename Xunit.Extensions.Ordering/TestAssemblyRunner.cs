using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
	/// <summary>
	/// Xunit.Extensions.Ordering customized test assembly runner.
	/// </summary>
	public class TestAssemblyRunner : XunitTestAssemblyRunner
	{
		protected Dictionary<Type, object> AssemblyFixtureMappings { get; } = new Dictionary<Type, object>();

		public TestAssemblyRunner(ITestAssembly testAssembly,
			IEnumerable<IXunitTestCase> testCases,
			IMessageSink diagnosticMessageSink,
			IMessageSink executionMessageSink,
			ITestFrameworkExecutionOptions executionOptions)
			: base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions) {}

		protected override async Task AfterTestAssemblyStartingAsync()
		{
			await base.AfterTestAssemblyStartingAsync();
			await CreateAssemblyFixturesAsync();
		}

		protected override async Task BeforeTestAssemblyFinishedAsync()
		{
			// Make sure we clean up everybody who is disposable, and use Aggregator.Run to isolate Dispose failures
			await Task
				.WhenAll(AssemblyFixtureMappings.Values.OfType<IAsyncLifetime>()
				.Select(fixture => Aggregator.RunAsync(fixture.DisposeAsync)));

			foreach (IDisposable disposable in AssemblyFixtureMappings.Values.OfType<IDisposable>())
				Aggregator.Run(disposable.Dispose);

			await base.BeforeTestAssemblyFinishedAsync();
		}

		protected virtual void CreateAssemlbyFixture(Type fixtureType)
		{
			ConstructorInfo[] ctors = 
				fixtureType
					.GetTypeInfo()
					.DeclaredConstructors
					.Where(ci => !ci.IsStatic && ci.IsPublic)
					.ToArray();

			if (ctors.Length != 1)
			{
				Aggregator
					.Add(
						new TestClassException(
							$"Assembly fixture type '{fixtureType.FullName}' may only define a single public constructor."));
				return;
			}

			ConstructorInfo ctor = ctors[0];
			var missingParameters = new List<ParameterInfo>();
			object[] ctorArgs = 
				ctor
					.GetParameters()
					.Select(p =>
					{
						if (p.ParameterType == typeof(IMessageSink))
							return (object) DiagnosticMessageSink;
						
						missingParameters.Add(p);
						return null;
					})
				.ToArray();

			if (missingParameters.Count > 0)
				Aggregator.Add(
						new TestClassException(
							$"Assembly fixture type '{fixtureType.FullName}' had one or more unresolved constructor arguments: "
							+ string.Join(", ", missingParameters.Select(p => $"{p.ParameterType.Name} {p.Name}"))));
			else
				Aggregator.Run(() => AssemblyFixtureMappings[fixtureType] = ctor.Invoke(ctorArgs));
		}

		protected virtual async Task CreateAssemblyFixturesAsync()
		{
			//discover all fixture defined using IAssemblyFixture<> fixtures
			//and merge them with all defined using <AssemblyFixtureAttribute
			foreach (Type type in 
				TestCases
					.SelectMany
					( tc =>
						((IReflectionTypeInfo)tc.TestMethod.TestClass.Class)
							.Type
							.GetTypeInfo()
							.ImplementedInterfaces
							.Where(i => i.GetTypeInfo().IsGenericType
								&& i.GetGenericTypeDefinition() == typeof(IAssemblyFixture<>)))
							.Select(u => u.GenericTypeArguments.Single()
					)
					.Union
					(
						((IReflectionAssemblyInfo)TestAssembly.Assembly)
							.Assembly
							.GetCustomAttributes<AssemblyFixtureAttribute>()
							.SelectMany(f => f.FixtureTypes)
					)
					.Distinct())
				CreateAssemlbyFixture(type);

			await Task.WhenAll(
				AssemblyFixtureMappings
					.Values
					.OfType<IAsyncLifetime>()
					.Select(fixture => Aggregator.RunAsync(fixture.InitializeAsync)));
		}

		protected override Task<RunSummary> RunTestCollectionAsync(
			IMessageBus messageBus,
			ITestCollection testCollection,
			IEnumerable<IXunitTestCase> testCases,
			CancellationTokenSource cancellationTokenSource)
		{
			return 
				new TestCollectionRunner(
					AssemblyFixtureMappings,
					testCollection,
					testCases,
					DiagnosticMessageSink,
					messageBus,
					TestCaseOrderer,
					new ExceptionAggregator(Aggregator), 
					cancellationTokenSource)
				.RunAsync();
		}
	}
}

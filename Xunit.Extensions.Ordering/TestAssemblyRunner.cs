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

		protected ITestClassOrderer TestClassOrderer { get; set; }

		private bool _initialized = false;

		///<inheritdoc />
		public TestAssemblyRunner(ITestAssembly testAssembly,
			IEnumerable<IXunitTestCase> testCases,
			IMessageSink diagnosticMessageSink,
			IMessageSink executionMessageSink,
			ITestFrameworkExecutionOptions executionOptions)
			: base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
		{
			TestCollectionOrderer = new TestCollectionOrderer(diagnosticMessageSink);
			TestCaseOrderer = new TestCaseOrderer(diagnosticMessageSink);
			TestClassOrderer = new TestClassOrderer(diagnosticMessageSink);
		}

		///<inheritdoc />
		protected override async Task AfterTestAssemblyStartingAsync()
		{
			await base.AfterTestAssemblyStartingAsync();
			await CreateAssemblyFixturesAsync();
		}

		///<inheritdoc />
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

		protected override string GetTestFrameworkEnvironment()
		{
			string result= base.GetTestFrameworkEnvironment();

			if (_initialized)
				return result;

			IAttributeInfo ordererAttr =
				TestAssembly
					.Assembly
					.GetCustomAttributes(typeof(TestClassOrdererAttribute))
					.SingleOrDefault();

			if (ordererAttr != null)
			{
				string[] args = ordererAttr.GetConstructorArguments().Cast<string>().ToArray();
				try
				{
					ITestClassOrderer orderer = GetTestClassOrderer(args[1], args[0]);

					if (orderer != null)
						TestClassOrderer = orderer;
					else
						DiagnosticMessageSink
							.OnMessage(
								new DiagnosticMessage(
									$"Could not find type '{args[0]}' in {args[1]} for assembly-level test class orderer"));

				}
				catch (Exception ex)
				{
					DiagnosticMessageSink
						.OnMessage(new DiagnosticMessage($"Assembly-level test case orderer '{args[0]}' threw Exception {ex}"));
				}
			}

			_initialized = true;

			return result;
		}

		protected virtual ITestClassOrderer GetTestClassOrderer(string assemblyName, string typeName)
		{
			Assembly assembly = null;
			try
			{
				var aname = new AssemblyName(assemblyName);
				assembly = Assembly.Load(
					new AssemblyName
					{
						Name = aname.Name,
						Version = aname.Version
					});
			}
			catch
			{
				return null;
			}

			TypeInfo ordererType = assembly
				.DefinedTypes
				.FirstOrDefault(t => t.FullName == typeName);

			if (ordererType == null)
				return null;
				
			return
				ExtensibilityPointFactory
					.Get<ITestClassOrderer>(DiagnosticMessageSink, ordererType.AsType());
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
		///<inheritdoc />
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
					TestClassOrderer,
					TestCaseOrderer,
					new ExceptionAggregator(Aggregator), 
					cancellationTokenSource)
				.RunAsync();
		}
	}
}

using Xunit.Abstractions;
using Xunit.Extensions.Ordering.Tests.Fixtures;

namespace Xunit.Extensions.Ordering.Tests
{
	public class AssemblyFixtureTC2 : 
		IAssemblyFixture<AssemblyFixture1>,
		IAssemblyFixture<AssemblyFixture2>
	{
		private readonly AssemblyFixture1 _fixture1;
		private readonly AssemblyFixture2 _fixture2;
		private readonly ITestOutputHelper _testOutput;

		public AssemblyFixtureTC2(AssemblyFixture1 fixture1, ITestOutputHelper testOutput, AssemblyFixture2 fixture2)
		{
			_fixture1 = fixture1;
			_fixture2 = fixture2;
			_testOutput = testOutput;
		}

		[Fact]
		public void Ctor_OneInstancePerAssembly()
		{
			Assert.Equal(1, AssemblyFixture1.Count);
		}

		[Fact]
		public void Ctor_FixturesAndTestOutputInjected()
		{
			Assert.NotNull(_fixture1);
			Assert.NotNull(_fixture2);
			Assert.NotNull(_testOutput);
		}

		[Fact]
		public void AssemlyFixture2_SinkInjected()
		{
			Assert.NotNull(_fixture2.MesssageSink);
		}

		[Fact]
		public void AssemlyFixture2_AsyncInitialized()
		{
			Assert.True(_fixture2.Initialized);
		}
	}
}

using Xunit.Extensions.Ordering.Tests.Fixtures;

namespace Xunit.Extensions.Ordering.Tests
{
	[CollectionDefinition("AF"), Collection("AF")]
	public class AssemblyFixtureTC4
	{
		private readonly AssemblyFixture1 _fixture1;
		private readonly AssemblyFixture4 _fixture4;
		private readonly AssemblyFixture5 _fixture5;
		private readonly AssemblyFixture6 _fixture6;

		public AssemblyFixtureTC4(AssemblyFixture1 fixture1, AssemblyFixture4 fixture4, AssemblyFixture5 fixture5, AssemblyFixture6 fixture6)
		{
			_fixture1 = fixture1;
			_fixture4 = fixture4;
			_fixture5 = fixture5;
			_fixture6 = fixture6;
		}

		[Fact]
		public void Ctor_OneInstancePerAssembly()
		{
			Assert.Equal(1, AssemblyFixture1.Count);
			Assert.Equal(1, AssemblyFixture4.Count);
			Assert.Equal(1, AssemblyFixture5.Count);
			Assert.Equal(1, AssemblyFixture6.Count);
		}

		[Fact]
		public void Ctor_FixturesInjected()
		{
			Assert.NotNull(_fixture1);
			Assert.NotNull(_fixture4);
			Assert.NotNull(_fixture5);
			Assert.NotNull(_fixture6);
		}
	}
}

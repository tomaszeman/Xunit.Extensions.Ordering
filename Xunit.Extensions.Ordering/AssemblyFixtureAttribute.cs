using System;

namespace Xunit.Extensions.Ordering
{
	/// <summary>
	/// Registers AssemblyFixture at assembly level. I recommend to use <see cref="Xunit.Extensions.Ordering.IAssemblyFixture{TFixture}"/> instead.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class AssemblyFixtureAttribute : Attribute
	{
		/// <summary>
		/// Gets all assembly fixture types registered via this <see cref="AssemblyFixtureAttribute"/> instance
		/// </summary>
		public Type[] FixtureTypes { get; }

		/// <summary>
		/// Registers single or multiple assembly fixtures
		/// </summary>
		/// <param name="fixtureTypes">Single or multipe assembly fixture types to register.</param>
		public AssemblyFixtureAttribute(params Type[] fixtureTypes)
		{
			FixtureTypes = fixtureTypes;
		}
	}
}


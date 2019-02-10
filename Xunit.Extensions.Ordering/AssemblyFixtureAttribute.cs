using System;

namespace Xunit.Extensions.Ordering
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class AssemblyFixtureAttribute : Attribute
	{
		public Type[] FixtureTypes { get; }

		public AssemblyFixtureAttribute(Type fixtureType)
		{
			FixtureTypes = new[] { fixtureType };
		}

		public AssemblyFixtureAttribute(params Type[] fixtureTypes)
		{
			FixtureTypes = fixtureTypes;
		}
	}
}


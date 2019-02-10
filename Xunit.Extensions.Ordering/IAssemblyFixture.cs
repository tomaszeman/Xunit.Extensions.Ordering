namespace Xunit.Extensions.Ordering
{
	/// <summary>
	/// Interface do decorate you test classes to register assembly level fixture.
	/// </summary>
	/// <typeparam name="TFixture">Assembly fixture to register</typeparam>
	public interface IAssemblyFixture<TFixture> 
		where TFixture : class { }
}

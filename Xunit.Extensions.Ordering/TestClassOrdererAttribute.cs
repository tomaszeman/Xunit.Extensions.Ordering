using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xunit.Extensions.Ordering
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class TestClassOrdererAttribute : Attribute
	{
		public string OrdererTypeName { get; set; }
		public string OrdererAssemblyName { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestClassOrdererAttribute"/> class.
		/// </summary>
		/// <param name="ordererTypeName">The type name of the orderer class (that implements <see cref="Xunit.Extensions.Ordering.ITestClassOrderer"/>).</param>
		/// <param name="ordererAssemblyName">The assembly that <paramref name="ordererTypeName"/> exists in.</param>
		public TestClassOrdererAttribute(string ordererTypeName, string ordererAssemblyName)
		{
			OrdererTypeName = ordererTypeName;
			OrdererAssemblyName = ordererAssemblyName;
		}

	}
}

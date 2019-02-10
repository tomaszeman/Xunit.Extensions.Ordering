using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Xunit.Extensions.Ordering
{
	public class OrdererBase
	{
		protected IMessageSink DiagnosticSink { get; }

		public OrdererBase(IMessageSink diagnosticSink)
		{
			DiagnosticSink = diagnosticSink;
		}

		protected virtual int ExtractOrderFromAttribute(IEnumerable<IAttributeInfo> attributes)
		{
			IAttributeInfo orderAttribute = attributes.FirstOrDefault();

			if (orderAttribute == null)
				return 0;

			return (int) orderAttribute.GetConstructorArguments().Single();
		}
								
		protected virtual string GetCollectionName(ITestCollection col)
		{
			return
				col.CollectionDefinition != null
				? col.DisplayName
				: GetCollectionTypeName(col);
		}

		protected virtual string GetCollectionTypeName(ITestCollection col)
		{
			return
				col
					.DisplayName
					.Substring(col.DisplayName.LastIndexOf(' ') + 1);
		}
	}
}

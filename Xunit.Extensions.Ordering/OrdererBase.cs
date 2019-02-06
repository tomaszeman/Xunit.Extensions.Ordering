using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Abstractions;

namespace Xunit.Extensions.Ordering
{
	public class OrdererBase
	{
		protected IMessageSink DiagnosticSink { get; }

		public OrdererBase() { }

		public OrdererBase(IMessageSink diagnosticSink)
		{
			DiagnosticSink = diagnosticSink;
		}

		public virtual int ExtractOrderFromAttribute(IEnumerable<IAttributeInfo> attributes)
		{
			IAttributeInfo orderAttribute = attributes.FirstOrDefault();

			if (orderAttribute == null)
				return 0;

			return (int) orderAttribute.GetConstructorArguments().First();
		}
	}
}

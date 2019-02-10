using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering.Tests.Fixtures
{
	public class AssemblyFixture2 : IDisposable, IAsyncLifetime
	{
		public IMessageSink MesssageSink { get; }
		public bool Initialized { get; private set; } = false;
		public static int Count { get; private set; }

		public AssemblyFixture2(IMessageSink messsageSink)
		{
			MesssageSink = messsageSink;
			Count++;
		}

		public void Dispose()
		{
			MesssageSink.OnMessage(
					new DiagnosticMessage("AssemblyFixture disposed."));
		}

		public async Task InitializeAsync()
		{
			await Task.Run(() => { Initialized = true; });
		}

		public async Task DisposeAsync()
		{
			await Task.Run(
				() => MesssageSink.OnMessage(
					new DiagnosticMessage("AssemblyFixture disposed async.")));
		}

		
	}
}

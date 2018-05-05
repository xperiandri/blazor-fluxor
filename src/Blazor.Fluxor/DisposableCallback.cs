using System;

namespace Blazor.Fluxor
{
	public class DisposableCallback : IDisposable
	{
		private readonly Action Action;

		public DisposableCallback(Action action)
		{
			Action = action ?? throw new ArgumentNullException(nameof(action));
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Action();
		}

		~DisposableCallback()
		{
			throw new InvalidOperationException("DisposableCallback was not disposed");
		}
	}
}

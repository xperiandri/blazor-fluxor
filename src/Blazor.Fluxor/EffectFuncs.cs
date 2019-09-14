using System.Threading.Tasks;

namespace Blazor.Fluxor
{
	public sealed class EffectFuncs
	{
		public delegate Task HandleAsyncHandler(object action, IDispatcher dispatcher);
		public delegate bool ShouldReactToActionHandler(object action);

		public readonly HandleAsyncHandler HandleAsync;
		public readonly ShouldReactToActionHandler ShouldReactToAction;

		public EffectFuncs(ShouldReactToActionHandler shouldReactToAction, HandleAsyncHandler handleAsync)
		{
			ShouldReactToAction = shouldReactToAction;
			HandleAsync = handleAsync;
		}
	}
}

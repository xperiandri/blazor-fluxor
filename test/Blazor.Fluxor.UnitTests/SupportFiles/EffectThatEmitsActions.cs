using System;
using System.Threading.Tasks;

namespace Blazor.Fluxor.UnitTests.SupportFiles
{
    public class EffectThatEmitsActions<TTriggerAction> : Effect<TTriggerAction>
    {
        public readonly object[] actionsToEmit;
        private readonly IDispatcher dispatcher;

        public EffectThatEmitsActions(object[] actionsToEmit, IDispatcher dispatcher)
        {
            this.actionsToEmit = actionsToEmit ?? Array.Empty<object>();
            this.dispatcher = dispatcher;
        }
        protected override Task HandleAsync(TTriggerAction action)
        {
            foreach (object actionToEmit in actionsToEmit)
                dispatcher.Dispatch(actionToEmit);
            return Task.CompletedTask;
        }
    }
}

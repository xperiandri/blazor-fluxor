using Blazor.Fluxor.UnitTests.MockFactories;
using Blazor.Fluxor.UnitTests.SupportFiles;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blazor.Fluxor.UnitTests.StoreTests
{
	public partial class StoreTests
	{
		public class DispatchAsync
		{
			BrowserInteropStub BrowserInteropStub = new BrowserInteropStub();

			[Fact]
			public async Task ThrowsArgumentNullException_WhenActionIsNull()
			{
				var subject = new Store(BrowserInteropStub);
				await Assert.ThrowsAsync<ArgumentNullException>(() => subject.DispatchAsync(null));
			}

			[Fact]
			public async Task ThrowsInvalidOperationException_IfStoreHasNotBeenInitialised()
			{
				var subject = new Store(BrowserInteropStub);
				await Assert.ThrowsAsync<InvalidOperationException>(() => subject.DispatchAsync(new TestAction()));
			}

			[Fact]
			public async Task DoesNotDispatchActions_WhenIsInsideMiddlewareChange()
			{
				var mockMiddleware = MockMiddlewareFactory.Create();

				var subject = new Store(BrowserInteropStub);
				subject.AddMiddleware(mockMiddleware.Object);

				BrowserInteropStub._TriggerPageLoaded();

				var testAction = new TestAction();
				using (subject.BeginInternalMiddlewareChange())
				{
					await subject.DispatchAsync(testAction);
				}

				mockMiddleware.Verify(x => x.MayDispatchAction(testAction), Times.Never);
			}

			[Fact]
			public async Task DoesNotSendActionToFeatures_WhenMiddlewareForbidsIt()
			{
				var testAction = new TestAction();
				var mockFeature = MockFeatureFactory.Create();
				var mockMiddleware = MockMiddlewareFactory.Create();
				mockMiddleware
					.Setup(x => x.MayDispatchAction(testAction))
					.Returns(false);
				var subject = new Store(BrowserInteropStub);

				BrowserInteropStub._TriggerPageLoaded();
				await subject.DispatchAsync(testAction);

				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(testAction), Times.Never);
			}

			[Fact]
			public async Task ExecutesBeforeDispatchActionOnMiddlewares()
			{
				var testAction = new TestAction();
				var mockMiddleware = MockMiddlewareFactory.Create();
				var subject = new Store(BrowserInteropStub);
				subject.AddMiddleware(mockMiddleware.Object);

				BrowserInteropStub._TriggerPageLoaded();
				await subject.DispatchAsync(testAction);

				mockMiddleware
					.Verify(x => x.BeforeDispatch(testAction), Times.Once);
			}

			[Fact]
			public async Task NotifiesFeatures()
			{
				var mockFeature = MockFeatureFactory.Create();
				var subject = new Store(BrowserInteropStub);
				subject.AddFeature(mockFeature.Object);

				var testAction = new TestAction();
				BrowserInteropStub._TriggerPageLoaded();
				await subject.DispatchAsync(testAction);

				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(testAction));
			}

			[Fact]
			public async Task DispatchesTasksFromMiddleware_WhenAfterDispatchReturnsATask()
			{
				var testAction = new TestAction();
				var testActionFromMiddleware = new TestActionFromMiddleware();
				var mockMiddleware = MockMiddlewareFactory.Create();
				mockMiddleware
					.Setup(x => x.AfterDispatch(testAction))
					.Returns(new IAction[] { testActionFromMiddleware });
				var mockFeature = MockFeatureFactory.Create();

				var subject = new Store(BrowserInteropStub);
				subject.AddMiddleware(mockMiddleware.Object);
				subject.AddFeature(mockFeature.Object);
				BrowserInteropStub._TriggerPageLoaded();

				await subject.DispatchAsync(testAction);

				mockMiddleware
					.Verify(x => x.MayDispatchAction(testActionFromMiddleware), Times.Once);
				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(testActionFromMiddleware), Times.Once);

			}

			[Fact]
			public async Task DispatchesTasksFromEffect()
			{
				var mockFeature = MockFeatureFactory.Create();
				var actionToEmit1 = new TestActionFromEffect1();
				var actionToEmit2 = new TestActionFromEffect2();
				var actionsToEmit = new IAction[] { actionToEmit1, actionToEmit2 };
				var subject = new Store(BrowserInteropStub);
				subject.AddFeature(mockFeature.Object);
				subject.AddEffect(new EffectThatEmitsActions<TestAction>(actionsToEmit));

				BrowserInteropStub._TriggerPageLoaded();
				await subject.DispatchAsync(new TestAction());

				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(actionToEmit1), Times.Once);
				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(actionToEmit2), Times.Once);
			}

			[Fact]
			public async Task TriggersOnlyEffectsThatHandleTheDispatchedAction()
			{
				var mockIncompatibleEffect = new Mock<IEffect>();
				mockIncompatibleEffect
					.Setup(x => x.ShouldReactToAction(It.IsAny<IAction>()))
					.Returns(false);
				var mockCompatibleEffect = new Mock<IEffect>();
				mockCompatibleEffect
					.Setup(x => x.ShouldReactToAction(It.IsAny<IAction>()))
					.Returns(true);

				var subject = new Store(BrowserInteropStub);
				subject.AddEffect(mockIncompatibleEffect.Object);
				subject.AddEffect(mockCompatibleEffect.Object);
				BrowserInteropStub._TriggerPageLoaded();

				var action = new TestAction();
				await subject.DispatchAsync(action);

				mockIncompatibleEffect.Verify(x => x.HandleAsync(action), Times.Never);
				mockCompatibleEffect.Verify(x => x.HandleAsync(action), Times.Once);
			}
		}


	}
}

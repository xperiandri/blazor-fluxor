using Blazor.Fluxor.UnitTests.MockFactories;
using Blazor.Fluxor.UnitTests.SupportFiles;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Blazor.Fluxor.UnitTests.StoreTests
{
	public partial class StoreTests
	{
		public class Dispatch
		{
			BrowserInteropStub BrowserInteropStub = new BrowserInteropStub();

			[Fact]
			public void ThrowsArgumentNullException_WhenActionIsNull()
			{
				var subject = new Store(BrowserInteropStub);
				Assert.Throws<ArgumentNullException>(() => subject.Dispatch(null));
			}

			[Fact]
			public void DoesNotDispatchActions_WhenIsInsideMiddlewareChange()
			{
				var mockMiddleware = MockMiddlewareFactory.Create();

				var subject = new Store(BrowserInteropStub);
				subject.AddMiddleware(mockMiddleware.Object);

				BrowserInteropStub._TriggerPageLoaded();

				var testAction = new TestAction();
				using (subject.BeginInternalMiddlewareChange())
				{
					subject.Dispatch(testAction);
				}

				mockMiddleware.Verify(x => x.MayDispatchAction(testAction), Times.Never);
			}

			[Fact]
			public void DoesNotSendActionToFeatures_WhenMiddlewareForbidsIt()
			{
				var testAction = new TestAction();
				var mockFeature = MockFeatureFactory.Create();
				var mockMiddleware = MockMiddlewareFactory.Create();
				mockMiddleware
					.Setup(x => x.MayDispatchAction(testAction))
					.Returns(false);
				var subject = new Store(BrowserInteropStub);

				BrowserInteropStub._TriggerPageLoaded();
				subject.Dispatch(testAction);

				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(testAction), Times.Never);
			}

			[Fact]
			public void ExecutesBeforeDispatchActionOnMiddlewares()
			{
				var testAction = new TestAction();
				var mockMiddleware = MockMiddlewareFactory.Create();
				var subject = new Store(BrowserInteropStub);
				subject.AddMiddleware(mockMiddleware.Object);

				BrowserInteropStub._TriggerPageLoaded();
				subject.Dispatch(testAction);

				mockMiddleware
					.Verify(x => x.BeforeDispatch(testAction), Times.Once);
			}

			[Fact]
			public void NotifiesFeatures()
			{
				var mockFeature = MockFeatureFactory.Create();
				var subject = new Store(BrowserInteropStub);
				subject.AddFeature(mockFeature.Object);

				var testAction = new TestAction();
				BrowserInteropStub._TriggerPageLoaded();
				subject.Dispatch(testAction);

				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(testAction));
			}

			[Fact]
			public void DispatchesTasksFromEffect()
			{
				var mockFeature = MockFeatureFactory.Create();
				mockFeature
					.Setup(x => x.ReceiveDispatchNotificationFromStore(It.IsAny<object>()));
				var actionToEmit1 = new TestActionFromEffect1();
				var actionToEmit2 = new TestActionFromEffect2();
				var testAction = new TestAction();

				var subject = new Store(BrowserInteropStub);
				subject.AddFeature(mockFeature.Object);

				var mockIEffectFuncs = new Mock<IEffectFuncs>();
				mockIEffectFuncs
					.Setup(x => x.ShouldReactToAction(testAction))
					.Returns(true);
				mockIEffectFuncs
					.Setup(x => x.HandleAsync(testAction, It.IsAny<IDispatcher>()))
					.Callback<object, IDispatcher>((action, dispatcher) =>
					{
						dispatcher.Dispatch(actionToEmit1);
						dispatcher.Dispatch(actionToEmit2);
					})
					.Returns(Task.CompletedTask);
				subject.AddEffect(mockIEffectFuncs.Object);

				BrowserInteropStub._TriggerPageLoaded();
				subject.Dispatch(testAction);

				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(actionToEmit1), Times.Once);
				mockFeature
					.Verify(x => x.ReceiveDispatchNotificationFromStore(actionToEmit2), Times.Once);
			}

			[Fact]
			public void TriggersOnlyEffectsThatHandleTheDispatchedAction()
			{
				var mockIncompatibleEffectHandler = new Mock<IEffectFuncs>();
				mockIncompatibleEffectHandler
					.Setup(x => x.ShouldReactToAction(It.IsAny<object>()))
					.Returns(false);

				var mockCompatibleEffectHandler = new Mock<IEffectFuncs>();
				mockCompatibleEffectHandler
					.Setup(x => x.ShouldReactToAction(It.IsAny<object>()))
					.Returns(true);

				var subject = new Store(BrowserInteropStub);
				subject.AddEffect(mockIncompatibleEffectHandler.Object);
				subject.AddEffect(mockCompatibleEffectHandler.Object);
				BrowserInteropStub._TriggerPageLoaded();

				var action = new TestAction();
				subject.Dispatch(action);

				mockIncompatibleEffectHandler
					.Verify(x => x.HandleAsync(It.IsAny<object>(), It.IsAny<IDispatcher>()), Times.Never);
				mockCompatibleEffectHandler
					.Verify(x => x.HandleAsync(action, It.IsAny<IDispatcher>()), Times.Once);
			}
		}
	}
}

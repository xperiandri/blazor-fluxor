﻿using Blazor.Fluxor.UnitTests.MockFactories;
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
                var actionToEmit1 = new TestActionFromEffect1();
                var actionToEmit2 = new TestActionFromEffect2();
                var actionsToEmit = new object[] { actionToEmit1, actionToEmit2 };
                var subject = new Store(BrowserInteropStub);
                subject.AddFeature(mockFeature.Object);
                subject.AddEffect(new EffectThatEmitsActions<TestAction>(actionsToEmit, subject));

                BrowserInteropStub._TriggerPageLoaded();
                subject.Dispatch(new TestAction());

                mockFeature
                    .Verify(x => x.ReceiveDispatchNotificationFromStore(actionToEmit1), Times.Once);
                mockFeature
                    .Verify(x => x.ReceiveDispatchNotificationFromStore(actionToEmit2), Times.Once);
            }

            [Fact]
            public void TriggersOnlyEffectsThatHandleTheDispatchedAction()
            {
                var mockIncompatibleEffect = new Mock<IEffect>();
                mockIncompatibleEffect
                    .Setup(x => x.ShouldReactToAction(It.IsAny<object>()))
                    .Returns(false);
                var mockCompatibleEffect = new Mock<IEffect>();
                mockCompatibleEffect
                    .Setup(x => x.ShouldReactToAction(It.IsAny<object>()))
                    .Returns(true);

                var subject = new Store(BrowserInteropStub);
                subject.AddEffect(mockIncompatibleEffect.Object);
                subject.AddEffect(mockCompatibleEffect.Object);
                BrowserInteropStub._TriggerPageLoaded();

                var action = new TestAction();
                subject.Dispatch(action);

                mockIncompatibleEffect.Verify(x => x.HandleAsync(action), Times.Never);
                mockCompatibleEffect.Verify(x => x.HandleAsync(action), Times.Once);
            }
        }


    }
}

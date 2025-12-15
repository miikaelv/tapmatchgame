using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TapMatch.Tests.PlayMode.TestAssets;
using TapMatch.UnityServices;
using UnityEngine.TestTools;
using VContainer;
using Views;

namespace TapMatch.Tests.PlayMode
{
    public class ViewControllerTests : ServiceTestBase<TestViewControllerPlayMode>
    {
        protected override void CreateContext(IContainerBuilder builder)
        {
            builder.RegisterAssetService();
            builder.Register<TestViewControllerPlayMode>(Lifetime.Transient);
        }

        protected override UniTask OnUnitySetup(CancellationToken ct)
        {
            // Transient Lifetime creates a new instance every time it's resolved
           ResolveService();
           return UniTask.CompletedTask;
        }

        [UnityTest]
        public IEnumerator ViewController_can_instantiate_and_call_OnInstantiate() => UniTask.ToCoroutine(async () =>
        {
            var instantiateResult = await Service.Instantiate(CT);
            Assert.IsTrue(instantiateResult, "Instantiate returned false");
            Assert.IsTrue(Service.IsInstantiated);
            Assert.IsTrue(Service.OnInstantiateCalled);
        });
        
        [UnityTest]
        public IEnumerator ViewController_can_be_shown_after_instantiation() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsFalse(Service.OnInstantiateCalled);
            
            var instantiateResult = await Service.Instantiate(CT);
            Assert.IsTrue(instantiateResult, "Instantiate returned false");
            Assert.IsFalse(Service.OnPreShowCalled, "OnPreShow true before show called");
            Assert.IsFalse(Service.OnPreShowCalled, "OnPostShow true before show called");

            await ShowAndAssert(Service);
            Assert.IsTrue(Service.OnPreShowCalled, "OnPreShow not called after show called");
            Assert.IsTrue(Service.OnPostShowCalled, "OnPostShow not called after show");
        });
        
        [UnityTest]
        public IEnumerator ViewController_can_be_shown_without_explicit_instantiation() => UniTask.ToCoroutine(async () =>
        {
            await ShowAndAssert(Service);
            Assert.IsTrue(Service.IsInstantiated, "View not Instantiated after Show");
            Assert.IsTrue(Service.OnInstantiateCalled, "OnInstantiate not called after Show");
            Assert.IsTrue(Service.IsShown, "IsShown false after show");
        });
        
        [UnityTest]
        public IEnumerator ViewController_can_be_hidden_and_shown_again() => UniTask.ToCoroutine(async () =>
        {
            await ShowAndAssert(Service);
            await HideAndAssert(Service);
            await ShowAndAssert(Service);
        });

        private async UniTask ShowAndAssert(IViewController controller)
        {
            Assert.IsFalse(Service.IsShown);
            var showResult = await controller.Show(CT);
            Assert.IsTrue(showResult, "Show returned false");
            Assert.IsTrue(controller.IsShown, "IsShown false after show");
        }
        
        private async UniTask HideAndAssert(IViewController controller)
        {
            Assert.IsTrue(Service.IsShown, "IsShown false before calling Hide");
            var hideResult = await controller.Hide(CT);
            Assert.IsTrue(hideResult, "Hide returned false");
            Assert.IsFalse(controller.IsShown, "IsShown true after Hide");
        }
    }
}
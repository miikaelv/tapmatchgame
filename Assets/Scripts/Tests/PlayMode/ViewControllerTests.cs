using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TapMatch.Tests.PlayMode.TestAssets;
using TapMatch.Views;
using UnityEngine.TestTools;
using VContainer;

namespace TapMatch.Tests.PlayMode
{
    public class ViewControllerTests : ViewControllerTestBase<TestViewController>
    {
        protected override void CreateContext(IContainerBuilder builder)
        {
            builder.Register<TestViewController>(Lifetime.Transient);
        }

        [UnityTest]
        public IEnumerator ViewController_can_instantiate_and_call_OnInstantiate() => UniTask.ToCoroutine(async () =>
        {
            var instantiateResult = await ViewController.Instantiate(CT);
            Assert.IsTrue(instantiateResult, "Instantiate returned false");
            Assert.IsTrue(ViewController.IsInstantiated);
            Assert.IsTrue(ViewController.OnInstantiateCalled);
            Assert.IsFalse(ViewController.View.gameObject.activeSelf);
        });
        
        [UnityTest]
        public IEnumerator ViewController_can_be_shown_after_instantiation() => UniTask.ToCoroutine(async () =>
        {
            Assert.IsFalse(ViewController.OnInstantiateCalled);
            
            var instantiateResult = await ViewController.Instantiate(CT);
            Assert.IsTrue(instantiateResult, "Instantiate returned false");
            Assert.IsFalse(ViewController.OnPreShowCalled, "OnPreShow true before show called");
            Assert.IsFalse(ViewController.OnPreShowCalled, "OnPostShow true before show called");

            await ShowAndAssert(ViewController);
            Assert.IsTrue(ViewController.OnPreShowCalled, "OnPreShow not called after show called");
            Assert.IsTrue(ViewController.OnPostShowCalled, "OnPostShow not called after show");
        });
        
        [UnityTest]
        public IEnumerator ViewController_can_be_shown_without_explicit_instantiation() => UniTask.ToCoroutine(async () =>
        {
            await ShowAndAssert(ViewController);
            Assert.IsTrue(ViewController.IsInstantiated, "View not Instantiated after Show");
            Assert.IsTrue(ViewController.OnInstantiateCalled, "OnInstantiate not called after Show");
            Assert.IsTrue(ViewController.IsShown, "IsShown false after show");
        });
        
        [UnityTest]
        public IEnumerator ViewController_can_be_hidden_and_shown_again() => UniTask.ToCoroutine(async () =>
        {
            await ShowAndAssert(ViewController);
            Assert.IsFalse(ViewController.OnHideCalled, "OnHide true before Hide called");
            await HideAndAssert(ViewController);
            Assert.IsTrue(ViewController.OnHideCalled, "OnHide false after Hide called");
            await ShowAndAssert(ViewController);
        });

        private async UniTask ShowAndAssert(IViewController controller)
        {
            Assert.IsFalse(ViewController.IsShown);
            var showResult = await controller.Show(CT);
            Assert.IsTrue(showResult, "Show returned false");
            Assert.IsTrue(controller.IsShown, "IsShown false after show");
            Assert.IsTrue(ViewController.View.gameObject.activeSelf);
        }
        
        private async UniTask HideAndAssert(IViewController controller)
        {
            Assert.IsTrue(ViewController.IsShown, "IsShown false before calling Hide");
            var hideResult = await controller.Hide(CT);
            Assert.IsTrue(hideResult, "Hide returned false");
            Assert.IsFalse(controller.IsShown, "IsShown true after Hide");
            Assert.IsFalse(ViewController.View.gameObject.activeSelf);
        }
    }
}
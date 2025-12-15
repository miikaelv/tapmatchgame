using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;
using VContainer;
using VContainer.Unity;

namespace TapMatch.Tests
{
    public abstract class ServiceTestBase<T> where T : IDisposable
    {
        private LifetimeScope Scope;
        private IObjectResolver Container;
        protected T Service;

        private readonly CancellationTokenSource CTSource = new();
        protected CancellationToken CT => CTSource.Token;
        protected abstract void CreateContext(IContainerBuilder builder);
        private bool OneTimeUnitySetUpDone;

        private void BuildEnvironment()
        {
            Scope = LifetimeScope.Create(CreateContext);
            Container = Scope.Container;
            ResolveService();
        }

        protected void ResolveService()
        {
            if (Service != null) Service.Dispose();
            Service = Container.Resolve<T>();
        }

        private void ResetEnvironment()
        {
            if (Container != null) Container.Dispose();
            if (Scope != null) Scope.Dispose();

            Container = null;
            Service = default;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            BuildEnvironment();
            OnOneTimeSetup();
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            OnOneTimeTearDown();

            ResetEnvironment();
            OneTimeUnitySetUpDone = false;
            CTSource.Cancel();
            CTSource.Dispose();
        }

        [UnitySetUp]
        public IEnumerator UnitySetUp() => UniTask.ToCoroutine(async () =>
        {
            if (!OneTimeUnitySetUpDone)
            {
                OneTimeUnitySetUpDone = true;
                BuildEnvironment();
                await OnOneTimeUnitySetup(CT);
            }

            await OnUnitySetup(CT);
        });

        [UnityTearDown]
        public IEnumerator UnityTearDown() => UniTask.ToCoroutine(async () => { await OnUnityTearDown(CT); });

        [SetUp]
        public void Setup()
        {
            OnSetup();
        }

        [TearDown]
        public void TearDown()
        {
            OnTeardown();
        }

        protected virtual void OnTeardown()
        {
        }

        protected virtual void OnSetup()
        {
        }

        protected virtual UniTask OnOneTimeUnitySetup(CancellationToken ct) => UniTask.CompletedTask;
        protected virtual UniTask OnUnitySetup(CancellationToken ct) => UniTask.CompletedTask;
        protected virtual UniTask OnUnityTearDown(CancellationToken ct) => UniTask.CompletedTask;
        protected virtual void OnOneTimeSetup()
        {
        }
        protected virtual void OnOneTimeTearDown()
        {
        }
    }
}
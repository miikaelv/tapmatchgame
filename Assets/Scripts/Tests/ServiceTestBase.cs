using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;
using VContainer;

namespace TapMatch.Tests
{
    public abstract class ServiceTestBase<T>
    {
        private IObjectResolver Container;
        protected T Service;
        
        private readonly CancellationTokenSource CTSource = new ();
        protected CancellationToken CT => CTSource.Token;
        protected abstract ContainerBuilder CreateContext(ContainerBuilder builder);
        private bool OneTimeUnitySetUpDone;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var builder = CreateContext(new ContainerBuilder());
            Container = builder.Build();
            Service = Container.Resolve<T>();
            
            OnOneTimeSetup();
        }
        
        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            OnOneTimeTeardown();
            
            OneTimeUnitySetUpDone = false;
            Container = null;
            Service = default;
            
            CTSource.Cancel();
            CTSource.Dispose();
        }

        [UnitySetUp]
        public IEnumerator UnitySetUp() => UniTask.ToCoroutine(async () =>
        {
            if (!OneTimeUnitySetUpDone)
            {
                OneTimeUnitySetUpDone = true;
                await OnOneTimeUnitySetup(CT);
            }

            await OnUnitySetup(CT);
        });
        
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

        protected virtual void OnTeardown() { }
        protected virtual void OnSetup() { }
        protected virtual UniTask OnOneTimeUnitySetup(CancellationToken ct) => UniTask.CompletedTask;
        protected virtual UniTask OnUnitySetup(CancellationToken ct) => UniTask.CompletedTask;
        protected virtual void OnOneTimeSetup() { }
        protected virtual void OnOneTimeTeardown() { }
    }
}
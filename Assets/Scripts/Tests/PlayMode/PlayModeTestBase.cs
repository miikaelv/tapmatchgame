using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace TapMatch.Tests.PlayMode
{
    public abstract class PlayModeTestBase
    {
        private readonly CancellationTokenSource CTSource = new();
        protected CancellationToken CT => CTSource.Token;
        private bool OneTimeUnitySetUpDone;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OnOneTimeSetup();
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            OnOneTimeTearDown();

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
                await OnOneTimeUnitySetup(CT);
            }

            await OnUnitySetup(CT);
        });

        [UnityTearDown]
        public IEnumerator UnityTearDown() => UniTask.ToCoroutine(async () => { await OnUnityTearDown(CT); });

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
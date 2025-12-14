using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TapMatch.UnitySystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VContainer;
using Object = UnityEngine.Object;

namespace TapMatch.Tests
{
    public class SmokeTests
    {
        private IObjectResolver Resolver { get; set; }
        private IGameInstance GameInstance;

        [UnitySetUp]
        public IEnumerator UnitySetUp() => UniTask.ToCoroutine(async () =>
        {
            var loadOp = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            await loadOp.ToUniTask();

            // Find the bootstrapper in the scene
            var bootstrap = Object.FindFirstObjectByType<Bootstrap>();
            Assert.IsNotNull(bootstrap, $"{nameof(Bootstrap)} not found in scene!");

            // Assign Resolver
            Resolver = bootstrap.Container;
            Assert.IsNotNull(Resolver, $"{nameof(IObjectResolver)} is invalid!");


            // IStartable is called on Awake and should be resolved by the time the Scene loads
            GameInstance = Resolver.Resolve<IGameInstance>();
        });

        [UnityTearDown]
        public IEnumerator UnityTeardown() => UniTask.ToCoroutine(async () =>
        {
            var loadedScene = SceneManager.GetSceneAt(0);

            if (loadedScene.IsValid())
            {
                var unloadOp = SceneManager.UnloadSceneAsync(loadedScene);
                if (unloadOp != null)
                    await unloadOp.ToUniTask();
            }

            Resolver = null;
            GameInstance = null;
        });

        [UnityTest]
        public IEnumerator Can_load_game() => UniTask.ToCoroutine(async () =>
        {
            var result = await GameInstance.WaitForGameToLoad(TimeSpan.FromSeconds(5));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.success);
        });
    }
}
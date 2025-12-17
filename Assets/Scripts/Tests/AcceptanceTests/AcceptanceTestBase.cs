using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TapMatch.Views;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VContainer;
using Object = UnityEngine.Object;

namespace TapMatch.Tests.AcceptanceTests
{
    public class AcceptanceTestBase
    {
        protected IObjectResolver Resolver { get; set; }
        protected GameInstance GameInstance;
        
        // TODO: TearDown to Unload to support multiple tests
        [UnitySetUp]
        public IEnumerator UnitySetUp() => UniTask.ToCoroutine(async () =>
        {
            var loadOp = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            await loadOp.ToUniTask();

            // Find the bootstrapper in the scene
            var bootstrap = Object.FindFirstObjectByType<Bootstrap>();
            Assert.IsNotNull(bootstrap, $"{nameof(Bootstrap)} not found in scene!");

            // Assign Resolver
            Resolver = bootstrap.Container;
            Assert.IsNotNull(Resolver, $"{nameof(IObjectResolver)} is invalid!");

            // IStartable is called on Awake and should be resolved by the time the Scene loads
            GameInstance = Resolver.Resolve<GameInstance>();
            await GameInstance.WaitForGameToLoad(TimeSpan.FromSeconds(5));
        });
    }
}
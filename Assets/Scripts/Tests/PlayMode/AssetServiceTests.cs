using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TapMatch.Tests.PlayMode.TestAssets;
using TapMatch.UnityServices;
using TapMatch.Views.ScriptableAssets;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace TapMatch.Tests.PlayMode
{
    public class AssetServiceTests : ServiceTestBase<AssetService>
    {
        protected override void CreateContext(IContainerBuilder builder)
        {
            builder.RegisterAssetService();
        }

        protected override async UniTask OnOneTimeUnitySetup(CancellationToken ct)
        {
            await base.OnOneTimeUnitySetup(ct);
            await Service.Initialize(ct);
        }

        protected override async UniTask OnUnityTearDown(CancellationToken ct)
        {
            await base.OnUnityTearDown(ct);
            Service.UnloadAll();
        }
        
        [UnityTest]
        public IEnumerator LoadAsset_loads_successfully() => UniTask.ToCoroutine(async () =>
        {
            var view = await Service.LoadAsset<GameObject>(nameof(TestAsset), CT);

            Assert.IsNotNull(view);
            Assert.IsInstanceOf<GameObject>(view);
            Assert.AreEqual(view.name, nameof(TestAsset));
        });
        
        [UnityTest]
        public IEnumerator LoadScriptable_loads_successfully() => UniTask.ToCoroutine(async () =>
        {
            var view = await Service.LoadScriptableObject<GridConfiguration>(CT);

            Assert.IsNotNull(view);
            Assert.IsInstanceOf<GridConfiguration>(view);
        });

        [UnityTest]
        public IEnumerator LoadView_loads_successfully() => UniTask.ToCoroutine(async () =>
        {
            var view = await Service.LoadViewComponent<TestAsset>(nameof(TestAsset), CT);

            Assert.IsNotNull(view);
            Assert.IsInstanceOf<TestAsset>(view);
        });
        
        [UnityTest]
        public IEnumerator LoadSingletonView_loads_successfully() => UniTask.ToCoroutine(async () =>
        {
            var view = await Service.LoadSingletonView<TestAsset>(CT);

            Assert.IsNotNull(view);
            Assert.IsInstanceOf<TestAsset>(view);
        });

        [UnityTest]
        public IEnumerator LoadSingleton_multiples_return_same_instance() => UniTask.ToCoroutine(async () =>
        {
            var view1 = await Service.LoadSingletonView<TestAsset>(CT);
            var view2 = await Service.LoadSingletonView<TestAsset>(CT);

            Assert.AreSame(view1, view2);
        });

        [UnityTest]
        public IEnumerator LoadView_wrong_component_throws()
        {
            Exception caught = null;

            yield return UniTask.ToCoroutine(async () =>
            {
                try
                {
                    await Service.LoadViewComponent<BoxCollider>(nameof(TestAsset), CT);
                }
                catch (Exception e)
                {
                    caught = e;
                }
            });

            Assert.IsInstanceOf<ArgumentException>(caught);
        }
    }
}
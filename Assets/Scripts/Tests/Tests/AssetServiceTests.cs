using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TapMatch.Tests.TestAssets;
using TapMatch.UnityServices;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace TapMatch.Tests.Tests
{
    public class AssetServiceTests : ServiceTestBase<AssetService>
    {
        protected override void CreateContext(IContainerBuilder builder)
        {
            builder.RegisterAssetService();
        }

        protected override async UniTask OnOneTimeUnitySetup(CancellationToken ct)
        {
            await Service.Initialize(ct);
        }

        [UnityTest]
        public IEnumerator LoadAsset_loads_successfully() => UniTask.ToCoroutine(async () =>
        {
            var view = await Service.LoadAsset<TestAsset>(nameof(TestAsset), CT);

            Assert.IsNotNull(view);
            Assert.IsInstanceOf<TestAsset>(view);
        });

        [UnityTest]
        public IEnumerator LoadSingleton_loads_successfully() => UniTask.ToCoroutine(async () =>
        {
            var view = await Service.LoadSingletonAsset<TestAsset>(CT);

            Assert.IsNotNull(view);
            Assert.IsInstanceOf<TestAsset>(view);
        });

        [UnityTest]
        public IEnumerator LoadSingleton_multiples_return_same_instance() => UniTask.ToCoroutine(async () =>
        {
            var view1 = await Service.LoadSingletonAsset<TestAsset>(CT);
            var view2 = await Service.LoadSingletonAsset<TestAsset>(CT);

            Assert.AreSame(view1, view2);
        });

        [UnityTest]
        public IEnumerator LoadAsset_wrong_component_throws()
        {
            yield return Assert.ThrowsAsync<System.ArgumentException>(async () =>
            {
                await Service.LoadAsset<BoxCollider>(nameof(TestAsset), CT);
            });
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.UnityServices;
using Views;

namespace TapMatch.Tests.PlayMode.TestAssets
{
    public class TestViewControllerPlayMode : ViewControllerPlayMode<TestAsset>
    {
        public bool OnInstantiateCalled { get; private set; }
        public bool OnPreShowCalled { get; private set; }
        public bool OnPostShowCalled { get; private set; }
        public bool OnHideCalled { get; private set; }

        public TestViewControllerPlayMode(IAssetService assetService) : base(assetService)
        {
        }

        protected override UniTask<bool> OnPreShow(CancellationToken ct)
        {
            OnPreShowCalled = true;
            return base.OnPreShow(ct);
        }

        protected override UniTask<bool> OnPostShow(CancellationToken ct)
        {
            OnPostShowCalled = true;
            return base.OnPostShow(ct);
        }

        protected override UniTask<bool> OnInstantiate(CancellationToken ct)
        {
            OnInstantiateCalled = true;
            return UniTask.FromResult(true);
        }
    }
}
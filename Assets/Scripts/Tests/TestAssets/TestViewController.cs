using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.UnityServices;
using TapMatch.Views;

namespace TapMatch.Tests.PlayMode.TestAssets
{
    public class TestViewController : ViewController<TestAsset>
    {
        public bool OnInstantiateCalled { get; private set; }
        public bool OnPreShowCalled { get; private set; }
        public bool OnPostShowCalled { get; private set; }
        public bool OnHideCalled { get; private set; }

        public TestViewController(IAssetService assetService, IUIRoot uiRoot, IInputService inputService) : base(
            assetService, uiRoot, inputService)
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

        protected override UniTask<bool> OnHide(CancellationToken ct)
        {
            OnHideCalled = true;
            return base.OnHide(ct);
        }
    }
}
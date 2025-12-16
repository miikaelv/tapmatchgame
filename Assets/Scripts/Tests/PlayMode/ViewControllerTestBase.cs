using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.UnityServices;
using TapMatch.Views;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace TapMatch.Tests.PlayMode
{
    public abstract class ViewControllerTestBase<T> : PlayModeTestBase where T : IDisposable
    {
        private UIRoot UIRoot;
        private InputService InputService;
        private LifetimeScope Scope;
        private IObjectResolver Container;
        protected T ViewController;
        protected AssetService AssetService;

        private void BuildEnvironment()
        {
            Scope = LifetimeScope.Create(CreateViewControllerContext);
            Container = Scope.Container;
            ResolveServices();
            AssetService = Container.Resolve<AssetService>();
        }

        private void CreateViewControllerContext(IContainerBuilder builder)
        {
            builder.Register<MockGlobalCT>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterInstance(UIRoot).As<IUIRoot>();
            builder.RegisterInstance(InputService).As<IInputService>();
            builder.RegisterModelService();
            builder.RegisterAssetService();
            CreateContext(builder);
        }

        protected override UniTask OnUnitySetup(CancellationToken ct)
        {
            ResolveServices();
            return base.OnUnitySetup(ct);
        }

        protected override UniTask OnUnityTearDown(CancellationToken ct)
        {
            ViewController.Dispose();
            AssetService.UnloadAll();
            return base.OnUnityTearDown(ct);
        }

        protected abstract void CreateContext(IContainerBuilder builder);

        protected override async UniTask OnOneTimeUnitySetup(CancellationToken ct)
        {
            await base.OnOneTimeUnitySetup(ct);
            await SceneManager.LoadSceneAsync("ViewControllerTestScene").ToUniTask(cancellationToken: ct);
            UIRoot = Object.FindFirstObjectByType<UIRoot>();
            InputService = Object.FindFirstObjectByType<InputService>();
            BuildEnvironment();
            
            await Container.Resolve<ModelService>().Initialize(ct);
        }

        private void ResolveServices()
        {
            ViewController = Container.Resolve<T>();
        }
    }
}
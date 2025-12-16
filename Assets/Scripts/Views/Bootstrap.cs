using TapMatch.UnityServices;
using VContainer;
using VContainer.Unity;

namespace TapMatch.Views
{
    public class Bootstrap : LifetimeScope
    {
        public UIRoot UIRoot;
        public InputService InputService;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInstance>().As<IGameInstance>().As<IGlobalCT>();
            
            builder.RegisterInstance(UIRoot).As<IUIRoot>();
            builder.RegisterInstance(InputService).As<IInputService>();
            
            builder.RegisterAssetService();
            
            builder.Register<GridWindowController>(Lifetime.Singleton).As<IGridWindowController>();
        }
    }
}

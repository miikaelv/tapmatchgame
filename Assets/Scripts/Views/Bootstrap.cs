using TapMatch.UnityServices;
using VContainer;
using VContainer.Unity;

namespace TapMatch.Views
{
    public class Bootstrap : LifetimeScope
    {
        public UIRoot UIRoot;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInstance>().As<IGameInstance>();
            
            builder.RegisterInstance(UIRoot).As<IUIRoot>();
            
            builder.RegisterAssetService();
            
            builder.Register<GridWindowController>(Lifetime.Singleton).As<IGridWindowController>();
        }
    }
}

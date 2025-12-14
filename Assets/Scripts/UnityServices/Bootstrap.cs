using VContainer;
using VContainer.Unity;

namespace TapMatch.UnityServices
{
    public class Bootstrap : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInstance>().As<IGameInstance>();

            builder.Register<AssetService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}

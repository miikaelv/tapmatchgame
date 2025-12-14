using VContainer;
using VContainer.Unity;

namespace TapMatch.UnitySystems
{
    public class Bootstrap : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInstance>().As<IGameInstance>();
        }
    }
}

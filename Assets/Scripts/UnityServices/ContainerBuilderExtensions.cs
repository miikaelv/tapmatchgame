using VContainer;

namespace TapMatch.UnityServices
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterAssetService(this IContainerBuilder builder)
        {
            builder.Register<AssetService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}
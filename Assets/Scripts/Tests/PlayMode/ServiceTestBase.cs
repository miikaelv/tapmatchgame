using System;
using VContainer;
using VContainer.Unity;

namespace TapMatch.Tests.PlayMode
{
    public abstract class ServiceTestBase<T> : PlayModeTestBase where T : IDisposable
    {
        private LifetimeScope Scope;
        private IObjectResolver Container;
        protected T Service;
        protected abstract void CreateContext(IContainerBuilder builder);

        protected override void BuildEnvironment()
        {
            Scope = LifetimeScope.Create(CreateContext);
            Container = Scope.Container;
            ResolveService();
        }

        protected void ResolveService()
        {
            if (Service != null) Service.Dispose();
            Service = Container.Resolve<T>();
        }

        protected override void ResetEnvironment()
        {
            if (Container != null) Container.Dispose();
            if (Scope != null) Scope.Dispose();

            Container = null;
            Service = default;
        }
    }
}
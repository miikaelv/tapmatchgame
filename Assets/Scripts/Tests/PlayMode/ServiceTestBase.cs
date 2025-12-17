using System;
using VContainer;
using VContainer.Unity;

namespace TapMatch.Tests.PlayMode
{
    /// <summary>
    /// Creates it's own Scope to test Services individually
    /// </summary>
    public abstract class ServiceTestBase<T> : PlayModeTestBase where T : IDisposable
    {
        private LifetimeScope Scope;
        private IObjectResolver Container;
        protected T Service;
        protected abstract void CreateContext(IContainerBuilder builder);

        private void BuildEnvironment()
        {
            Scope = LifetimeScope.Create(CreateContext);
            Container = Scope.Container;
            ResolveService();
        }

        protected override void OnOneTimeSetup()
        {
            BuildEnvironment();
        }

        protected override void OnOneTimeTearDown()
        {
            ResetEnvironment();
        }

        private void ResolveService()
        {
            if (Service != null) Service.Dispose();
            Service = Container.Resolve<T>();
        }

        private void ResetEnvironment()
        {
            if (Container != null) Container.Dispose();
            if (Scope != null) Scope.Dispose();

            Container = null;
            Service = default;
        }
    }
}
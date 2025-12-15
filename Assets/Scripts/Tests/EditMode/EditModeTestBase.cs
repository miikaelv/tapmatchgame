using NUnit.Framework;

namespace TapMatch.Tests.EditMode
{
    public abstract class EditModeTestBase
    {
        protected virtual void BuildEnvironment()
        {
        }

        protected virtual void ResetEnvironment()
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            BuildEnvironment();
            OnOneTimeSetup();
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            OnOneTimeTearDown();
            ResetEnvironment();
        }

        [SetUp]
        public void Setup()
        {
            OnSetup();
        }

        [TearDown]
        public void TearDown()
        {
            OnTeardown();
        }

        protected virtual void OnTeardown()
        {
        }

        protected virtual void OnSetup()
        {
        }
        protected virtual void OnOneTimeSetup()
        {
        }

        protected virtual void OnOneTimeTearDown()
        {
        }
    }
}
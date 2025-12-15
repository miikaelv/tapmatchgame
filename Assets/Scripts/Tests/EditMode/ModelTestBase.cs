namespace TapMatch.Tests.EditMode
{
    public abstract class ModelTestBase<T> : EditModeTestBase
    {
        protected T Model { get; private set; }

        protected abstract T CreateModelOnSetup();

        protected override void OnSetup()
        {
            Model = CreateModelOnSetup();
        }
    }
}
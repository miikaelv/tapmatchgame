using System.Threading;
using TapMatch.Views;
using TapMatch.Views.GameInstance;

namespace TapMatch.Tests.PlayMode
{
    public class MockGlobalCT : IGlobalCT
    {
        private readonly CancellationTokenSource GlobalCTSource = new();
        public CancellationToken GlobalCT => GlobalCTSource.Token;
    }
}
using System.Threading;
using TapMatch.Views;

namespace TapMatch.Tests.PlayMode
{
    public class MockGlobalCT : IGlobalCT
    {
        private readonly CancellationTokenSource GlobalCTSource = new();
        public CancellationToken GlobalCT => GlobalCTSource.Token;
    }
}
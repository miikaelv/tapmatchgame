using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace TapMatch.UnitySystems
{
    public interface IGameInstance
    {
        public UniTask<(bool success, TimeSpan timeToLoad)> WaitForGameToLoad(TimeSpan timeout);
        public bool GameLoaded { get; }
    }

    public class GameInstance : IStartable, IGameInstance
    {
        public bool GameLoaded { get; private set; }
        private readonly CancellationTokenSource GlobalCTSource = new();
        private CancellationToken GlobalCT => GlobalCTSource.Token;

        private DateTime TimeGameLoadStarted;

        void IStartable.Start()
        {
            Debug.Log($"{nameof(GameInstance)} Start");
            LoadGame(GlobalCT).Forget();
        }

        private async UniTask LoadGame(CancellationToken ct)
        {
            TimeGameLoadStarted = DateTime.UtcNow;

            await InitializeServices(ct);
            await LoadView(ct);

            GameLoaded = true;
            var timeToLoad = TimeGameLoadStarted - DateTime.UtcNow;
            Debug.Log($"{nameof(GameInstance)} LoadTime {(int)timeToLoad.TotalMilliseconds}ms");
        }

        private UniTask InitializeServices(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        private UniTask LoadView(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask<(bool success, TimeSpan timeToLoad)> WaitForGameToLoad(TimeSpan timeout)
        {
            try
            {
                await UniTask.WaitUntil(() => GameLoaded, cancellationToken: GlobalCT)
                    .Timeout(timeout);

                var timeToLoad = DateTime.UtcNow - TimeGameLoadStarted;
                return (true, timeToLoad);
            }
            catch (TimeoutException e)
            {
                throw e;
            }
        }
    }
}
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace TapMatch.UnityServices
{
    public interface IGameInstance
    {
        public UniTask<TimeSpan> WaitForGameToLoad(TimeSpan timeout);
        public bool GameLoadComplete { get; }
    }

    public class GameInstance : IStartable, IGameInstance, IDisposable
    {
        public bool GameLoadComplete { get; private set; }
        private readonly CancellationTokenSource GlobalCTSource = new();
        private CancellationToken GlobalCT => GlobalCTSource.Token;
        private DateTime TimeGameLoadStarted;
        private DateTime TimeGameLoadFinished;

        private readonly AssetService AssetService;

        public GameInstance(AssetService assetService)
        {
            AssetService = assetService;
        }

        void IStartable.Start()
        {
            Debug.Log($"{nameof(GameInstance)} Start");
            LoadGame(GlobalCT).Forget();
        }

        private async UniTask LoadGame(CancellationToken ct)
        {
            try
            {
                TimeGameLoadStarted = DateTime.UtcNow;

                await InitializeServices(ct);
                await LoadView(ct);

                TimeGameLoadFinished = DateTime.UtcNow;
                GameLoadComplete = true;
                var timeToLoad = TimeGameLoadFinished - TimeGameLoadStarted;
                Debug.Log($"{nameof(GameInstance)} LoadTime {(int)timeToLoad.TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Game failed to load: {ex}");
                TimeGameLoadFinished = DateTime.UtcNow;
                GameLoadComplete = true;
                Dispose();
                throw;
            }
        }

        private async UniTask InitializeServices(CancellationToken ct)
        {
            await AssetService.Initialize(ct);
        }

        private UniTask LoadView(CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask<TimeSpan> WaitForGameToLoad(TimeSpan timeout)
        {
            await UniTask.WaitUntil(() => GameLoadComplete, cancellationToken: GlobalCT)
                .Timeout(timeout);
            
            var loadFinished = TimeGameLoadFinished != default ? TimeGameLoadFinished : DateTime.UtcNow;
            var timeToLoad = loadFinished - TimeGameLoadStarted;
            return timeToLoad;
        }

        public void Dispose()
        {
            GlobalCTSource?.Cancel();
            GlobalCTSource?.Dispose();
        }
    }
}
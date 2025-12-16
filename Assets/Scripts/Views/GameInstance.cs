using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.UnityServices;
using UnityEngine;
using VContainer.Unity;

namespace TapMatch.Views
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
        private readonly IGridWindowController GridWindowController;

        public GameInstance(AssetService assetService, IGridWindowController gridWindowController)
        {
            AssetService = assetService;
            GridWindowController = gridWindowController;
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

        private async UniTask LoadView(CancellationToken ct)
        {
            await GridWindowController.Show(ct);
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
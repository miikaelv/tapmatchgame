using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.UnityServices;
using UnityEngine;
using VContainer.Unity;

namespace TapMatch.Views
{
    public interface IGlobalCT
    {
        public CancellationToken GlobalCT { get; }
    }
    
    public interface IGameInstance
    {
        public UniTask<TimeSpan> WaitForGameToLoad(TimeSpan timeout);
        public bool GameLoadComplete { get; }
    }

    /// <summary>
    /// Game entry point, Initializes services, calls first View to start game.
    /// </summary>
    public class GameInstance : IStartable, IGameInstance, IDisposable, IGlobalCT
    {
        public bool GameLoadComplete { get; private set; }
        
        //CancellationToken used by all UniTasks in the whole game in order to stop everything when wanted.
        private readonly CancellationTokenSource GlobalCTSource = new();
        public CancellationToken GlobalCT => GlobalCTSource.Token;
        private DateTime TimeGameLoadStarted;
        private DateTime TimeGameLoadFinished;

        public readonly AssetService AssetService;
        public readonly ModelService ModelService;
        public readonly GridWindowController GridWindowController;
        public readonly IInputService InputService;

        public GameInstance(AssetService assetService, ModelService modelService,  GridWindowController gridWindowController, IInputService inputService)
        {
            AssetService = assetService;
            ModelService = modelService;
            GridWindowController = gridWindowController;
            InputService = inputService;
        }

        void IStartable.Start()
        {
            Debug.Log($"{nameof(GameInstance)} Start GameInstance");
            LoadGame(GlobalCT).Forget();
        }

        private async UniTask LoadGame(CancellationToken ct)
        {
            try
            {
                using var _ = InputService.BlockInputInScope();
                
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
            await ModelService.Initialize(ct);
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

        /// <summary>
        /// Cancel GlobalCT
        /// </summary>
        public void Dispose()
        {
            GlobalCTSource?.Cancel();
            GlobalCTSource?.Dispose();
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.Models;
using TapMatch.Models.Utility;
using TapMatch.UnityServices.Actions;
using TapMatch.UnityServices.ScriptableAssets;

namespace TapMatch.UnityServices
{
    public interface IModelService
    {
        /// <summary>
        /// Readonly version of game data for Views
        /// </summary>
        public IGameStateReader GameStateReader { get; }

        /// <summary>
        /// All logic that modifies data should be run through here with Actions.
        /// </summary>
        public Result<TResult> CallAction<TResult>(GameAction<TResult> action)
            where TResult : IGameActionResult;
    }

    /// <summary>
    /// Creates, stores and handles all calls to modify or read data.
    /// </summary>
    public class ModelService : IModelService
    {
        private readonly IAssetService AssetService;

        private GameState GameState;
        public IGameStateReader GameStateReader => GameState;

        public ModelService(IAssetService assetService)
        {
            AssetService = assetService;
        }

        public async UniTask Initialize(CancellationToken ct)
        {
            // Should replace this with loading JSON from disk or server, but works for this prototype
            var configAsset = await AssetService.LoadScriptableObject<GridConfiguration>(ct);
            GameState = new GameState(configAsset.GridConfig);
        }

        // Synchronous for now, but idea is to modify as async in a Command pattern to handle server calls
        // and avoid race conditions.
        public Result<TResult> CallAction<TResult>(GameAction<TResult> action)
            where TResult : IGameActionResult
        {
            return action.Execute(GameState);
        }
    }
}
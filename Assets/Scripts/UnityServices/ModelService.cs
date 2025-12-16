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
        public IGameStateReader GameStateReader { get; }

        public Result<TResult> CallAction<TResult>(GameAction<TResult> action)
            where TResult : IGameActionResult;
    }

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

        public Result<TResult> CallAction<TResult>(GameAction<TResult> action)
            where TResult : IGameActionResult
        {
            return action.Execute(GameState);
        }
    }
}
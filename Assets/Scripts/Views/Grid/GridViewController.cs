using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.Models;
using TapMatch.Models.Actions;
using TapMatch.Models.Configs;
using TapMatch.Models.Utility;
using TapMatch.UnityServices;
using TapMatch.Views.ScriptableConfigs;
using TapMatch.Views.Utility;
using UnityEngine;

namespace TapMatch.Views.Grid
{
    public interface IGridWindowController : IViewController
    {
    }

    public class GridWindowController : ViewController<GridWindow>, IGridWindowController
    {
        private readonly IModelService ModelService;
        public IGridReader GridReader => ModelService.GameStateReader.GridReader;

        public readonly Dictionary<Guid, MatchableView> Matchables = new();
        public Dictionary<Coordinate, Vector3> GridPositions = new();

        private MatchableViewPool MatchableViewPool;
        public Dictionary<MatchableType, Color> MatchableColorDictionary;

        public GridWindowController(IAssetService assetService, IUIRoot uiRoot, IInputService inputService,
            IModelService modelService) : base(
            assetService, uiRoot, inputService)
        {
            ModelService = modelService;
        }

        protected override async UniTask<bool> OnInstantiate(CancellationToken ct)
        {
            await base.OnInstantiate(ct);

            var colorConfig = await AssetService.LoadScriptableObject<MatchableColorConfiguration>(ct);
            MatchableColorDictionary = colorConfig.GetMatchableColorDictionary();

            var cellSize = (int)View.GridBaseRect.rect.width / GridReader.Width - View.MatchablePrefab.CellSizeOffset;
            MatchableViewPool = new MatchableViewPool(View.MatchablePrefab, View.MatchablePoolParent, AssetService,
                cellSize, OnMatchablePressed);
            var matchableData = GridReader.GetAllMatchables();
            GridPositions = View.CreateGridPositions(GridReader.Width, GridReader.Height);
            CreateMatchables(matchableData);

            return true;
        }

        private async UniTask OnMatchablePressed(Coordinate coordinate, CancellationToken ct)
        {
            using var _ = InputService.BlockInputInScope();

            if (!GridReader.TryGetMatchableAtPosition(coordinate, out var matchable))
            {
                return;
            }

            var result = ModelService.CallAction(TapMatchAction.Create(coordinate));

            if (!result.TryGetModel(out var tapMatchData))
                return;

            foreach (var toDestroy in tapMatchData.DestroyedMatchableIds)
            {
                RemoveMatchable(toDestroy);
            }

            await UniTask.Delay(200, cancellationToken: ct);

            foreach (var gravityMovement in tapMatchData.GravityMovedMatchables)
            {
                MoveMatchableToPosition(gravityMovement);
            }

            await UniTask.Delay(200, cancellationToken: ct);

            foreach (var newMatchableColumn in tapMatchData.GeneratedMatchables)
            {
                foreach (var newMatchable in newMatchableColumn.NewTiles)
                {
                    CreateNewMatchable(newMatchable.matchable, newMatchable.targetCoordinate);
                }
            }
        }

        private void RemoveMatchable(Guid id)
        {
            if (!Matchables.Remove(id, out var matchable))
            {
                Debug.LogError($"Couldn't find Matchable {id} to Remove in View.");
                return;
            }

            MatchableViewPool.ReturnToPool(matchable);
        }

        private void CreateMatchables(Dictionary<Coordinate, MatchableModel> matchables)
        {
            foreach (var matchable in matchables)
                CreateNewMatchable(matchable.Value, matchable.Key);
        }

        private void CreateNewMatchable(MatchableModel matchable, Coordinate coordinate)
        {
            var matchableView = MatchableViewPool.GetFromPool();
            matchableView.SetMatchableType(matchable.Type, MatchableColorDictionary);
            SetMatchableToPosition(matchableView, coordinate);
            Matchables.Add(matchable.Id, matchableView);
        }

        private void MoveMatchableToPosition(GravityMovement gravityMovement)
        {
            if (!Matchables.TryGetValue(gravityMovement.MatchableId, out var matchable))
            {
                Debug.LogError($"Couldn't find Matchable {gravityMovement.MatchableId.ToString()} to move.");
                return;
            }

            SetMatchableToPosition(matchable, gravityMovement.EndCoordinate);
        }

        private void SetMatchableToPosition(MatchableView matchableView, Coordinate coordinate)
        {
            if (!GridPositions.TryGetValue(coordinate, out var position))
            {
                Debug.LogError($"Coordinate {coordinate} has no position on Grid.");
                return;
            }

            matchableView.MoveToTransformPosition(coordinate, position, View.GridBaseRect);
        }

        public async UniTask<bool> PressMatchableAtCoordinate(Coordinate coordinate)
        {
            var matchable = Matchables.Values.FirstOrDefault(m => m.Coordinate == coordinate);

            if (matchable == null)
                return false;

            await matchable.Press();

            return true;
        }
    }
}
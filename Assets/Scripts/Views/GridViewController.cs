using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.Models;
using TapMatch.Models.Configs;
using TapMatch.Models.Utility;
using TapMatch.UnityServices;
using TapMatch.Views.ScriptableAssets;
using TapMatch.Views.Utility;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TapMatch.Views
{
    public interface IGridWindowController : IViewController
    {
    }

    public class GridWindowController : ViewController<GridWindow>, IGridWindowController
    {
        public GridModel GridModel;

        public readonly Dictionary<Guid, MatchableView> Matchables = new();
        public Dictionary<Coordinate, Transform> GridPositions = new();

        private MatchableViewPool MatchableViewPool;
        public Dictionary<MatchableType, Color> MatchableColorDictionary;

        public GridWindowController(IAssetService assetService, IUIRoot uiRoot) : base(assetService, uiRoot)
        {
        }

        protected override async UniTask<bool> OnInstantiate(CancellationToken ct)
        {
            await base.OnInstantiate(ct);

            var colorConfig = await AssetService.LoadScriptableObject<MatchableColorConfiguration>(ct);
            MatchableColorDictionary = colorConfig.GetMatchableColorDictionary();

            // TODO: Temporarily here before I add ModelService
            var gridConfig = await AssetService.LoadScriptableObject<GridConfiguration>(ct);
            GridModel = new GridModel(gridConfig.GridConfig, new System.Random());
            
            var cellSize = (int)View.GridBaseRect.rect.width / GridModel.Width - View.MatchablePrefab.CellSizeOffset;
            MatchableViewPool = new MatchableViewPool(View.MatchablePrefab, View.MatchablesParent, cellSize, OnMatchablePressed);
            var matchableData = GridModel.GetAllMatchables();
            GridPositions = CreateGridPositions(GridModel.Width, GridModel.Height, cellSize);
            CreateMatchables(matchableData);

            return true;
        }

        private async UniTask OnMatchablePressed(Coordinate coordinate, CancellationToken ct)
        {
            if (!GridModel.TryGetMatchableAtPosition(coordinate, out var matchable))
            {
                return;
            }

            var destroyedMatchableCoordinates = GridModel.GetConnectingMatchables(coordinate);
            var destroyedMatchableIds = GridModel.ClearMatchables(destroyedMatchableCoordinates);
            var gravityMovedMatchables = GridModel.ApplyGravity();
            var generatedMatchables = GridModel.RefillEmptySpaces(new System.Random());

            foreach (var toDestroy in destroyedMatchableIds)
            {
                RemoveMatchable(toDestroy);
            }

            await UniTask.Delay(200, cancellationToken: ct);

            foreach (var gravityMovement in gravityMovedMatchables)
            {
                MoveMatchableToPosition(gravityMovement);
            }

            await UniTask.Delay(200, cancellationToken: ct);

            foreach (var newMatchableColumn in generatedMatchables)
            {
                foreach (var newMatchable in newMatchableColumn.NewTiles)
                {
                    if (!TryCreateNewMatchable(newMatchable.matchable, newMatchable.targetCoordinate))
                    {
                        Debug.LogError(
                            $"Could not create new matchable of type {newMatchable.matchable.ToString()} at {newMatchable.matchable}");
                    }
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
            {
                if (!TryCreateNewMatchable(matchable.Value, matchable.Key))
                    continue;
            }
        }

        private bool TryCreateNewMatchable(MatchableModel matchable, Coordinate coordinate)
        {
            if (!MatchableColorDictionary.TryGetValue(matchable.Type, out var color))
            {
                Debug.LogError($"{matchable.ToString()} has no Color in ColorData.");
                return false;
            }

            var matchableView = MatchableViewPool.GetFromPool();
            matchableView.Type = matchable.Type;
            matchableView.ColorImage.color = color;
            SetMatchableToPosition(matchableView, coordinate);
            Matchables.Add(matchable.Id, matchableView);

            return true;
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

            matchableView.SetParent(position);
            matchableView.RectTransform.localPosition = Vector3.zero;
            matchableView.Coordinate = coordinate;
        }

        // TODO: Refactor to only handle positions as Vector3 instead of Transforms, supremely slow atm
        private Dictionary<Coordinate, Transform> CreateGridPositions(int width, int height, int cellSize)
        {
            var dict = new Dictionary<Coordinate, Transform>();

            if (View.GridBase == null)
            {
                Debug.LogError("GridBase is not assigned!", View);
                return dict;
            }

            if (View.TileBase == null)
            {
                Debug.LogError("TileBase is not assigned!", View);
                return dict;
            }

            View.GridBase.cellSize = new Vector2(cellSize, cellSize);
            View.GridBase.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            View.GridBase.constraintCount = width;

            for (var i = View.GridBase.transform.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(View.GridBase.transform.GetChild(i).gameObject);

            // Instantiate width * height empty GameObjects
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var tile = Object.Instantiate(View.TileBase, View.GridBase.transform, false);
                    dict.Add(new Coordinate(x, y), tile);
                }
            }

            return dict;
        }

        public bool PressMatchableAtCoordinate(Coordinate coordinate)
        {
            var matchable = Matchables.Values.FirstOrDefault(m => m.Coordinate == coordinate);

            if (matchable == null)
                return false;
            
            matchable.OnPointerDown(null);

            return true;
        }
    }
}
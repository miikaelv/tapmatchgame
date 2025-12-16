using System;
using System.Collections.Generic;
using TapMatch.Models;
using TapMatch.Models.Utility;

namespace TapMatch.UnityServices.Actions
{
    public class TapMatchAction : GameAction<TapMatchResult>
    {
        public static TapMatchAction Create(Coordinate coordinate) => new (coordinate);
        private readonly Coordinate TapCoordinate;

        public TapMatchAction(Coordinate tapCoordinate)
        {
            TapCoordinate = tapCoordinate;
        }

        protected override Result<bool> CanExecute(GameState state)
        {
            // Does not need to run here necessarily, but here for demonstration
            var isGridValid = state.GridModel.ValidateGrid();

            return isGridValid ? true.ToResult() : Result<bool>.GenericError("Grid is invalid");
        }

        protected override Result<TapMatchResult> ExecuteInternal(GameState state)
        {
            var destroyedMatchableCoordinates = state.GridModel.GetConnectingMatchables(TapCoordinate);
            var destroyedMatchableIds = state.GridModel.ClearMatchables(destroyedMatchableCoordinates);
            var gravityMovedMatchables = state.GridModel.ApplyGravity();
            var generatedMatchables = state.GridModel.RefillEmptySpaces(new System.Random());

            return new TapMatchResult(destroyedMatchableIds, gravityMovedMatchables, generatedMatchables).ToResult();
        }
    }

    public class TapMatchResult : IGameActionResult
    {
        public readonly IReadOnlyList<Guid> DestroyedMatchableIds;
        public readonly IReadOnlyList<GravityMovement> GravityMovedMatchables;
        public readonly IReadOnlyList<NewTilesColumn> GeneratedMatchables;

        public TapMatchResult(List<Guid> destroyedMatchableIds, List<GravityMovement> gravityMovedMatchables,
            List<NewTilesColumn> generatedMatchables)
        {
            DestroyedMatchableIds = destroyedMatchableIds;
            GravityMovedMatchables = gravityMovedMatchables;
            GeneratedMatchables = generatedMatchables;
        }
    }
}
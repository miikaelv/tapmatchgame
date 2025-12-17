using System;
using System.Collections.Generic;
using TapMatch.Models.Utility;

namespace TapMatch.Models.Actions
{
    public class TapMatchAction : GameAction<TapMatchResult>
    {
        // Alternative to constructor just to look nicer.
        public static TapMatchAction Create(Coordinate coordinate) => new (coordinate);
        private readonly Coordinate TapCoordinate;

        public TapMatchAction(Coordinate tapCoordinate)
        {
            TapCoordinate = tapCoordinate;
        }

        protected override Result<bool> CanExecute(GameState state)
        {
            // Does not need to run here necessarily, but here for CanExecute demonstration
            var isGridValid = state.GridModel.ValidateGrid();
            
            return isGridValid ? true.ToResult() : Result<bool>.GenericError("Grid is invalid");
        }

        // TradeOff: Does not currently handle mid-logic failure states.
        
        // In the future could be refactored with some type of transaction logic 
        // so either all changes are stored or none if the action fails.
        protected override Result<TapMatchResult> ExecuteInternal(GameState state)
        {
            var destroyedMatchableCoordinates = state.GridModel.GetConnectingMatchables(TapCoordinate);
            var destroyedMatchableIds = state.GridModel.ClearMatchables(destroyedMatchableCoordinates);
            var gravityMovedMatchables = state.GridModel.ApplyGravity();
            var generatedMatchables = state.GridModel.RefillEmptySpaces(new Random());

            return new TapMatchResult(destroyedMatchableIds, gravityMovedMatchables, generatedMatchables).ToResult();
        }
    }

    public readonly struct TapMatchResult : IGameActionResult
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
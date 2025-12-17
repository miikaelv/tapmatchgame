using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TapMatch.Models.Configs;
using TapMatch.Models.Utility;
using UnityEngine;
using Random = System.Random;

namespace TapMatch.Models
{
    /// <summary>
    /// ReadOnly version of Grid for Views
    /// </summary>
    public interface IGridReader
    {
        public int Width { get; }
        public int Height { get; }
        public bool IsCoordinateValidOnGrid(Coordinate coordinate);
        public bool TryGetMatchableAtPosition(Coordinate coordinate, out MatchableModel matchable);
        public Dictionary<Coordinate, MatchableModel> GetAllMatchables();
        public Coordinate GetRandomCoordinateWithinGrid(Random rng);
    }

    /// <summary>
    /// Stores Grid state and logic, could also move logic to GameState to keep this cleaner, but for now this is nice.
    /// </summary>
    [Serializable]
    public class GridModel : IGridReader
    {
        public int Width => Grid.Width;
        public int Height => Grid.Height;

        private readonly MatchableType[] ValidMatchables;
        public readonly GameGrid Grid;

        public GridModel(MatchableModel[,] grid, MatchableType[] validMatchables)
        {
            Grid = new GameGrid(grid);
            ValidMatchables = validMatchables;
        }

        public GridModel(GridConfig config, Random rng)
        {
            ValidMatchables = config.ValidMatchables.ToArray();
            Grid = new GameGrid(config.Width, config.Height);
            FillWholeGridWithMatchables(rng);
        }

        private void FillWholeGridWithMatchables(Random rng)
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Grid[x, y] = CreateRandomMatchable(rng);
                }
            }
        }

        private MatchableModel CreateRandomMatchable(Random rng)
        {
            var type = ValidMatchables[rng.Next(ValidMatchables.Length)];
            return new MatchableModel(type);
        }

        public bool IsCoordinateValidOnGrid(Coordinate coordinate) => coordinate.X >= 0 && coordinate.X < Width &&
                                                                      coordinate.Y >= 0 && coordinate.Y < Height;

        public List<Guid> ClearMatchables(IEnumerable<Coordinate> coordinates)
        {
            var cleared = new List<Guid>();
            foreach (var coordinate in coordinates)
            {
                cleared.Add(Grid[coordinate.X, coordinate.Y].Id);
                Grid[coordinate.X, coordinate.Y] = MatchableModel.Empty;
            }

            return cleared;
        }

        public bool TryGetMatchableAtPosition(Coordinate coordinate, out MatchableModel matchable)
        {
            matchable = MatchableModel.Empty;

            if (!IsCoordinateValidOnGrid(coordinate))
                return false;

            matchable = Grid[coordinate.X, coordinate.Y];

            return !matchable.IsEmpty;
        }

        // Depth First Flood Fill from target coordinate
        public List<Coordinate> GetConnectingMatchables(Coordinate start)
        {
            var matched = new List<Coordinate>();

            if (!TryGetMatchableAtPosition(start, out var startMatchable))
                return matched;

            var targetType = startMatchable.Type;

            var visited = new bool[Width, Height];
            var stack = new Stack<Coordinate>();
            stack.Push(start);
            visited[start.X, start.Y] = true;

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                matched.Add(current);

                foreach (var neighbor in current.GetOrthogonalNeighbors())
                {
                    if (!IsCoordinateValidOnGrid(neighbor))
                        continue;

                    if (visited[neighbor.X, neighbor.Y])
                        continue;

                    if (Grid[neighbor.X, neighbor.Y].Type != targetType)
                        continue;

                    stack.Push(neighbor);
                    visited[neighbor.X, neighbor.Y] = true;
                }
            }

            return matched;
        }

        public List<GravityMovement> ApplyGravity()
        {
            var movements = new List<GravityMovement>();

            for (var x = 0; x < Width; x++)
            {
                // Previous position on the Y axis where Matchable would stop falling
                var bottomPosition = 0;

                // Move up from the bottom of the Y axis
                for (var y = 0; y < Height; y++)
                {
                    var matchable = Grid[x, y];

                    // Skip if no Matchable
                    if (matchable.IsEmpty) continue;

                    // Check if the tile needs to move down
                    if (y != bottomPosition)
                    {
                        var startCoordinate = new Coordinate(x, y);
                        var endCoordinate = new Coordinate(x, bottomPosition);
                        movements.Add(new GravityMovement(matchable.Id, startCoordinate, endCoordinate));

                        Grid[x, bottomPosition] = matchable;
                        Grid[x, y] = MatchableModel.Empty;
                    }

                    // Increment the bottom position to account for the existing or fallen Matchable
                    bottomPosition++;
                }
            }

            return movements;
        }

        public List<NewTilesColumn> RefillEmptySpaces(Random rng)
        {
            var newTilesData = new List<NewTilesColumn>();

            for (var x = 0; x < Width; x++)
            {
                var newTileStack = new Stack<(Coordinate coordinate, MatchableModel matchable)>();

                // Starting from the top, for each None tile, create new Matchable and add it to tileStack
                for (var y = Height - 1; y >= 0; y--)
                {
                    if (!Grid[x, y].IsEmpty) break;

                    var newType = CreateRandomMatchable(rng);
                    newTileStack.Push(new ValueTuple<Coordinate, MatchableModel>(new Coordinate(x, y), newType));

                    Grid[x, y] = newType;
                }

                if (newTileStack.Count <= 0) continue;

                // Stack is converted to list in order of Pop
                newTilesData.Add(new NewTilesColumn(x, newTileStack.ToList()));
            }

            return newTilesData;
        }

        // For grid construction on View side
        public Dictionary<Coordinate, MatchableModel> GetAllMatchables()
        {
            var result = new Dictionary<Coordinate, MatchableModel>();

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var coord = new Coordinate(x, y);
                    result[coord] = Grid[x, y];
                }
            }

            return result;
        }

        public Coordinate GetRandomCoordinateWithinGrid(Random rng)
        {
            var x = rng.Next(0, Width);
            var y = rng.Next(0, Height);
            return new Coordinate(x, y);
        }

        public bool ValidateGrid()
        {
            var isGridValid = true;

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    if (!Grid[x, y].IsEmpty) continue;

                    Debug.LogError($"Grid has Empty Matchable at {x}:{y}");
                    isGridValid = false;
                }
            }

            return isGridValid;
        }

        public string GridToString()
        {
            var sb = new StringBuilder();

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    sb.Append(Grid[x, y].Type);

                    if (x < Width - 1)
                        sb.Append(", ");
                }

                if (y < Height - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    // X for which Column to drop from, List is ordered from first to drop to last.
    public readonly struct NewTilesColumn
    {
        public readonly int X;
        public readonly List<(Coordinate targetCoordinate, MatchableModel matchable)> NewTiles;

        public NewTilesColumn(int x, List<(Coordinate targetCoordinate, MatchableModel matchable)> newTiles)
        {
            X = x;
            NewTiles = newTiles;
        }
    }

    // Start and End Coordinate for Animations
    public readonly struct GravityMovement
    {
        public readonly Guid MatchableId;
        public readonly Coordinate StartCoordinate;
        public readonly Coordinate EndCoordinate;

        public GravityMovement(Guid id, Coordinate startCoordinate, Coordinate endCoordinate)
        {
            MatchableId = id;
            StartCoordinate = startCoordinate;
            EndCoordinate = endCoordinate;
        }
    }
}
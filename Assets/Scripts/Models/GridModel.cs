using System;
using System.Collections.Generic;
using TapMatch.Models.Configs;
using TapMatch.Models.Utility;

namespace TapMatch.Models
{
    public interface IGridReader
    {
        public int Width { get; }
        public int Height { get; }
        public bool IsCoordinateValidOnGrid(Coordinate coordinate);
        public bool TryGetMatchableAtPosition(Coordinate coordinate, out MatchableType matchable);
    }

    public class TileMovement
    {
        private readonly Coordinate StartCoordinate;
        private readonly Coordinate EndCoordinate;

        public TileMovement(Coordinate startCoordinate, Coordinate endCoordinate)
        {
            StartCoordinate = startCoordinate;
            EndCoordinate = endCoordinate;
        }
    }
    
    [Serializable]
    public class GridModel : IGridReader
    {
        public int Width { get; }
        public int Height { get; }
        private readonly MatchableType[] ValidMatchables;
        public readonly MatchableType[,] Grid;

        public GridModel(MatchableType[,] grid, MatchableType[] validMatchables)
        {
            Width = grid.GetLength(0);
            Height = grid.GetLength(1);
            Grid = grid;
            ValidMatchables = validMatchables;
        }

        public GridModel(GridConfig config, Random rng)
        {
            Width = config.Width;
            Height = config.Height;
            ValidMatchables = config.ValidMatchables.ToArray();
            Grid = CreateMatchables(rng);
        }
        
        public List<TileMovement> ApplyGravity()
        {
            var movements = new List<TileMovement>();

            for (var x = 0; x < Width; x++)
            {
                // Previous position on the Y axis where Matchable would stop falling
                var bottomPosition = 0; 

                // Move up from the bottom of the Y axis
                for (var y = 0; y < Height; y++)
                {
                    var matchable = Grid[x, y];
                
                    // Skip if no Matchable
                    if (matchable == MatchableType.None) continue;
                    
                    // Check if the tile needs to move down
                    if (y != bottomPosition)
                    {
                        var startCoordinate = new Coordinate(x, y);
                        var endCoordinate = new Coordinate(x, bottomPosition);
                        movements.Add(new TileMovement(startCoordinate, endCoordinate));

                        Grid[x, bottomPosition] = matchable;
                        Grid[x, y] = MatchableType.None;
                    }
                    
                    // Increment the bottom position to account for the existing or fallen Matchable
                    bottomPosition++;
                }
            }
        
            return movements;
        }

        private MatchableType[,] CreateMatchables(Random rng)
        {
            var matchables = new MatchableType[Width, Height];

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    matchables[x, y] = CreateRandomMatchable(rng);
                }
            }

            return matchables;
        }

        private MatchableType CreateRandomMatchable(Random rng) => ValidMatchables[rng.Next(ValidMatchables.Length)];

        public bool IsCoordinateValidOnGrid(Coordinate coordinate) => coordinate.X >= 0 && coordinate.X < Width &&
                                                                      coordinate.Y >= 0 && coordinate.Y < Height;

        public void ClearMatchables(IEnumerable<Coordinate> coordinates)
        {
            foreach (var coordinate in coordinates)
            {
                Grid[coordinate.X, coordinate.Y] = MatchableType.None;
            }
        }
        
        public bool TryGetMatchableAtPosition(Coordinate coordinate, out MatchableType matchable)
        {
            matchable = MatchableType.None;

            if (!IsCoordinateValidOnGrid(coordinate))
                return false;

            matchable = Grid[coordinate.X, coordinate.Y];

            return matchable != MatchableType.None;
        }

        // Depth First Flood Fill from target coordinate
        public List<Coordinate> GetConnectingMatchables(Coordinate start)
        {
            var matched = new List<Coordinate>();

            if (!TryGetMatchableAtPosition(start, out var targetType))
                return matched;

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

                    if (Grid[neighbor.X, neighbor.Y] != targetType)
                        continue;

                    stack.Push(neighbor);
                    visited[neighbor.X, neighbor.Y] = true;
                }
            }

            return matched;
        }
    }
}
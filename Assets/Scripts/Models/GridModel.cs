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
    
    [Serializable]
    public class GridModel : IGridReader
    {
        public int Width { get; }
        public int Height { get; }
        private readonly MatchableType[] ValidMatchables;
        public readonly MatchableType[,] Grid;
        
        public GridModel(GridConfig config, Random rng)
        {
            Width = config.Width;
            Height = config.Height;
            ValidMatchables = config.ValidMatchables.ToArray();
            Grid = CreateMatchables(rng);
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
        
        private MatchableType CreateRandomMatchable(Random rng)
        {
            return ValidMatchables[rng.Next(ValidMatchables.Length)];
        }
        
        public bool IsCoordinateValidOnGrid(Coordinate coordinate) => coordinate.X < 0 || coordinate.X >= Width ||
                                                                coordinate.Y < 0 || coordinate.Y >= Height;
        
        public bool TryGetMatchableAtPosition(Coordinate coordinate, out MatchableType matchable)
        {
            matchable = MatchableType.None;

            if (IsCoordinateValidOnGrid(coordinate))
                return false;

            matchable = Grid[coordinate.X, coordinate.Y];

            return matchable != MatchableType.None;
        }
    }
}
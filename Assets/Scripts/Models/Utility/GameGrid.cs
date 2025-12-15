using System;
using TapMatch.Models.Configs;

namespace TapMatch.Models.Utility
{
    /// <summary>
    /// Takes in a 2DArray, uses the Columns as the width and Rows as Height and reverses Y coordinate.
    /// Alternatively creates a new Array based on width and height and follows these same rules.
    /// This is to help with manual creation of readable grids.
    /// </summary>
    public class GameGrid
    {
        public readonly MatchableType[,] Array;
        public int Width { get; }
        public int Height { get; }

        public GameGrid(MatchableType[,] array)
        {
            Array = array;
            Width = array.GetLength(1);
            Height = array.GetLength(0);
        }
        
        public GameGrid(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and Height must be positive.");

            Width = width;
            Height = height;
            Array = new MatchableType[width, height];
        }

        private int ToArrayY(int gameY) => Height - 1 - gameY;

        public MatchableType this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    throw new IndexOutOfRangeException("Coordinates are outside the grid bounds.");

                var arrayY = ToArrayY(y);
                
                return Array[arrayY, x];
            }
            set
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    throw new IndexOutOfRangeException("Coordinates are outside the grid bounds.");
            
                var arrayY = ToArrayY(y);

                Array[arrayY, x] = value;
            }
        }
    }
}
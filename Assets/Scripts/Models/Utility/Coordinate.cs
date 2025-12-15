using System;
using System.Collections.Generic;

namespace TapMatch.Models.Utility
{
    // Copied over from personal roguelite project, helper struct for grid games.
    [Serializable]
    public struct Coordinate : IEquatable<Coordinate>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly int GetDistanceTo(Coordinate other)
        {
            var dx = Math.Abs(X - other.X);
            var dy = Math.Abs(Y - other.Y);

            if (dx > dy)
                return 14 * dy + 10 * (dx - dy);

            return 14 * dx + 10 * (dy - dx);
        }

        public readonly int GetManhattanDistanceTo(Coordinate other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public static int GetManhattanDistanceTo(int x, int y, int otherX, int otherY)
        {
            return Math.Abs(x - otherX) + Math.Abs(y - otherY);
        }

        public readonly Coordinate[] GetOrthogonalNeighbors()
        {
            return new Coordinate[]
            {
                new(X + 1, Y), new(X - 1, 0),
                new(0, Y + 1), new(0, Y - 1)
            };
        }

        public readonly IList<Coordinate> GetDiagonalNeighbors()
        {
            return new Coordinate[]
            {
                new(X + 1, Y), new(X - 1, 0),
                new(X, Y + 1), new(X, Y - 1),
                new(X - 1, Y - 1), new(X + 1, Y - 1),
                new(X - 1, Y + 1), new(X + 1, Y + 1),
            };
        }

        public bool Equals(Coordinate other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Coordinate c && Equals(c);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static bool operator == (Coordinate c1, Coordinate c2) => c1.Equals(c2);
        public static bool operator != (Coordinate c1, Coordinate c2) => !c1.Equals(c2);
        public static Coordinate operator +(Coordinate a, Coordinate b) => new (a.X + b.X, a.Y + b.Y);
        public static Coordinate operator -(Coordinate a, Coordinate b) => new (a.X - b.X, a.Y - b.Y);
        public override string ToString() => $"{X}:{Y}";

        public static string MakePairId(Coordinate a, Coordinate b) => a.GetHashCode() <= b.GetHashCode()
            ? $"{a.X},{a.Y}-{b.X},{b.Y}"
            : $"{b.X},{b.Y}-{a.X},{a.Y}";
    }
}
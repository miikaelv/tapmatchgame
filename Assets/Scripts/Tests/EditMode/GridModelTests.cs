using System;
using System.Collections.Generic;
using NUnit.Framework;
using TapMatch.Models;
using TapMatch.Models.Configs;
using TapMatch.Models.Utility;

namespace TapMatch.Tests.EditMode
{
    public class GridModelTests : ModelTestBase<GridModel>
    {
        private const int Seed = 13;

        private readonly GridConfig GridConfig = new(10, 10, new List<MatchableType>
        {
            MatchableType.Red,
            MatchableType.Green,
            MatchableType.Blue,
            MatchableType.Yellow,
        });


        protected override GridModel CreateModelOnSetup() => new(GridConfig, new Random(Seed));

        [Test]
        public void Grid_has_correct_dimensions()
        {
            Assert.AreEqual(GridConfig.Width, Model.Width);
            Assert.AreEqual(GridConfig.Height, Model.Height);
            Assert.AreEqual(GridConfig.Width, Model.Grid.GetLength(0));
            Assert.AreEqual(GridConfig.Height, Model.Grid.GetLength(1));
        }

        [Test]
        public void Grid_contains_only_valid_matchables()
        {
            for (var x = 0; x < Model.Width; x++)
            {
                for (var y = 0; y < Model.Height; y++)
                {
                    var value = Model.Grid[x, y];
                    CollectionAssert.Contains(GridConfig.ValidMatchables, value);
                }
            }
        }

        [Test]
        public void Can_get_matchable_with_coordinate()
        {
            var coordinate = new Coordinate(0, 0);

            var success = Model.TryGetMatchableAtPosition(coordinate, out var matchable);

            Assert.IsTrue(success);
            Assert.AreEqual(Model.Grid[coordinate.X, coordinate.Y], matchable);
        }

        [Test]
        public void Invalid_coordinate_returns_false_matchable()
        {
            var coordinate = new Coordinate(100, 100);

            var success = Model.TryGetMatchableAtPosition(coordinate, out var matchable);

            Assert.IsFalse(success);
            Assert.AreEqual(MatchableType.None, matchable);
        }

        private readonly List<Coordinate> ExpectedCoordinates = new()
        {
            new Coordinate(0, 1),
            new Coordinate(0, 2),
            new Coordinate(1, 2),
            new Coordinate(2, 2),
            new Coordinate(2, 1),
            new Coordinate(2, 0),
            new Coordinate(3, 0),
        };

        private readonly Coordinate MatchStartCoordinate = new(0, 1);

        private readonly MatchableType[,] ManualGrid =
        {
            { MatchableType.Green, MatchableType.Red, MatchableType.Red, MatchableType.Yellow },
            { MatchableType.Blue, MatchableType.Green, MatchableType.Red, MatchableType.Yellow },
            { MatchableType.Red, MatchableType.Red, MatchableType.Red, MatchableType.Yellow },
            { MatchableType.Red, MatchableType.Green, MatchableType.Blue, MatchableType.Yellow },
        };

        [Test]
        public void Can_get_all_connecting_matchables()
        {
            var manualModel = new GridModel(ManualGrid, GridConfig.ValidMatchables.ToArray());
            var matched = manualModel.GetConnectingMatchables(MatchStartCoordinate);

            CollectionAssert.AreEquivalent(ExpectedCoordinates, matched,
                $"Expected: {string.Join(", ", ExpectedCoordinates)}; Got: {string.Join(", ", matched)}");
        }
    }
}
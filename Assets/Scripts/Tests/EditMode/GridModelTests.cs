using System.Collections.Generic;
using NUnit.Framework;
using TapMatch.Models;
using TapMatch.Models.Configs;
using TapMatch.Models.Utility;
using Random = System.Random;

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
            new Coordinate(0, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1),
            new Coordinate(2, 1),
            new Coordinate(2, 2),
            new Coordinate(2, 3),
            new Coordinate(1, 3),
        };

        private readonly Coordinate MatchStartCoordinate = new(0, 0);

        private readonly MatchableType[,] ManualGrid =
        {
            { MatchableType.Green, MatchableType.Red, MatchableType.Red, MatchableType.Yellow },
            { MatchableType.Blue, MatchableType.Green, MatchableType.Red, MatchableType.Yellow },
            { MatchableType.Red, MatchableType.Red, MatchableType.Red, MatchableType.Yellow },
            { MatchableType.Red, MatchableType.Green, MatchableType.Blue, MatchableType.Red },
        };

        private readonly MatchableType[,] ExpectedGridAfterGravity =
        {
            { MatchableType.None, MatchableType.None, MatchableType.None, MatchableType.Yellow },
            { MatchableType.None, MatchableType.None, MatchableType.None, MatchableType.Yellow },
            { MatchableType.Green, MatchableType.Green, MatchableType.None, MatchableType.Yellow },
            { MatchableType.Blue, MatchableType.Green, MatchableType.Blue, MatchableType.Red },
        };

        [Test]
        public void Can_get_all_connecting_matchables()
        {
            var manualModel = new GridModel(ManualGrid, GridConfig.ValidMatchables.ToArray());
            var matched = manualModel.GetConnectingMatchables(MatchStartCoordinate);

            CollectionAssert.AreEquivalent(ExpectedCoordinates, matched);
        }

        [Test]
        public void Gravity_works_correctly_after_clearing_matchables()
        {
            var manualModel = new GridModel(ManualGrid, GridConfig.ValidMatchables.ToArray());
            var matched = manualModel.GetConnectingMatchables(MatchStartCoordinate);

            //CollectionAssert.AreEquivalent(ExpectedCoordinates, matched);

            manualModel.ClearMatchables(matched);
            manualModel.ApplyGravity();

            var expectedModel = new GridModel(ExpectedGridAfterGravity, GridConfig.ValidMatchables.ToArray());

            CollectionAssert.AreEquivalent(expectedModel.Grid.Array, manualModel.Grid.Array);

            for (var x = 0; x < expectedModel.Width; x++)
            {
                for (var y = 0; y < expectedModel.Height; y++)
                {
                    Assert.AreEqual(expectedModel.Grid[x, y], manualModel.Grid[x, y],
                        $"Grid mismatch at {x},{y}, {manualModel.GridToString()}");
                }
            }
        }

        private readonly MatchableType[,] GenerationTestGrid =
        {
            { MatchableType.None, MatchableType.None, },
            { MatchableType.None, MatchableType.Green, },
            { MatchableType.Red, MatchableType.Blue, }
        };

        private readonly MatchableType[,] ExpectedGenerationTestGrid =
        {
            { MatchableType.Blue, MatchableType.Red, },
            { MatchableType.Green, MatchableType.Green, },
            { MatchableType.Red, MatchableType.Blue, }
        };

        [Test]
        public void Grid_can_be_filled_from_None_Tiles()
        {
            var manualModel = new GridModel(GenerationTestGrid, GridConfig.ValidMatchables.ToArray());
            var sequence = new List<int> { 2, 1, 0 };
            var mockRandom = new MockRandom(sequence);
            var newTilesColumns = manualModel.RefillEmptySpaces(mockRandom);

            var expectedModel = new GridModel(ExpectedGenerationTestGrid, GridConfig.ValidMatchables.ToArray());

            CollectionAssert.AreEquivalent(expectedModel.Grid.Array, manualModel.Grid.Array);
        }
    }
}
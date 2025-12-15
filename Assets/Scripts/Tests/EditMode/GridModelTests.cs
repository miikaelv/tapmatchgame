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

        protected override GridModel CreateModelOnSetup() => new (GridConfig, new Random(Seed));
        
        [Test]
        public void Grid_has_correct_dimensions()
        {
            Assert.AreEqual(GridConfig.Width, Model.Width);
            Assert.AreEqual(GridConfig.Height, Model.Height);
            Assert.AreEqual(GridConfig.Width, Model.Grid.GetLength(0));
            Assert.AreEqual(GridConfig.Height, Model.Grid.GetLength(1));
        }
        
        [Test]
        public void Grid_ContainsOnlyValidMatchables()
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
        public void TryGetMatchableAtPosition_ReturnsTrue_ForValidCoordinate()
        {
            var coordinate = new Coordinate(0, 0);

            var success = Model.TryGetMatchableAtPosition(coordinate, out var matchable);

            Assert.IsTrue(success);
            Assert.AreEqual(Model.Grid[coordinate.X, coordinate.Y], matchable);
        }
        
        [Test]
        public void TryGetMatchableAtPosition_ReturnsFalse_ForInvalidCoordinate()
        {
            var coordinate = new Coordinate(100, 100);

            var success = Model.TryGetMatchableAtPosition(coordinate, out var matchable);

            Assert.IsFalse(success);
            Assert.AreEqual(MatchableType.None, matchable);
        }
    }
}
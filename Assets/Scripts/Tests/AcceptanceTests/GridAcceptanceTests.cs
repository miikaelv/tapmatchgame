using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace TapMatch.Tests.AcceptanceTests
{
    public class GridAcceptanceTests : AcceptanceTestBase
    {
        public static readonly int[] IterationCounts = { 5, 10 };

        [UnityTest]
        public IEnumerator
            Can_play_the_game_for_many_iterations([ValueSource(nameof(IterationCounts))] int iterations) =>
            UniTask.ToCoroutine(async () =>
            {
                var rng = new Random();

                for (var i = 0; i < iterations; i++)
                {
                    var randomCoordinate =
                        GameInstance.ModelService.GameStateReader.GridReader.GetRandomCoordinateWithinGrid(rng);

                    await GameInstance.GridWindowController.PressMatchableAtCoordinate(randomCoordinate);
                }
            });
    }
}
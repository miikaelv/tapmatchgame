using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using TapMatch.Models.Utility;
using TapMatch.Views;
using UnityEngine.TestTools;
using VContainer;

namespace TapMatch.Tests.PlayMode
{
    public class GridControllerTests : ViewControllerTestBase<GridWindowController>
    {
        protected override void CreateContext(IContainerBuilder builder)
        {
            builder.Register<GridWindowController>(Lifetime.Transient);
        }

        protected override async UniTask OnUnityTearDown(CancellationToken ct)
        {
            await base.OnUnityTearDown(ct);
            AssetService.UnloadAll();
        }

        [UnityTest]
        public IEnumerator GridController_can_be_shown() => UniTask.ToCoroutine(async () =>
        {
            var showResult = await ViewController.Show(CT);

            Assert.IsTrue(showResult);
            Assert.IsNotNull(ViewController.View);
        });

        [UnityTest]
        public IEnumerator GridController_grid_is_instantiated_correctly() => UniTask.ToCoroutine(async () =>
        {
            var showResult = await ViewController.Show(CT);
            var gridModel = ViewController.GridModel;

            Assert.IsTrue(showResult);
            Assert.IsNotNull(ViewController.View);

            for (var x = 0; x < gridModel.Width; x++)
            {
                for (var y = 0; y < gridModel.Height; y++)
                {
                    var matchableModel = gridModel.Grid[x, y];
                    var foundGridPos = ViewController.GridPositions.TryGetValue(new Coordinate(x, y), out var gridPos);

                    Assert.IsTrue(foundGridPos, $"Expected GridPosition not found at {x}:{y}");

                    var foundMatchable = ViewController.Matchables.TryGetValue(matchableModel.Id, out var matchable);

                    Assert.IsTrue(foundMatchable, $"Expected Matchable not found at {x}:{y}");
                    Assert.AreEqual(matchableModel.Type, matchable.Type);
                }
            }
        });

        [UnityTest]
        public IEnumerator Can_press_matchable() => UniTask.ToCoroutine(async () =>
        {
            var showResult = await ViewController.Show(CT);

            Assert.IsTrue(showResult);
            Assert.IsNotNull(ViewController.View);

            var gridModel = ViewController.GridModel;

            var coord = new Coordinate(0, 0);
            var getMatchableResult = gridModel.TryGetMatchableAtPosition(coord, out var matchable);
            Assert.IsTrue(getMatchableResult);
            
            var pressed = ViewController.PressMatchableAtCoordinate(coord);
            Assert.IsTrue(pressed);
        });
    }
}
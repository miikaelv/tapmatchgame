using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.Models.Utility;
using UnityEngine;

namespace TapMatch.Views.Utility
{
    public class MatchableViewPool : ObjectPool<MatchableView>
    {
        private readonly float CellSize;
        private readonly Func<Coordinate, CancellationToken, UniTask> OnPressEvent;

        public MatchableViewPool(MatchableView prefab, Transform parent, float cellSize,
            Func<Coordinate, CancellationToken, UniTask> onPressEvent) : base(
            prefab, parent)
        {
            CellSize = cellSize;
            OnPressEvent = onPressEvent;
        }

        protected override void OnInstantiate(MatchableView instance)
        {
            instance.SubscribeToOnPress(OnPressEvent);
        }

        protected override void ResetObject(MatchableView obj)
        {
            obj.RectTransform.sizeDelta = new Vector2(CellSize, CellSize);
        }
    }
}
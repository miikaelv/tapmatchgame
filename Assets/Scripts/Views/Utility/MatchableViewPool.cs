using UnityEngine;

namespace TapMatch.Views.Utility
{
    public class MatchableViewPool : ObjectPool<MatchableView>
    {
        private readonly float CellSize;
        
        public MatchableViewPool(MatchableView prefab, Transform parent, float cellSize, int initialSize = 10) : base(
            prefab, parent,
            initialSize)
        {
            CellSize = cellSize;
        }

        protected override void ResetObject(MatchableView obj)
        {
            obj.RectTransform.sizeDelta = new Vector2(CellSize, CellSize);
        }
    }
}
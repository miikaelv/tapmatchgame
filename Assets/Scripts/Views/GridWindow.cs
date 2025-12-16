using UnityEngine;
using UnityEngine.UI;

namespace TapMatch.Views
{
    public class GridWindow : View
    {
        public MatchableView MatchablePrefab;
        public Transform MatchablesParent;
        public RectTransform GridBaseRect;
        public GridLayoutGroup GridBase;
        public Transform TileBase;
    }
}
using System.Collections.Generic;
using TapMatch.Models.Utility;
using UnityEngine;

namespace TapMatch.Views.Grid
{
    public class GridWindow : View
    {
        public MatchableView MatchablePrefab;
        public Transform MatchablePoolParent;
        public RectTransform GridBaseRect;

        public Dictionary<Coordinate, Vector3> CreateGridPositions(int width, int height)
        {
            var dict = new Dictionary<Coordinate, Vector3>();

            var parentWidth = GridBaseRect.rect.width;
            var cellSize = parentWidth / width;
            var gridWidth = cellSize * width;
            var gridHeight = cellSize * height;

            // Assuming centered pivot, calculate bottom left corner position
            var gridOrigin = new Vector2(-gridWidth * 0.5f, -gridHeight * 0.5f);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var localPosition = new Vector3(gridOrigin.x + (x + 0.5f) * cellSize,
                        gridOrigin.y + (y + 0.5f) * cellSize, 0f);

                    dict.Add(new Coordinate(x, y), localPosition);
                }
            }

            return dict;
        }
    }
}
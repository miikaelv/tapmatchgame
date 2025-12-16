using TapMatch.Models.Configs;
using TapMatch.Models.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TapMatch.Views
{
    public class MatchableView : View, IPointerDownHandler
    {
        public Coordinate Coordinate;
        public MatchableType Type;
        
        public int CellSizeOffset;
        public Image ColorImage;
        public RectTransform RectTransform;

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"Clicked {Type.ToString()} matchable at {Coordinate.ToString()}");
        }
    }
}
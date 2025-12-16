using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        private Func<Coordinate, CancellationToken, UniTask> OnPressEvent;

        public void SubscribeToOnPress(Func<Coordinate, CancellationToken, UniTask> listener)
        {
            OnPressEvent += listener;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnPressEvent.Invoke(Coordinate, GameInstance.GlobalCT).Forget();
            Debug.Log($"Clicked {Type.ToString()} matchable at {Coordinate.ToString()}");
        }
    }
}
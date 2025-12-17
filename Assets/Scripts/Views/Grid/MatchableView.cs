using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.Models.Configs;
using TapMatch.Models.Utility;
using TapMatch.Views.GameInstance;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace TapMatch.Views.Grid
{
    public class MatchableView : View, IPointerDownHandler
    {
        private IGlobalCT GlobalCT;

        public Coordinate Coordinate;
        public MatchableType Type;

        public int CellSizeOffset;
        public Image ColorImage;
        public RectTransform RectTransform;

        private Func<Coordinate, CancellationToken, UniTask> OnPressEvent;

        [Inject]
        private void Construct(IGlobalCT globalCt)
        {
            GlobalCT = globalCt;
        }

        public void SetMatchableType(MatchableType type, Dictionary<MatchableType, Color> colorDictionary)
        {
            if (!colorDictionary.TryGetValue(type, out var color))
            {
                Debug.LogError($"{type.ToString()} has no Color in ColorData.");
                return;
            }
            
            Type = type;
            ColorImage.color = color;
        }
        
        public void MoveToTransformPosition(Coordinate coordinate, Vector3 position, RectTransform gridParent)
        {
            SetParent(gridParent);
            transform.localPosition = position;
            Coordinate = coordinate;
        }

        public void SubscribeToOnPress(Func<Coordinate, CancellationToken, UniTask> listener)
        {
            OnPressEvent += listener;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPressEvent.Invoke(Coordinate, GlobalCT.GlobalCT).Forget();
            Debug.Log($"Clicked {Type.ToString()} matchable at {Coordinate.ToString()}");
        }

        public async UniTask Press()
        {
            if (OnPressEvent != null) await OnPressEvent.Invoke(Coordinate, GlobalCT.GlobalCT);
        }
    }
}
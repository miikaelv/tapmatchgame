using UnityEngine;

namespace TapMatch.Views
{
    public interface IUIRoot
    {
        public RectTransform WindowParent { get; }
    }
    
    public class UIRoot : View, IUIRoot
    {
        public RectTransform WindowParentField;
        public RectTransform WindowParent => WindowParentField;
    }
}
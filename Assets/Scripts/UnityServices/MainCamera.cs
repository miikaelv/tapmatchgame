using UnityEngine;

namespace TapMatch.UnityServices
{
    public interface IMainCamera
    {
        public Vector3 ScreenToWorldPoint(Vector3 touchPosition);
        public float OrthographicSize { get; }
        public float Aspect { get; }
    }
    
    public class MainCamera : MonoBehaviour, IMainCamera
    {
        public Camera Camera;

        public Vector3 ScreenToWorldPoint(Vector3 touchPosition) =>  Camera.ScreenToWorldPoint(touchPosition);
        public float OrthographicSize => Camera.orthographicSize;
        public float Aspect => Camera.aspect;
    }
}
using UnityEngine;

namespace Views
{
    public class View : MonoBehaviour
    {
        public void SetActive(bool active) => gameObject.SetActive(active);
        public void SetParent(Transform parent) => transform.SetParent(parent);
        public void SetGlobalPosition(Vector3 position) => transform.position = position;
        public void SetLocalPosition(Vector3 position) => transform.localPosition = position;
    }
}
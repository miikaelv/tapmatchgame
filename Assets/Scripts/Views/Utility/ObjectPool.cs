using System.Collections.Generic;
using UnityEngine;

namespace TapMatch.Views.Utility
{
    public abstract class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T Prefab;
        private readonly Transform Parent;
    
        private readonly Queue<T> Pool = new ();

        protected ObjectPool(T prefab, Transform parent, int initialSize = 10)
        {
            Prefab = prefab;
            Parent = parent;
            
            for (var i = 0; i < initialSize; i++)
            {
                var obj = Object.Instantiate(prefab, Parent);
                obj.gameObject.SetActive(false);
                Pool.Enqueue(obj);
            }
        }

        public T GetFromPool()
        {
            var obj = Pool.Count > 0 ? Pool.Dequeue() : Object.Instantiate(Prefab, Parent);

            ResetObject(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            Pool.Enqueue(obj);
        }

        protected abstract void ResetObject(T obj);
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TapMatch.Views.Utility
{
    public abstract class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T Prefab;
        private readonly Transform Parent;

        private readonly Queue<T> Pool = new();

        protected ObjectPool(T prefab, Transform parent)
        {
            Prefab = prefab;
            Parent = parent;
        }

        protected abstract void OnInstantiate(T instance);

        public void PreBake(int initialSize)
        {
            for (var i = 0; i < initialSize; i++)
            {
                var obj = Object.Instantiate(Prefab, Parent);
                obj.gameObject.SetActive(false);
                OnInstantiate(obj);
                Pool.Enqueue(obj);
            }
        }
        
        public T GetFromPool()
        {
            T obj;
            if (!Pool.Any())
            {
                obj = Object.Instantiate(Prefab, Parent);
                OnInstantiate(obj);
            }
            else obj = Pool.Dequeue();

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
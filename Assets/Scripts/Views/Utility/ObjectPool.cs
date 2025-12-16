using System.Collections.Generic;
using System.Linq;
using TapMatch.UnityServices;
using UnityEngine;

namespace TapMatch.Views.Utility
{
    public abstract class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T Prefab;
        private readonly Transform Parent;
        private readonly IAssetService AssetService;

        private readonly Queue<T> Pool = new();

        protected ObjectPool(T prefab, Transform parent, IAssetService assetService)
        {
            Prefab = prefab;
            Parent = parent;
            AssetService = assetService;
        }

        protected abstract void OnInstantiate(T instance);

        public void PreBake(int initialSize)
        {
            for (var i = 0; i < initialSize; i++)
            {
                var obj = AssetService.InstantiateWithInject(Prefab, Parent);
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
                obj = AssetService.InstantiateWithInject(Prefab, Parent);
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
            obj.transform.SetParent(Parent);
            Pool.Enqueue(obj);
        }

        protected abstract void ResetObject(T obj);
    }
}
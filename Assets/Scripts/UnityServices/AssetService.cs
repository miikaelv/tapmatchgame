using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TapMatch.UnityServices
{
    public interface IAssetService
    {
        public UniTask<T> LoadAsset<T>(string id, CancellationToken ct) where T : Component;
        public UniTask<T> LoadSingletonAsset<T>(CancellationToken ct) where T : MonoBehaviour;
    }

    public class AssetService : IAssetService
    {
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> AssetHandles = new();

        public async UniTask<T> LoadSingletonAsset<T>(CancellationToken ct) where T : MonoBehaviour =>
            await LoadAsset<T>(typeof(T).Name, ct);

        public async UniTask<bool> Initialize(CancellationToken ct)
        {
            var handle = Addressables.InitializeAsync(false);

            try
            {
                await handle.ToUniTask(cancellationToken: ct);

                Debug.Log($"Addressables initialized with status {handle.Status}");

                return handle.Status == AsyncOperationStatus.Succeeded;
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Addressables initialization cancelled");
                return false;
            }
        }
        
        public async UniTask<T> LoadAsset<T>(string id, CancellationToken ct)
            where T : Component
        {
            Debug.Log($"LoadAsset {id}");

            if (AssetHandles.TryGetValue(id, out var cached))
            {
                await cached.ToUniTask(cancellationToken: ct);
                return GetComponentOrThrow<T>(id, cached);
            }

            Debug.Log("Asset handle not found, loading from Addressables");

            var handle = Addressables.LoadAssetAsync<GameObject>(id);
            AssetHandles[id] = handle;

            await handle.ToUniTask(cancellationToken: ct);

            return handle.Status != AsyncOperationStatus.Succeeded
                ? throw new Exception($"Failed to load asset with id \"{id}\"")
                : GetComponentOrThrow<T>(id, handle);
        }

        private T GetComponentOrThrow<T>(string id, AsyncOperationHandle<GameObject> handle)
            where T : Component
        {
            if (handle.Result == null)
                throw new Exception($"Loaded asset with id \"{id}\" is null");

            return !handle.Result.TryGetComponent<T>(out var component)
                ? throw new ArgumentException($"Asset \"{id}\" does not contain component of type {typeof(T).Name}")
                : component;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;

namespace TapMatch.UnityServices
{
    public interface IAssetService
    {
        public bool IsAssetLoaded(string id);
        public bool ReleaseAsset(string id);
        public void UnloadUnusedAssets();
        public UniTask<T> LoadAsset<T>(string id, CancellationToken ct) where T : class;
        public UniTask<T> LoadSingletonView<T>(CancellationToken ct) where T : Component;
        public UniTask<T> LoadViewComponent<T>(string id, CancellationToken ct) where T : Component;
        public UniTask<T> LoadScriptableObject<T>(CancellationToken ct) where T : ScriptableObject;
        public T InstantiateWithInject<T>(T prefab, Transform parent = null) where T : MonoBehaviour;
    }

    public class AssetService : IAssetService, IDisposable
    {
        private readonly IObjectResolver Resolver;
        private readonly Dictionary<string, AsyncOperationHandle> AssetHandles = new();

        public AssetService(IObjectResolver resolver)
        {
            Resolver = resolver;
        }

        public async UniTask<bool> Initialize(CancellationToken ct)
        {
            AssetHandles.Clear();

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

        public bool IsAssetLoaded(string id)
        {
            if (!AssetHandles.TryGetValue(id, out var handle))
                return false;

            return handle.Status == AsyncOperationStatus.Succeeded;
        }

        public bool ReleaseAsset(string id)
        {
            if (!AssetHandles.TryGetValue(id, out var handle))
                return false;

            Addressables.Release(handle);
            AssetHandles.Remove(id);
            return true;
        }

        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }

        public async UniTask<T> LoadSingletonView<T>(CancellationToken ct) where T : Component
            => await LoadViewComponent<T>(typeof(T).Name, ct);

        public async UniTask<T> LoadViewComponent<T>(string id, CancellationToken ct) where T : Component
        {
            if (AssetHandles.TryGetValue(id, out var cached))
            {
                Debug.Log($"Asset {id} already loaded, getting from cache.");

                await cached.ToUniTask(cancellationToken: ct);

                var go = cached.Result as GameObject ??
                         throw new Exception($"Loaded asset {id} not the required {nameof(GameObject)}");

                return go.TryGetComponent<T>(out var cachedComponent)
                    ? cachedComponent
                    : throw new ArgumentException($"ViewAsset {typeof(T).Name} has no component {typeof(T).Name}");
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(id);
            AssetHandles[id] = handle;

            await handle.ToUniTask(cancellationToken: ct);

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new Exception($"Failed to load asset with id \"{id}\"");

            return handle.Result.TryGetComponent<T>(out var component)
                ? component
                : throw new ArgumentException($"ViewAsset {typeof(T).Name} has no component {typeof(T).Name}");
        }

        public async UniTask<T> LoadScriptableObject<T>(CancellationToken ct) where T : ScriptableObject
            => await LoadAsset<T>(typeof(T).Name, ct);

        public T InstantiateWithInject<T>(T prefab, Transform parent = null) where T : MonoBehaviour =>
            Resolver.Instantiate(prefab, parent);

        public async UniTask<T> LoadAsset<T>(string id, CancellationToken ct)
            where T : class
        {
            Debug.Log($"LoadAsset {id}");

            if (AssetHandles.TryGetValue(id, out var cached))
            {
                Debug.Log($"Asset {id} already loaded, getting from cache.");

                await cached.ToUniTask(cancellationToken: ct);

                return cached as T ??
                       throw new ArgumentException($"Loaded asset {id} not of requested type {typeof(T).Name}");
            }

            Debug.Log("Asset handle not found, loading from Addressables");

            var handle = Addressables.LoadAssetAsync<T>(id);
            AssetHandles[id] = handle;

            await handle.ToUniTask(cancellationToken: ct);

            return handle.Status != AsyncOperationStatus.Succeeded
                ? throw new Exception($"Failed to load asset with id \"{id}\"")
                : handle.Result;
        }

        public void UnloadAll()
        {
            UnloadUnusedAssets();
            foreach (var handle in AssetHandles.Values)
                Addressables.Release(handle);

            AssetHandles.Clear();
        }

        public void Dispose()
        {
            UnloadAll();
        }
    }
}
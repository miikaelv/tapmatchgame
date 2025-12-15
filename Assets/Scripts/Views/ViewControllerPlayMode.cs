using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TapMatch.UnityServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Views
{
    public interface IViewController
    {
        public string Id { get; }
        public bool IsLoaded { get; }
        public bool IsInstantiated { get; }
        public bool IsShown { get; }
        public bool IsViewObjectActive { get; }
        public UniTask<bool> Load(CancellationToken ct);
        public void Dispose();
        public UniTask<bool> Instantiate(CancellationToken ct, Transform parent);
        public UniTask<bool> Show(CancellationToken ct);
        public UniTask<bool> Hide(CancellationToken ct);
    }

    public abstract class ViewControllerPlayMode<T> : IViewController, IDisposable where T : View
    {
        protected T View;
        private T LoadedAsset;
        public string Id => typeof(T).Name;
        public bool IsInstantiated => View != null;
        public bool IsLoaded => LoadedAsset != null;
        public bool IsShown { get; private set; }
        public bool IsViewObjectActive => View.gameObject.activeSelf;

        private readonly IAssetService AssetService;

        protected ViewControllerPlayMode(IAssetService assetService)
        {
            AssetService = assetService;
        }

        public async UniTask<bool> Load(CancellationToken ct)
        {
            LoadedAsset = await AssetService.LoadSingletonAsset<T>(ct);

            return LoadedAsset != null;
        }

        public void Dispose()
        {
            DestroyViewObject();
            AssetService.ReleaseAsset(Id);
            LoadedAsset = null;
            IsShown = false;
        }

        private void DestroyViewObject()
        {
            if (IsInstantiated) Object.Destroy(View.gameObject);
            View = null;
            IsShown = false;
        }

        // null parent instantiates at Scene root
        public async UniTask<bool> Instantiate(CancellationToken ct, Transform parent = null)
        {
            try
            {
                if (View != null)
                {
                    Debug.LogWarning($"Asset already Instantiated");

                    return false;
                }

                if (LoadedAsset == null) await Load(ct);

                View = Object.Instantiate(LoadedAsset, parent);
                View.SetActive(false);

                var result = await OnInstantiate(ct);

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public async UniTask<bool> Show(CancellationToken ct)
        {
            try
            {
                if (IsShown) return true;
                if (!IsInstantiated) await Instantiate(ct);

                await OnPreShow(ct);
                View.SetActive(true);
                await OnPostShow(ct);
                IsShown = true;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public async UniTask<bool> Hide(CancellationToken ct)
        {
            try
            {
                if (!IsShown) return true;
                
                await OnHide(ct);
                View.SetActive(false);
                IsShown = false;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        protected virtual UniTask<bool> OnInstantiate(CancellationToken ct) => UniTask.FromResult(true);
        protected virtual UniTask<bool> OnPreShow(CancellationToken ct) => UniTask.FromResult(true);
        protected virtual UniTask<bool> OnPostShow(CancellationToken ct) => UniTask.FromResult(true);
        protected virtual UniTask<bool> OnHide(CancellationToken ct) => UniTask.FromResult(true);
    }
}
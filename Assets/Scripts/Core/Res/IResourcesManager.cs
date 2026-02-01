using UnityEngine;
using UnityEngine.Events;

namespace Core.Res
{
    /// <summary>
    /// ��Դ�������ӿ�
    /// </summary>
    public interface IResourcesManager
    {
        void Clear();
        T Load<T>(string resPath) where T : Object;
        void LoadAsync<T>(string resName, UnityAction<T> callBack) where T : Object;
        void UnloadAsset<T>(string resName) where T : Object;
        void UnloadUnusedAssets(UnityAction callBack = null);
    }
}

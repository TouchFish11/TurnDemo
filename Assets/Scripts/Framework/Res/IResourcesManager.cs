using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源管理器接口
/// </summary>
public interface IResourcesManager
{
    void Clear();
    T Load<T>(string resPath) where T : Object;
    void LoadAsync<T>(string resName, UnityAction<T> callBack) where T : Object;
    void UnloadAsset<T>(string resName) where T : Object;
    void UnloadUnusedAssets(UnityAction callBack = null);
}

using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 对象池管理器接口
/// </summary>
public interface IPoolManager
{
    void Clear();
    void ClearType<T>();
    Task<GameObject> GetAssetBundleObjAsync(E_AssetBundleType assetBundleType, string assetName);
    T GetData<T>(string nameSpace = "") where T : class, IPoolData, new();
    T GetObj<T>(string assetName) where T : Behaviour;
    void PushData<T>(T data, string nameSpace = "") where T : class, IPoolData, new();
    void PushObj(GameObject obj);
}

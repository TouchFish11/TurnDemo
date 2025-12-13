using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 对象构建器
/// </summary>
public class ObjectBuilder
{
    /// <summary>
    /// 获取或创建实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetBundleType"></param>
    /// <param name="assetName"></param>
    /// <param name="parent"></param>
    /// <param name="worldPosStay"></param>
    /// <returns></returns>
    public static async Task<T> GetOrCreateInstance<T>(E_AssetBundleType assetBundleType, string assetName, Transform parent, bool worldPosStay = false) where T : Component
    {
        GameObject cacheObj = await PoolManager.Instance.GetAssetBundleObjAsync(assetBundleType, assetName);
        cacheObj.transform.SetParent(parent, worldPosStay);
        T component = cacheObj.GetComponent<T>();
        return component;
    }
}

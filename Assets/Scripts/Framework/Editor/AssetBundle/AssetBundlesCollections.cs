using Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// AB包收集类
/// </summary>
public class AssetBundlesCollections : SingletonSOBase<AssetBundlesCollections>
{
    /// <summary>
    /// AB包信息
    /// </summary>
    [Serializable]
    public sealed class AssetBundleInfo
    {
        //AB包名
        public string assetBundleName;
        //AB包资源列表
        public List<AssetInfo> assetInfoList  = new List<AssetInfo>();

        public AssetBundleInfo(string assetBundleName)
        {
            this.assetBundleName = assetBundleName;
        }
    }

    /// <summary>
    /// 资源信息
    /// </summary>
    [Serializable]
    public sealed class AssetInfo
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        [Tooltip("资源路径")]
        public string assetPath;
        /// <summary>
        /// AB包大小
        /// </summary>
        [Tooltip("资源大小（字节）")]
        public long size;
        /// <summary>
        /// 资源引用
        /// </summary>
        [Tooltip("资源名称")]
        public string name;

        public AssetInfo(string assetPath, long assetBundleSize, string name)
        {
            this.assetPath = assetPath;
            this.size = assetBundleSize;
            this.name = name;
        }
    }

    //AB包信息列表
    public List<AssetBundleInfo> assetBundleInfoList = new List<AssetBundleInfo>();

    /// <summary>
    /// 添加信息
    /// </summary>
    /// <param name="assetBundleName">AB包名</param>
    /// <param name="assetInfo">资源信息对象</param>
    public void Add(string assetBundleName, AssetInfo assetInfo)
    {
        bool isExist = false;
        for (int i = 0; i < assetBundleInfoList.Count; i++)
        {
            isExist = false;
            AssetBundleInfo bundleInfo = assetBundleInfoList[i];

            if (bundleInfo.assetBundleName == assetBundleName)
            {
                bundleInfo.assetInfoList.Add(assetInfo);
                isExist = true;
                break;
            }
        }

        if (!isExist)
        {
            AssetBundleInfo assetBundleInfo = new AssetBundleInfo(assetBundleName);
            assetBundleInfo.assetInfoList.Add(assetInfo);
            assetBundleInfoList.Add(assetBundleInfo);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void Clear()
    {
        assetBundleInfoList.Clear();
    }
}

using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.Singleton;
using UnityEditor;
using UnityEngine;

namespace Editor.AssetBundle
{
    /// <summary>
    /// AssetBundle集合管理类（单例SO）
    /// 作用：统一管理所有AssetBundle的信息，包括AB包名称、包内资源列表，提供添加/清空AB信息的能力
    /// 继承：基于单例SO基类，保证全局唯一实例
    /// </summary>
    public class AssetBundlesCollections : SingletonSOBase<AssetBundlesCollections>
    {
        /// <summary>
        /// AssetBundle信息实体类
        /// 用于存储单个AB包的名称和其包含的所有资源信息列表
        /// </summary>
        [Serializable] // 标记可序列化，支持在Inspector面板显示和序列化存储
        public sealed class AssetBundleInfo
        {
            // AB包的名称，不带后缀（唯一标识）
            public string assetBundleName;
            // 该AB包下包含的所有资源信息列表
            public List<AssetInfo> assetInfos = new();

            /// <summary>
            /// 构造函数：初始化AB包名称
            /// </summary>
            /// <param name="assetBundleName">AB包名称</param>
            public AssetBundleInfo(string assetBundleName)
            {
                this.assetBundleName = assetBundleName;
            }
        }

        /// <summary>
        /// 资源信息实体类
        /// 用于存储单个资源的路径、大小、名称等核心信息
        /// </summary>
        [Serializable]
        public sealed class AssetInfo
        {
            /// <summary>
            /// 资源在项目中的相对路径（如：Assets/Res/UI/Button.prefab）
            /// </summary>
            [Tooltip("资源在项目中的相对路径")] // Inspector面板提示
            public string assetPath;

            /// <summary>
            /// 资源的大小
            /// </summary>
            [Tooltip("资源大小（单位：字节）")]
            public long size;

            /// <summary>
            /// 资源的名称
            /// </summary>
            [Tooltip("资源名称")]
            public string name;
            
            /// <summary>
            /// 资源唯一标识
            /// </summary>
            [Tooltip("Hash字符串")]
            public string hash;
            
            /// <summary>
            /// 资源类型
            /// </summary>
            [Tooltip("资源类型枚举")]
            public EAssetType assetType;

            /// <summary>
            /// 构造函数：初始化资源的核心信息
            /// </summary>
            /// <param name="assetPath">资源路径</param>
            /// <param name="assetBundleSize">资源大小（字节）</param>
            /// <param name="name">资源名称</param>
            /// <param name="hash"></param>
            /// <param name="assetType"></param>
            public AssetInfo(string assetPath, long assetBundleSize, string name, string hash, EAssetType assetType)
            {
                this.assetPath = assetPath;
                this.size = assetBundleSize;
                this.name = name;
                this.hash = hash;
                this.assetType = assetType;
            }
        }

        // 所有AssetBundle信息的总列表，存储项目中所有AB包的信息
        public List<AssetBundleInfo> assetBundleInfos = new();

        /// <summary>
        /// 添加资源信息到指定AB包的信息列表中
        /// 逻辑：先检查AB包是否已存在，存在则追加资源，不存在则新建AB包再添加资源
        /// </summary>
        /// <param name="assetBundleName">目标AB包名称</param>
        /// <param name="assetInfo">要添加的资源信息实例</param>
        public void Add(string assetBundleName, AssetInfo assetInfo)
        {

            var assetBundleInfo = assetBundleInfos.Find(abInfo => abInfo.assetBundleName == assetBundleName);
            // AB包存在。追加当前包资源信息
            if (assetBundleInfo != null)
            {
                assetBundleInfo.assetInfos.Add(assetInfo);
            }
            // 若AB包不存在，则新建AB包信息并添加资源
            else
            {
                // 实例化新的AB包信息
                assetBundleInfo = new AssetBundleInfo(assetBundleName);
                // 添加资源信息到新AB包的列表
                assetBundleInfo.assetInfos.Add(assetInfo);
                // 将新AB包信息加入总列表
                assetBundleInfos.Add(assetBundleInfo); 
            }

            Save();
        }

        /// <summary>
        /// 移除旧AB包的旧资源信息
        /// </summary>
        /// <param name="OldAssetBundleName"></param>
        /// <param name="oldAssetPath"></param>
        public void Remove(string OldAssetBundleName, string oldAssetPath)
        {
            var assetbundleInfo = assetBundleInfos.Find(abInfo => abInfo.assetBundleName == OldAssetBundleName);
            if (assetbundleInfo != null)
            {
                var assetInfo = assetbundleInfo.assetInfos.Find(assetInfo => assetInfo.assetPath == oldAssetPath);
                if (assetInfo != null)
                {
                    assetbundleInfo.assetInfos.Remove(assetInfo);
                }
            }

            Save();
        }

        /// <summary>
        /// 清空所有AssetBundle信息列表
        /// 注意：该方法仅清空内存中的列表，未主动保存修改，需外部按需处理保存逻辑
        /// </summary>
        public void Clear()
        {
            assetBundleInfos.Clear();
        }

        /// <summary>
        /// 保存变化
        /// </summary>
        private void Save()
        {
            // 标记当前SO对象为已修改（Editor下生效，确保序列化数据保存）
            EditorUtility.SetDirty(this);
            // 保存所有AssetDatabase的修改
            AssetDatabase.SaveAssets();
            // 刷新AssetDatabase，使修改立即生效
            AssetDatabase.Refresh();
        }
    }
}
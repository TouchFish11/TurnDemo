using System;
using Core.Collection;
using Core.Utility;
using UnityEngine;

namespace Core.AssetBundles.Collection
{
    /// <summary>
    /// AssetBundle包集合类
    /// 继承自泛型集合，键为AB包标识字符串，值为AB包信息对象
    /// </summary>
    public class ABPackageCollection : Collection<string, ABPackageInfo>
    {
        public override bool TryGetValue(string key, out ABPackageInfo value)
        {
            var abKey = !key.Contains(FileUtility.AbSuffix) ? $"{key}{FileUtility.AbSuffix}" : key;
            return base.TryGetValue(abKey, out value);
        }
        
        public string[] GetAllDependencies(string abName)
        {
            var abKey = !abName.Contains(FileUtility.AbSuffix) ? $"{abName}{FileUtility.AbSuffix}" : abName;
            return base.TryGetValue(abKey, out var abPackageInfo) ? abPackageInfo.Dependencies : Array.Empty<string>();
        }

        public void Add(string abName, ABPackageInfo abPackageInfo)
        {
            keyToValueMap.Add(abName, abPackageInfo);
        }

        /// <summary>
        /// 计算需要下载的AB包总字节数
        /// 对比远程最新AB包集合与本地缓存的AB包信息，得出待下载的总数据量
        /// </summary>
        /// <param name="remoteCollection">远程服务器端的最新AB包集合</param>
        /// <param name="waitDownloadCollection"></param>
        /// <returns>需要下载的总字节数（若无需下载则返回0）</returns>
        public static long GetTotalDownLoadBytes(ABPackageCollection remoteCollection, AbPackageCacheCollection waitDownloadCollection)
        {
            // 初始化待下载总字节数为0
            long totalDownLoadBytes = 0;
            // 遍历远程所有AB包信息
            foreach (var pair in remoteCollection)
            {
                // 获取当前遍历的远程AB包信息
                var packageInfo = remoteCollection[pair.Key];
                // 检查本地是否存在该AB包的缓存记录
                if (waitDownloadCollection.TryGetValue(packageInfo.Name, out var waiDownloadInfo))
                {
                    // 本地有缓存：计算需补充下载的字节数（确保数值非负）
                    // 若本地已下载字节数≥远程包大小，则补充下载量为0
                    totalDownLoadBytes += (long)Mathf.Max(0, packageInfo.Size - waiDownloadInfo.DownloadedBytes);
                }
            }

            // 返回最终计算的待下载总字节数
            return totalDownLoadBytes;
        }
    }
}
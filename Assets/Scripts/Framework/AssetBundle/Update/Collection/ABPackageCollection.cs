using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// AB包集合
    /// </summary>
    //[Serializable]
    public class ABPackageCollection : Collection<string, ABPackageInfo>
    {
        /// <summary>
        /// 获取当前更新的总下载字节数
        /// </summary>
        /// <param name="remoteCollection">待下载AB包字典</param>
        /// <param name="localCacheCollection">本地已下载AB包记录信息字典</param>
        /// <returns>总字节数</returns>
        public static long GetTotalDownLoadBytes(ABPackageCollection remoteCollection, ABPackageCacheCollection localCacheCollection)
        {
            long totalDownLoadBytes = 0;

            foreach (KeyValuePair<string, ABPackageInfo> pair in remoteCollection)
            {
                ABPackageInfo packageInfo = remoteCollection[pair.Key];
                if (localCacheCollection.TryGetValue(packageInfo.Name, out var cacheInfo))
                {
                    totalDownLoadBytes += (long)Mathf.Max(0, packageInfo.Size - cacheInfo.DownloadedBytes);
                }
                else
                {
                    totalDownLoadBytes += packageInfo.Size;
                }
            }

            //返回
            return totalDownLoadBytes;
        }
    }
}

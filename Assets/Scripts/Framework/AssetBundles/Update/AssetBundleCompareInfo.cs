using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// AB包对比信息类
    /// </summary>
    public class AssetBundleCompareInfo
    {
        //AB包名称
        private readonly string _name;
        //AB包大小
        private readonly long _size;
        //AB包MD5码
        private readonly string _md5;

        public AssetBundleCompareInfo(string name, string size, string md5)
        {
            this._name = name;
            this._size = long.Parse(size);
            this._md5 = md5;
        }

        /// <summary>
        /// 获取当前更新的总下载字节数
        /// </summary>
        /// <param name="remoteABCompareInfoDic">待下载AB包字典</param>
        /// <param name="localDownedABInfoRecordDic">本地已下载AB包记录信息字典</param>
        /// <returns>总字节数</returns>
        public static long GetTotalDownLoadBytes(Dictionary<string, AssetBundleCompareInfo> remoteABCompareInfoDic, Dictionary<string, AssetBundleRecordInfo> localDownedABInfoRecordDic)
        {
            long totalDownLoadBytes = 0;

            foreach (var compareInfo in remoteABCompareInfoDic.Values)
            {
                if (localDownedABInfoRecordDic.TryGetValue(compareInfo.Name, out var recordInfo))
                {
                    totalDownLoadBytes += (long)Mathf.Max(0, compareInfo.Size - recordInfo.DownloadedBytes);
                }
                else
                {
                    totalDownLoadBytes += compareInfo.Size;
                }
            }

            //返回
            return totalDownLoadBytes;
        }

        /// <summary>
        /// AB包名称
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// AB包大小
        /// </summary>
        public long Size => _size;
        /// <summary>
        /// AB包MD5码
        /// </summary>
        public string Md5 => _md5;
    }
}

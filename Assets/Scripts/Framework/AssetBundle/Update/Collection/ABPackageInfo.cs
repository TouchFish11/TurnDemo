using System;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// AB包信息
    /// </summary>
    [Serializable]
    public class ABPackageInfo
    {
        [SerializeField]
        private string packageName;
        [SerializeField]
        private long packageSize;
        [SerializeField]
        private string packageMD5;

        public ABPackageInfo(string packageName, long packageSize, string packageMD5)
        {
            this.packageName = packageName;
            this.packageSize = packageSize;
            this.packageMD5 = packageMD5;
        }

        /// <summary>
        /// AB包名称
        /// </summary>
        public string Name => packageName;
        /// <summary>
        /// AB包大小
        /// </summary>
        public long Size => packageSize;
        /// <summary>
        /// AB包MD5码
        /// </summary>
        public string Md5 => packageMD5;
    }
}

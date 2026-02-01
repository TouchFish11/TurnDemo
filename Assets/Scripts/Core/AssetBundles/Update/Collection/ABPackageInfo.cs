using System;
using UnityEngine;

namespace Core.AssetBundles.Update.Collection
{
    /// <summary>
    /// AssetBundle包信息实体类
    /// 用于存储单个AB包的核心信息（名称、大小、MD5值），支持序列化
    /// </summary>
    [Serializable]
    public class ABPackageInfo
    {
        /// <summary>
        /// AB包名称（包含相对路径或唯一标识）
        /// </summary>
        [SerializeField]
        private string packageName;

        /// <summary>
        /// AB包文件大小（单位：字节）
        /// </summary>
        [SerializeField]
        private long packageSize;

        /// <summary>
        /// AB包的MD5校验值
        /// 用于校验AB包文件的完整性和唯一性，防止文件损坏或篡改
        /// </summary>
        [SerializeField]
        private string packageMD5;

        /// <summary>
        /// 构造函数：初始化AB包信息
        /// </summary>
        /// <param name="packageName">AB包名称</param>
        /// <param name="packageSize">AB包大小（字节）</param>
        /// <param name="packageMD5">AB包MD5校验字符串</param>
        public ABPackageInfo(string packageName, long packageSize, string packageMD5)
        {
            this.packageName = packageName;
            this.packageSize = packageSize;
            this.packageMD5 = packageMD5;
        }

        /// <summary>
        /// 只读属性：获取AB包名称
        /// </summary>
        public string PackageName => packageName;

        /// <summary>
        /// 只读属性：获取AB包大小（字节）
        /// </summary>
        public long PackageSize => packageSize;

        /// <summary>
        /// 只读属性：获取AB包的MD5校验值
        /// </summary>
        public string PackageMd5 => packageMD5;
    }
}
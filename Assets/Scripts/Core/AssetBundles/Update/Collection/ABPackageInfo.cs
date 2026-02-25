using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.AssetBundles.Update.Collection
{
    /// <summary>
    /// AssetBundle包信息实体类
    /// 用于存储单个AB包的核心信息（名称、大小、MD5值），支持序列化
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ABPackageInfo
    {
        /// <summary>
        /// AB包名称
        /// </summary>
        [JsonProperty]
        private string name;

        /// <summary>
        /// AB包文件大小（单位：字节）
        /// </summary>
        [JsonProperty]
        private long size;

        /// <summary>
        /// AB包的Hash
        /// 用于校验AB包文件的完整性和唯一性，防止文件损坏或篡改
        /// </summary>
        [JsonProperty]
        private string hash;

        /// <summary>
        /// AB包依赖项
        /// </summary>
        [JsonProperty] private string[] dependencies;

        /// <summary>
        /// 构造函数：初始化AB包信息
        /// </summary>
        /// <param name="name">AB包名称</param>
        /// <param name="size">AB包大小（字节）</param>
        /// <param name="hash">AB包Hash字符串</param>
        /// <param name="dependencies">AB包依赖项</param>
        public ABPackageInfo(string name, long size, string hash, string[] dependencies)
        {
            this.name = name;
            this.size = size;
            this.hash = hash;
            this.dependencies = dependencies;
        }

        /// <summary>
        /// 获取AB包名称
        /// </summary>
        public string Name => name;

        /// <summary>
        /// 获取AB包大小（字节）
        /// </summary>
        public long Size => size;

        /// <summary>
        /// 获取AB包的Hash值
        /// </summary>
        public string Hash => hash;
        
        /// <summary>
        /// 依赖项
        /// </summary>
        public string[] Dependencies => dependencies;
    }
}
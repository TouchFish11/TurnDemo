using System;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 上传模块配置
    /// </summary>
    [Serializable]
    public class UploadModuleConfig
    {
        /// <summary>
        /// 上传地址
        /// </summary>
        [Header("上传地址")]
        [Tooltip("上传到服务器指定文件路径（若存在）")]
        public string uploadServerIp = "http://ip:port/...";
    }
}

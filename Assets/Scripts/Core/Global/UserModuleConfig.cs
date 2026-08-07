using System;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 用户模块配置
    /// </summary>
    [Serializable]
    public class UserModuleConfig
    {
        /// <summary>
        /// 用户数据加载/保存路径类型
        /// </summary>
        [Header("用户数据加载路径类型")]
        [Tooltip("确定从哪个文件夹加载/保存用户数据")]
        public EDataLoadPath userDataPath = EDataLoadPath.Streaming;
    }
}

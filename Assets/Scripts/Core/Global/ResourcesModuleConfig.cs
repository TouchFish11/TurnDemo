using System;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 数据加载路径类型
    /// </summary>
    public enum EDataLoadPath
    {
        /// <summary>
        /// 流文件夹
        /// </summary>
        Streaming,
        /// <summary>
        /// 持久文件夹
        /// </summary>
        Persistent,
    }
    
    /// <summary>
    /// 资源加载模块配置
    /// </summary>
    [Serializable]
    public class ResourcesModuleConfig
    {
        /// <summary>
        /// AB包数据加载路径类型
        /// </summary>
        [Header("AB包数据加载路径类型")]
        [Tooltip("确定从哪个文件夹加载AB包")]
        public EDataLoadPath abLoadPath = EDataLoadPath.Streaming;
        
        /// <summary>
        /// AB包访问活跃阈值，高于该数值则放入热包列表，小于则放入冷包列表
        /// </summary>
        [Header("AB包访问活跃阈值")] 
        [Tooltip("AB包访问活跃阈值，高于该数值则放入热包列表，小于则放入冷包列表")]
        public int criticalActiveThreshold = 8;
        
        /// <summary>
        /// 单个AB包滑动窗口最大数
        /// </summary>
        [Header("单个AB包滑动窗口最大数")] 
        [Tooltip("单个AB包滑动窗口最大数")]
        public int bundleSlidingWindowMaxCount = 10;
        
        /// <summary>
        /// 单个滑动窗口最大时间
        /// </summary>
        [Header("单个滑动窗口最大时间")] 
        [Tooltip("单个滑动窗口最大时间")]
        public float maxDurationPerWindow = 30f;
    }
}

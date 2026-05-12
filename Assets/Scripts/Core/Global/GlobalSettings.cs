using Core.Log;
using Core.Singleton;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class GlobalSettings : SingletonSOBase<GlobalSettings>
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
        /// 热更新文件加载路径
        /// </summary>
        [Header("热更新文件加载路径")]
        [Tooltip("热更新文件本地加载根路径")]
        public string hotUpdateLoadPath;

        /// <summary>
        /// 日志过滤级别
        /// </summary>
        [Header("日志过滤级别")]
        [Tooltip("没有选中的日志类型将不会被记录")]
        public ELogLevel filterLevel = ~ELogLevel.None;

        /// <summary>
        /// 日志写入最大间隔时间（s）
        /// </summary>
        [Header("日志写入最大间隔时间")]
        [Tooltip("到达时间将写入一次日志到本地")]
        public ushort writeLogMaxIntervalTime = 30;

        /// <summary>
        /// 是否启用缓存池布局――开发阶段使用
        /// </summary>
        [Header("启用对象池层级结构")]
        [Tooltip("对象池对象按层级结构布局")]
        public bool isOpenLayout = true;

        /// <summary>
        /// 上传地址
        /// </summary>
        [Header("上传地址")]
        [Tooltip("上传到服务器指定文件路径（若存在）")]
        public string uploadServerIp = "http://ip:port/...";

        /// <summary>
        /// 资源服务器地址
        /// </summary>
        [Header("资源服务器地址")]
        [Tooltip("服务器资源下载路径")]
        public string resServerIp = "http://ip:port/...";

        /// <summary>
        /// 单次更新中对比文件重新下载最大次数
        /// </summary>
        [Header("对比文件重新下载最大次数")]
        [Tooltip("对比文件最大重试次数（0为无限制）")]
        public int reDownloadCompareFileMaxNum = 5;

        /// <summary>
        /// 单次更新中AB包重新下载最大次数
        /// </summary>
        [Header("AB包重新下载最大次数")]
        [Tooltip("AB包重试下载最大次数（0为无限制）")]
        public int reDownloadAbMaxNum = 5;

        /// <summary>
        /// 最大并发数
        /// </summary>
        [Header("最大下载并发数")]
        [Tooltip("最大下载并发数")]
        public int maxConcurrencyNum = 8;

        /// <summary>
        /// 连接超时（s）
        /// </summary>
        [Header("连接超时")]
        [Tooltip("建立服务器连接的最大等待时间（s），0为无限制")]
        public int connectTimeout = 60;

        /// <summary>
        /// 单文件最大重试次数
        /// </summary>
        [Header("单文件最大重试次数")]
        [Tooltip("单文件最大重试次数（连接失败+下载失败）")]
        public int maxRetryCount = 5;

        /// <summary>
        /// 最大重试等待时间
        /// </summary>
        [Header("最大重试等待时间")]
        [Tooltip("重试前等待一段时间，避免频繁请求")]
        public float maxRetryWaitSeconds = 5f;

        /// <summary>
        /// 速度更新间隔
        /// </summary>
        [Header("速度更新间隔")]
        [Tooltip("单位时间内的下载量")]
        public float speedUpdateInterval = 1f;

        /// <summary>
        /// AB包数据加载路径类型
        /// </summary>
        [Header("AB包数据加载路径类型")]
        [Tooltip("确定从哪个文件夹加载AB包")]
        public EDataLoadPath abLoadPath = EDataLoadPath.Streaming;

        /// <summary>
        /// 用户数据加载/保存路径类型
        /// </summary>
        [Header("用户数据加载路径类型")]
        [Tooltip("确定从哪个文件夹加载/保存用户数据")]
        public EDataLoadPath userDataPath = EDataLoadPath.Streaming;

        /// <summary>
        /// AB包访问活跃阈值，高于该数值则放入热包列表，小于则放入冷包列表
        /// </summary>
        [Header("AB包访问活跃阈值")] 
        [Tooltip("AB包访问活跃阈值，高于该数值则放入热包列表，小于则放入冷包列表")]
        public int criticalActiveThreshold = 2;
        
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
        
        /// <summary>
        /// UDP接收缓冲区大小
        /// </summary>
        [Header("UDP接收缓冲区大小")]
        [Tooltip("缓存帧同步数据包")]
        public short udpReceiveBufferSize = 8192;

        /// <summary>
        /// TCP接收临时缓冲区大小
        /// </summary>
        [Header("TCP发送缓冲区大小")]
        [Tooltip("临时缓存接收的TCP消息")]
        public short tcpReceiveTempBufferSize = 1024;

        /// <summary>
        /// TCP接收缓冲区大小
        /// </summary>
        [Header("UDP接收缓冲区大小")]
        [Tooltip("缓存接收的TCP待处理消息")]
        public short tcpReceiveBufferSize = 1024;

        /// <summary>
        /// 心跳消息发送间隔时间（ms）
        /// </summary>
        [Header("心跳消息发送间隔时间（ms）")]
        [Tooltip("心跳消息发送间隔（ms）")]
        public short heartMsgSendIntervalTime = 3000;
    }
}

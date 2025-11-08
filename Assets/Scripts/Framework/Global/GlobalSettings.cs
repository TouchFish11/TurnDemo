using Framework;
using UnityEngine;

/// <summary>
/// 全局设置
/// </summary>
public sealed class GlobalSettings : SingletonSOBase<GlobalSettings>
{
    /// <summary>
    /// 日志过滤级别
    /// </summary>
    [Header("日志过滤级别")]
    [Tooltip("没有选中的日志类型将不会被记录")]
    public E_LogLevel FilterLevel = ~E_LogLevel.None;

    /// <summary>
    /// 日志写入最大间隔时间
    /// </summary>
    [Header("日志写入最大间隔时间")]
    [Tooltip("到达时间将写入一次日志到本地")]
    public ushort WriteLogMaxIntervalTime = 30;

    /// <summary>
    /// 是否启用缓存池布局——开发阶段使用
    /// </summary>
    [Header("启用对象池层级结构")]
    [Tooltip("对象池对象按层级结构布局")]
    public bool IsOpenLayout = true;

    /// <summary>
    /// 上传地址
    /// </summary>
    [Header("上传地址")]
    [Tooltip("上传到服务器指定文件路径（若存在）")]
    public string UploadServerIp = "http://ip:port/...";

    /// <summary>
    /// 资源服务器地址
    /// </summary>
    [Header("资源服务器地址")]
    [Tooltip("服务器资源下载路径")]
    public string ResServerIp = "http://ip:port/...";

    /// <summary>
    /// 单次更新中对比文件重新下载最大次数
    /// </summary>
    [Header("对比文件重新下载最大次数")]
    [Tooltip("对比文件最大重试次数（0为无限制）")]
    public int ReDownloadCompareFileMaxNum = 5;

    /// <summary>
    /// 单次更新中AB包重新下载最大次数
    /// </summary>
    [Header("AB包重新下载最大次数")]
    [Tooltip("AB包重试下载最大次数（0为无限制）")]
    public int ReDownloadAbMaxNum = 5;

    /// <summary>
    /// 最大并发数
    /// </summary>
    [Header("最大下载并发数")]
    [Tooltip("最大下载并发数")]
    public int MaxConcurrencyNum = 8;

    /// <summary>
    /// 连接超时
    /// </summary>
    [Header("连接超时")]
    [Tooltip("建立服务器连接的最大等待时间")]
    public int connectTimeout = 5;

    /// <summary>
    /// 下载超时
    /// </summary>
    [Header("下载超时")]
    [Tooltip("数据传输阶段的最大无进展时间")]
    public int downloadTimeout = 30;

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
    public float SpeedUpdateInterval = 1f;
}

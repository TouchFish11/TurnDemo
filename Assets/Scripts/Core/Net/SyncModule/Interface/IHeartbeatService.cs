using System;

namespace Core.Net.SyncModule.Interface
{
    /// <summary>
    /// 心跳服务接口
    /// </summary>
    public interface IHeartbeatService
    {
        /// <summary>
        /// 两次心跳之间的时间间隔（毫秒）
        /// </summary>
        int Interval { get; set; }
        
        /// <summary>
        /// 记录最近一次成功心跳的往返时间（RTT），单位毫秒
        /// </summary>
        long LastRttMilliseconds { get; } 
        
        /// <summary>
        /// 记录最后一次成功完成心跳（即收到回复）的 UTC 时间
        /// </summary>
        DateTime? LastHeartbeatUtc { get; }
        
        /// <summary>
        /// 超时阈值
        /// </summary>
        TimeSpan TimeoutThreshold { get; }
        
        /// <summary>
        /// TCP Rtt计算结果回调（ms）
        /// </summary>
        event Action<long> OnRttCalc;
        
        /// <summary>
        /// 表示连接当前是否仍然活跃（未超时）
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// 启动心跳循环（定时发送心跳、监控超时等）
        /// </summary>
        void Start();
        
        /// <summary>
        /// 停止心跳循环，释放相关资源
        /// </summary>
        void Stop();

        /// <summary>
        /// 手动发送一次心跳（可选），并异步等待发送完成（或等待回复，取决于设计）
        /// </summary>
        /// <returns></returns>
        void SendHeartbeatAsync();

        /// <summary>
        /// 计算RTT，触发<see cref="OnRttCalc"/>事件回调，传递当前RTT
        /// </summary>
        /// <param name="clientSendTime">客户端发送心跳的时间戳</param>
        void CalcRtt(long clientSendTime);
    }
}

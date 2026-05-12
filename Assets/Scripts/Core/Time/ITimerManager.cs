using UnityEngine.Events;

namespace Core.Time
{
    /// <summary>
    /// 计时器管理器
    /// </summary>
    public interface ITimerManager
    {
        /// <summary>
        /// 关闭定时器管理器
        /// 停止所有定时器驱动协程，终止定时器轮询
        /// </summary>
        void Close();
        
        /// <summary>
        /// 继续指定ID的定时器
        /// 将定时器状态设为运行中，恢复计时
        /// </summary>
        /// <param name="id">需要继续的定时器唯一ID</param>
        void ContinueTimer(int id);
        
        /// <summary>
        /// 创建一个新的定时器
        /// </summary>
        /// <param name="isRealTime">是否不受游戏时间影响（TimeScale）</param>
        /// <param name="maxTime">定时器总时长（单位：毫秒）</param>
        /// <param name="timeOverCallBack">定时器结束时的回调</param>
        /// <param name="intervalTime">定时器间隔回调时长（单位：毫秒，默认0表示不触发间隔回调）</param>
        /// <param name="intervalTimeOverCallBack">定时器间隔回调（当intervalTime>0时生效）</param>
        /// <returns>新创建的定时器唯一ID（用于后续操作）</returns>
        int CreateTimer(bool isRealTime, int maxTime, UnityAction timeOverCallBack, int intervalTime = 0, UnityAction intervalTimeOverCallBack = null);
        
        /// <summary>
        /// 获取指定ID的定时器对象
        /// </summary>
        /// <param name="id">定时器唯一ID</param>
        /// <returns>定时器对象（不存在则返回null）</returns>
        Timer GetTimer(int id);
        
        /// <summary>
        /// 暂停指定ID的定时器
        /// 将定时器状态设为非运行中，暂停计时
        /// </summary>
        /// <param name="id">需要暂停的定时器唯一ID</param>
        void PauseTimer(int id);
        
        /// <summary>
        /// 移除指定ID的定时器
        /// 标记定时器为待删除，在轮询阶段统一清理（避免遍历中修改字典）
        /// </summary>
        /// <param name="id">需要移除的定时器唯一ID</param>
        void RemoveTimer(int id);
        
        /// <summary>
        /// 重置指定ID的定时器
        /// 将定时器的剩余时间恢复为初始值，重新开始计时
        /// </summary>
        /// <param name="id">需要重置的定时器唯一ID</param>
        void ResetTimer(int id);
        
        /// <summary>
        /// 设置全局时间流速（影响游戏时间缩放）
        /// </summary>
        /// <param name="timeRate">时间流速枚举（Normal/Zero/Recovery等）</param>
        void SetTimeRate(E_TimeRate timeRate);
    }
}

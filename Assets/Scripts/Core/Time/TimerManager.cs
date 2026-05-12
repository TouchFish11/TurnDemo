using System.Collections;
using System.Collections.Generic;
using Core.Mono;
using Core.Pool;
using Core.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Time
{
    /// <summary>
    /// 定时器管理器（单例）
    /// 负责统一管理所有基于游戏时间/真实时间的定时器，提供创建、重置、暂停、继续、移除定时器等功能
    /// 支持时间缩放（TimeScale）控制，定时器对象使用对象池复用
    /// </summary>
    public class TimerManager : ITimerManager
    {
        private readonly IMonoAdapter _monoAdapter;
        private readonly IPoolManager _poolManager;
        // 存储受游戏时间影响的定时器字典（Key：定时器唯一ID，Value：定时器对象）
        private readonly Dictionary<int, Timer> _timerDic = new();
        // 存储不受游戏时间影响的定时器字典（Key：定时器唯一ID，Value：定时器对象）
        private readonly Dictionary<int, Timer> _realTimerDic = new();
        // 存储待删除的受游戏时间影响的定时器ID列表
        private readonly List<int> _delTimerIDList = new();
        // 存储待删除的不受游戏时间影响的定时器ID列表
        private readonly List<int> _realDelTimerIDList = new();
        // 定时器全局唯一ID生成器（自增）
        private static int _TimerKey;
        // 受游戏时间影响的定时器驱动协程
        private readonly Coroutine _coroutine;
        // 不受游戏时间影响的定时器驱动协程
        private readonly Coroutine _realCoroutine;
        // 定时器轮询间隔（单位：秒），每0.1秒检查一次定时器状态
        private const float IntervalTime = 0.1f;
        // 受游戏时间影响的协程等待对象（复用避免重复创建）
        private readonly WaitForSeconds _WaitForSecondsTime = new(IntervalTime);
        // 不受游戏时间影响的协程等待对象（复用避免重复创建）
        private readonly WaitForSecondsRealtime _WaitForSecondsRealTime = new(IntervalTime);
        // 当前全局时间流速（控制TimeScale）
        private E_TimeRate _timeRate;
        
        private TimerManager(IMonoAdapter monoAdapter, IPoolManager poolManager)
        {
            // 初始化时间流速为正常速度
            _timeRate = E_TimeRate.Normal;
            // 启动受游戏时间影响的定时器轮询协程
            _coroutine = monoAdapter.StartCoroutine(StartTiming(false, _timerDic));
            // 启动不受游戏时间影响的定时器轮询协程
            _realCoroutine = monoAdapter.StartCoroutine(StartTiming(true, _realTimerDic));
            
            _monoAdapter = monoAdapter;
            _poolManager = poolManager;
        }

        public void Close()
        {
            // 停止受游戏时间影响的定时器协程
            _monoAdapter.StopCoroutine(_coroutine);
            // 停止不受游戏时间影响的定时器协程
            _monoAdapter.StopCoroutine(_realCoroutine);
        }

        /// <summary>
        /// 定时器核心轮询协程
        /// 周期性检查并更新所有定时器状态，处理定时器回调和删除逻辑
        /// </summary>
        /// <param name="isRealTime">是否为不受游戏时间影响的定时器</param>
        /// <param name="timerDic">待轮询的定时器字典</param>
        /// <returns>协程迭代器</returns>
        private IEnumerator StartTiming(bool isRealTime, Dictionary<int, Timer> timerDic)
        {
            // 无限循环轮询（直到协程被停止）
            while (true)
            {
                // 遍历所有定时器，更新时间并检查回调条件
                foreach (var timer in timerDic.Values)
                {
                    // 跳过未运行状态的定时器
                    if (!timer.IsRunning)
                        continue;

                    // 更新定时器间隔时间（转换为毫秒计算，避免浮点精度问题）
                    timer.NowIntervalTime -= (int)(IntervalTime * 1000);
                    // 间隔时间耗尽时，执行间隔回调
                    if (timer.NowIntervalTime <= 0)
                        timer.IntervalInvoke();

                    // 更新定时器总剩余时间（转换为毫秒计算）
                    timer.NowTime -= (int)(IntervalTime * 1000);
                    // 总时间耗尽时，执行结束回调并标记待删除
                    if (timer.NowTime > 0) continue;
                    timer.OverInvoke();
                    // 将定时器ID加入待删除列表（延迟删除，避免遍历中修改字典）
                    _delTimerIDList.Add(timer.Id);
                }

                // 处理待删除的定时器（区分真实时间/游戏时间）
                if (isRealTime)
                {
                    // 遍历真实时间定时器的待删除列表
                    for (var i = 0; i < _realDelTimerIDList.Count; i++)
                    {
                        // 检查字典中是否存在该ID的定时器
                        if (!timerDic.ContainsKey(_realDelTimerIDList[i])) continue;
                        // 将定时器对象归还至对象池（复用）
                        _poolManager.PushData(timerDic[_realDelTimerIDList[i]]);
                        // 从字典中移除该定时器
                        timerDic.Remove(_realDelTimerIDList[i]);
                    }
                    // 清空真实时间定时器的待删除列表
                    _realDelTimerIDList.Clear();
                }
                else
                {
                    // 遍历游戏时间定时器的待删除列表
                    for (var i = 0; i < _delTimerIDList.Count; i++)
                    {
                        // 检查字典中是否存在该ID的定时器
                        if (!timerDic.ContainsKey(_delTimerIDList[i])) continue;
                        // 将定时器对象归还至对象池（复用）
                        _poolManager.PushData(timerDic[_delTimerIDList[i]]);
                        // 从字典中移除该定时器
                        timerDic.Remove(_delTimerIDList[i]);
                    }
                    // 清空游戏时间定时器的待删除列表
                    _delTimerIDList.Clear();
                }

                // 等待指定间隔后继续轮询（区分真实时间/游戏时间）
                if (isRealTime)
                    yield return _WaitForSecondsRealTime;
                else
                    yield return _WaitForSecondsTime;
            }
        }
        
        public int CreateTimer(bool isRealTime, int maxTime, UnityAction timeOverCallBack, int intervalTime = 0, UnityAction intervalTimeOverCallBack = null)
        {
            // 从对象池获取定时器对象（复用，避免频繁创建销毁）
            var timer = _poolManager.GetData<Timer>();
            // 初始化定时器参数（生成唯一ID，设置时长、回调等）
            timer.InitTimer(++_TimerKey, maxTime, timeOverCallBack, intervalTime, intervalTimeOverCallBack);
            // 根据是否为真实时间，将定时器加入对应字典
            if(isRealTime)
                _realTimerDic.Add(_TimerKey, timer);
            else
                _timerDic.Add(_TimerKey, timer);
            // 返回定时器唯一ID
            return _TimerKey;
        }

        public void ResetTimer(int id)
        {
            if(_timerDic.TryGetValue(id, out var timer))
            {
                timer.ResetTimer();
            }
            else if(_realTimerDic.TryGetValue(id, out var realTimer))
            {
                realTimer.ResetTimer();
            }
        }

        public void RemoveTimer(int id)
        {
            if (_timerDic.ContainsKey(id))
            {
                _delTimerIDList.Add(id);
            }
            else if (_realTimerDic.ContainsKey(id))
            {
                _realDelTimerIDList.Add(id);
            }
        }

        public void ContinueTimer(int id)
        {
            if (_timerDic.TryGetValue(id, out var timer))
            {
                timer.IsRunning = true;
            }
            else if (_realTimerDic.TryGetValue(id, out var realTimer))
            {
                realTimer.IsRunning = true;
            }
        }

        public void PauseTimer(int id)
        {
            if (_timerDic.TryGetValue(id, out var timer))
            {
                timer.IsRunning = false;
            }
            else if (_realTimerDic.TryGetValue(id, out var realTimer))
            {
                realTimer.IsRunning = false;
            }
        }
        
        public Timer GetTimer(int id)
        {
            return _timerDic.TryGetValue(id, out var timer) ? timer : _realTimerDic.GetValueOrDefault(id);
        }

        public void SetTimeRate(E_TimeRate timeRate)
        {
            // 非恢复/非零速时，更新本地枚举并设置TimeScale
            if (timeRate != E_TimeRate.Recovery && timeRate != E_TimeRate.Zero)
            {
                _timeRate = timeRate;
                TimeUtil.Timescale = (int)_timeRate;
            }
            // 恢复时间流速时，直接设置TimeScale为恢复值
            else if(timeRate == E_TimeRate.Recovery)
            {
                TimeUtil.Timescale = (int)_timeRate;
            }
            // 零速时，直接设置TimeScale为0
            else
            {
                TimeUtil.Timescale = (int)timeRate;
            }
        }
    }
}
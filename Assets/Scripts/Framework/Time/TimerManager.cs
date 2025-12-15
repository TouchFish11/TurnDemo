using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 计时器管理器
    /// </summary>
    public class TimerManager : SingletonBase<TimerManager>
    {
        //存储受时间缩放影响的计时器的字典
        private readonly Dictionary<int, Timer> _timerDic = new Dictionary<int, Timer>();
        //存储不受时间缩放影响的计时器的字典
        private readonly Dictionary<int, Timer> _realTimerDic = new Dictionary<int, Timer>();
        //存储受时间缩放影响的计时器的待移除列表
        private readonly List<int> _delTimerIDList = new List<int>();
        //存储不受时间缩放影响的计时器的待移除列表
        private readonly List<int> _realDelTimerIDList = new List<int>();
        //计时器唯一ID
        private static int _TimerKey;
        //受时间缩放影响的计时器协程
        private Coroutine _coroutine;
        //不受时间缩放影响的计时器协程
        private Coroutine _realCoroutine;
        //计时更新间隔
        private const float IntervalTime = 0.1f;
        //受时间缩放影响的协程返回对象
        private readonly WaitForSeconds _WaitForSecondsTime = new WaitForSeconds(IntervalTime);
        //不受时间缩放影响的协程返回对象
        private readonly WaitForSecondsRealtime _WaitForSecondsRealTime = new WaitForSecondsRealtime(IntervalTime);
        //当前设置的时间速度
        private E_TimeRate _timeRate;

        private TimerManager()
        {
            //初始化时间速度
            _timeRate = E_TimeRate.Normal;
            Start();
        }

        /// <summary>
        /// 开启计时器管理器
        /// </summary>
        private void Start()
        {
            //开启受时间缩放影响的计时器
            _coroutine = MonoManager.Instance.StartCoroutine(StartTiming(false, _timerDic));
            //开启不受时间缩放影响的计时器
            _realCoroutine = MonoManager.Instance.StartCoroutine(StartTiming(true, _realTimerDic));
        }

        /// <summary>
        /// 关闭计时器管理器
        /// </summary>
        public void Close()
        {
            //关闭受时间缩放影响的计时器
            MonoManager.Instance.StopCoroutine(_coroutine);
            //关闭不受时间缩放影响的计时器
            MonoManager.Instance.StopCoroutine(_realCoroutine);
        }

        /// <summary>
        /// 开始计时协程
        /// </summary>
        /// <param name="isRealTime">是否受时间缩放影响</param>
        /// <param name="timerDic">计时器字典</param>
        /// <returns></returns>
        private IEnumerator StartTiming(bool isRealTime, Dictionary<int, Timer> timerDic)
        {
            while (true)
            {
                foreach (Timer timer in timerDic.Values)
                {
                    //标识为不在计时的计时器不参与计时
                    if (!timer.IsRunning)
                        continue;

                    //间隔时间更新
                    timer.NowIntervalTime -= (int)(IntervalTime * 1000);
                    //剩余间隔时间小于等于0，执行间隔时间结束回调
                    if (timer.NowIntervalTime <= 0)
                        timer.IntervalInvoke();

                    //总时间更新
                    timer.NowTime -= (int)(IntervalTime * 1000);
                    //剩余总时间小于等于0，执行总时间结束回调
                    if (timer.NowTime <= 0)
                    {
                        //计时完毕，放入待移除列表
                        _delTimerIDList.Add(timer.Id);
                        timer.OverInvoke();
                    }
                }

                if (isRealTime)
                {
                    //移除计时结束的计时器
                    for (int i = 0; i < _realDelTimerIDList.Count; i++)
                    {
                        //找到有该ID的计时器
                        if (timerDic.ContainsKey(_realDelTimerIDList[i]))
                        {
                            //放入缓存池
                            PoolManager.Instance.PushData(timerDic[_realDelTimerIDList[i]]);
                            //从字典中移除
                            timerDic.Remove(_realDelTimerIDList[i]);
                        }
                    }
                    //清空待删除列表
                    _realDelTimerIDList.Clear();
                }
                else
                {
                    //移除计时结束的计时器
                    for (int i = 0; i < _delTimerIDList.Count; i++)
                    {
                        //找到有该ID的计时器
                        if (timerDic.ContainsKey(_delTimerIDList[i]))
                        {
                            //放入缓存池
                            PoolManager.Instance.PushData(timerDic[_delTimerIDList[i]]);
                            //从字典中移除
                            timerDic.Remove(_delTimerIDList[i]);
                        }
                    }
                    //清空待删除列表
                    _delTimerIDList.Clear();
                }

                //100ms更新一次
                if (isRealTime)
                    yield return _WaitForSecondsRealTime;
                else
                    yield return _WaitForSecondsTime;
            }
        }

        /// <summary>
        /// 创建计时器
        /// </summary>
        /// <param name="isRealTime">是否受时间缩放影响</param>
        /// <param name="maxTime">最大时间</param>
        /// <param name="timeOverCallBack">结束回调</param>
        /// <param name="intervalTime">间隔时间</param>
        /// <param name="intervalTimeOverCallBack">间隔时间回调</param>
        /// <returns>计时器唯一ID</returns>
        public int CreateTimer(bool isRealTime, int maxTime, UnityAction timeOverCallBack, int intervalTime = 0, UnityAction intervalTimeOverCallBack = null)
        {
            //从缓存池中获取计时器对象
            Timer timer = PoolManager.Instance.GetData<Timer>();
            //初始化计时器
            timer.InitTimer(++_TimerKey, maxTime, timeOverCallBack, intervalTime, intervalTimeOverCallBack);
            //根据是否受时间缩放影响放入不同字典中
            if(isRealTime)
                _realTimerDic.Add(_TimerKey, timer);
            else
                _timerDic.Add(_TimerKey, timer);
            //返回计时器唯一ID
            return _TimerKey;
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        /// <param name="id">计时器唯一ID</param>
        public void ResetTimer(int id)
        {
            if(_timerDic.ContainsKey(id))
            {
                _timerDic[id].ResetTimer();
            }
            else if(_realTimerDic.ContainsKey(id))
            {
                _realTimerDic[id].ResetTimer();
            }
        }

        /// <summary>
        /// 移除指定计时器
        /// </summary>
        /// <param name="id">计时器唯一ID</param>
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

        /// <summary>
        /// 继续计时
        /// </summary>
        /// <param name="id">计时器唯一ID</param>
        public void ContinueTimer(int id)
        {
            if (_timerDic.ContainsKey(id))
                _timerDic[id].IsRunning = true;
            else if (_realTimerDic.ContainsKey(id))
                _realTimerDic[id].IsRunning = true;
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        /// <param name="id">计时器唯一ID</param>
        public void PauseTimer(int id)
        {
            if (_timerDic.ContainsKey(id))
                _timerDic[id].IsRunning = false;
            else if (_realTimerDic.ContainsKey(id))
                _realTimerDic[id].IsRunning = false;
        }

        /// <summary>
        /// 获取指定计时器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Timer GetTimer(int id)
        {
            if (_timerDic.ContainsKey(id))
                return _timerDic[id];
            else if (_realTimerDic.ContainsKey(id))
                return _realTimerDic[id];
            return null;
        }

        /// <summary>
        /// 设置时间速率
        /// </summary>
        /// <param name="timeRate"></param>
        public void SetTimeRate(E_TimeRate timeRate)
        {
            //不等于恢复或0时，才去设置和更新时间
            if (timeRate != E_TimeRate.Recovery && timeRate != E_TimeRate.Zero)
            {
                _timeRate = timeRate;
                Time.timeScale = (int)_timeRate;
            }
            //等于恢复时，直接设置为上次时间速度即可
            else if(timeRate == E_TimeRate.Recovery)
            {
                Time.timeScale = (int)_timeRate;
            }
            else
            {
                Time.timeScale = (int)timeRate;
            }
        }
    }
}

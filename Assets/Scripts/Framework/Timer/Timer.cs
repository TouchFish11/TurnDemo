using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 计时器
    /// </summary>
    public class Timer : IPoolData
    {
        //计时器唯一ID
        private int _id;
        //剩余时间(ms)
        private int _nowTime;
        //最大时间(ms)
        private int _maxTime;
        //间隔剩余时间(ms)
        private int _nowIntervalTime;
        //最大间隔时间(ms)
        private int _maxIntervalTime;
        //总时间结束回调
        private  UnityAction _allTimeOverCallBack;
        //间隔时间结束回调
        private  UnityAction _intervalTimeOverCallBack;
        //是否正在计时
        private bool _isRunning;

        /// <summary>
        /// 初始化计时器
        /// </summary>
        /// <param name="id">唯一ID</param>
        /// <param name="maxTime">最大时间</param>
        /// <param name="timeOverCallBack">时间结束回调</param>
        /// <param name="maxIntervalTime">可选：间隔时间</param>
        /// <param name="intervalTimeOverCallBack">可选：间隔时间回调</param>
        public void InitTimer(int id, int maxTime, UnityAction timeOverCallBack, int maxIntervalTime = 0, UnityAction intervalTimeOverCallBack = null)
        {
            this._id = id;
            this._maxTime = _nowTime = maxTime;
            this._maxIntervalTime = _nowIntervalTime = maxIntervalTime;
            this._allTimeOverCallBack = timeOverCallBack;
            this._intervalTimeOverCallBack = intervalTimeOverCallBack;
            this._isRunning = true;
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        public void ResetTimer()
        {
            _nowTime = _maxTime;
            _nowIntervalTime = _maxIntervalTime;
            _isRunning = true;
        }

        /// <summary>
        /// 计时器结束回调
        /// </summary>
        public void OverInvoke()
        {
            _allTimeOverCallBack?.Invoke();
        }

        /// <summary>
        /// 计时器间隔时间回调
        /// </summary>
        public void IntervalInvoke()
        {
            _intervalTimeOverCallBack?.Invoke();
            _nowIntervalTime = _maxIntervalTime;
        }

        public void ResetData()
        {
            _id = -1;
            _nowTime = _maxTime = 0;
            _nowIntervalTime = _maxIntervalTime = 0;
            IsRunning = false;
            //清空委托
            _allTimeOverCallBack = null;
            _intervalTimeOverCallBack = null;
        }

        /// <summary>
        /// 是否正在计时
        /// </summary>
        public bool IsRunning { get => _isRunning; set => _isRunning = value; }

        /// <summary>
        /// 剩余的总时间
        /// </summary>
        public int NowTime { get => _nowTime; set => _nowTime = value; }

        /// <summary>
        /// 剩余的间隔时间
        /// </summary>
        public int NowIntervalTime { get => _nowIntervalTime; set => _nowIntervalTime = value; }

        /// <summary>
        /// 计时器唯一ID
        /// </summary>
        public int Id { get => _id; }
    }
}

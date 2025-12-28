using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 日期时间
    /// </summary>
    public class DateTime : IPoolData
    {
        // 指定的天数
        private int _targetDay;
        // 指定的小时数
        private int _targetHour;
        // 指定的分钟数
        private int _targetMinute;
        // 指定的秒数
        private int _targetSecond;
        //真正的时间
        private System.DateTime realTargetTime;
        //剩余时间为0时的回调
        public event UnityAction OverCallBack;

        /// <summary>
        /// 初始化时间对象
        /// </summary>
        /// <param name="currentTime">当前时间结构体</param>
        /// <param name="targetDay">指定天数</param>
        /// <param name="targetHour">指定小时</param>
        /// <param name="targetMin">指定分钟</param>
        /// <param name="targetSec">指定秒数</param>
        /// <returns>时间对象</returns>
        public DateTime Init(System.DateTime currentTime, int targetDay, int targetHour, int targetMin, int targetSec)
        {
            this._targetDay = targetDay;
            this._targetHour = targetHour;
            this._targetMinute = targetMin;
            this._targetSecond = targetSec;

            //检测天数
            CheckDays(currentTime);

            return this;
        }

        /// <summary>
        /// 计算剩余时间
        /// </summary>
        /// <param name="currentTime">当前时间结构体</param>
        /// <returns>剩余时间（秒）</returns>
        public long CalcRemainTime(System.DateTime currentTime)
        {
            // 如果今天的指定时间已经过去，那么目标时间是下次的指定时间
            if (currentTime > this.realTargetTime)
            {
                //检测天数
                CheckDays(currentTime);

                //剩余时间小于等于0，执行回调
                if ((long)(this.realTargetTime - currentTime).TotalSeconds <= 0)
                {
                    OverCallBack?.Invoke();
                }

                return (long)(this.realTargetTime - currentTime).TotalSeconds;
            }
            // 否则目标时间就是今天的指定时间
            return (long)(this.realTargetTime - currentTime).TotalSeconds;
        }

        public void ResetData()
        {
            this.OverCallBack = null;
        }

        /// <summary>
        /// 检测天数
        /// </summary>
        /// <param name="currentTime">当前时间结构体</param>
        private void CheckDays(System.DateTime currentTime)
        {
            if (_targetDay != 0)
            {
                if (currentTime.Day + _targetDay > System.DateTime.DaysInMonth(currentTime.Year, currentTime.Month))
                {
                    int deltaDay = 0;
                    //获取当前月共有多少天
                    int daysInMonth = System.DateTime.DaysInMonth(currentTime.Year, currentTime.Month);
                    //相差天数 = 当前天数 + 目标天数 - 当前月总天数
                    deltaDay = currentTime.Day + _targetDay - daysInMonth;
                    //相差天数大于当前月总天数
                    while (deltaDay > daysInMonth)
                    {
                        //让当前时间加一个月
                        currentTime = currentTime.AddMonths(1);
                        //让相差天数减去当前时间的天数
                        deltaDay = deltaDay - currentTime.Day;
                    }

                    this.realTargetTime = new System.DateTime(currentTime.Year, currentTime.Month, deltaDay, _targetHour, _targetMinute, _targetSecond);
                }
                else
                {
                    this.realTargetTime = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day + _targetDay, _targetHour, _targetMinute, _targetSecond);
                }
            }
            else
            {
                this.realTargetTime = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day + 1, _targetHour, _targetMinute, _targetSecond);
                if (currentTime < this.realTargetTime.AddDays(-1))
                {
                    this.realTargetTime = this.realTargetTime.AddDays(-1);
                }
            }
        }
    }
}

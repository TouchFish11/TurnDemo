using System.Collections.Generic;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 时间检查器(本地)
    /// </summary>
    public class TimeChecker : ITimeChecker
    {
        // 存储时间对象字典 Key：唯一键，Value；时间对象
        private Dictionary<int, DateTime> _dateTimeDic = new Dictionary<int, DateTime>();

        /// <summary>
        /// 时间对象唯一键
        /// </summary>
        private static int TIME_KEY = 0;

        /// <summary>
        /// 创建目标时间
        /// </summary>
        /// <param name="currentTime">当前时间结构体</param>
        /// <param name="targetDay">指定天数</param>
        /// <param name="targetHour">指定小时</param>
        /// <param name="targetMin">指定分钟</param>
        /// <param name="targetSec">指定秒数</param>
        /// <returns>时间对象对应Key</returns>
        public int CreateTargetTime(System.DateTime currentTime, int targetDay, int targetHour, int targetMin, int targetSec)
        {
            // 创建今天指定时间的 DateTime 对象
            DateTime tagetTime = PoolManager.Instance.GetData<DateTime>("GameUtility");
            //初始化时间对象
            tagetTime = tagetTime.Init(currentTime, targetDay, targetHour, targetMin, targetSec);
            //存储进字典
            _dateTimeDic.Add(++TIME_KEY, tagetTime);
            //返回值时间对应的键
            return TIME_KEY;
        }

        /// <summary>
        /// 添加时间结束监听
        /// </summary>
        /// <param name="key">时间对象对应的键</param>
        /// <param name="overCallBack">结束回调</param>
        public void AddListener(int key, UnityAction overCallBack)
        {
            GetDateTime(key).OverCallBack += overCallBack;
        }

        /// <summary>
        /// 计算剩余时间
        /// </summary>
        /// <param name="current">当前时间</param>
        /// <param name="key">时间对象Key</param>
        /// <returns>当前剩余时间（秒）</returns>
        public long CalcRemainTime(System.DateTime current, int key)
        {
            if (_dateTimeDic.ContainsKey(key))
            {
                return _dateTimeDic[key].CalcRemainTime(current);
            }

            LogManager.LogError($"未找到该指定的时间键，KEY：{key}");
            return default;
        }

        /// <summary>
        /// 获取键对应的时间对象
        /// </summary>
        /// <param name="key">时间对象Key</param>
        /// <returns>时间对象</returns>
        public DateTime GetDateTime(int key)
        {
            if (_dateTimeDic.ContainsKey(key))
                return _dateTimeDic[key];

            LogManager.LogError($"未找到该指定的时间键，KEY：{key}");
            return default;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            _dateTimeDic.Clear();
            _dateTimeDic = null;
        }
    }
}
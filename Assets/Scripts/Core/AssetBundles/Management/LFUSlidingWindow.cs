using Core.Time;
using UnityEngine;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// LFU滑动窗口
    /// </summary>
    internal class LFUSlidingWindow
    {
        // 环形数组，每个元素存这T秒的访问次数
        private readonly int[] _slots;
        // 当前指在哪个格子
        private int _currentIndex;     
        // 上一次移动指针的时间
        private float _lastUpdateTime; 
        // 每个格子代表多少秒
        private readonly float _slotDuration;
        // 总频率数
        private int _totalCount;

        public LFUSlidingWindow(int windowSizeSlots, float slotDuration)
        {
            _slots = new int[windowSizeSlots];
            _slotDuration = slotDuration;
            _currentIndex = 0;
            _lastUpdateTime = -1;
            _totalCount = 0;
        }

        /// <summary>
        /// 每次加载/访问 AB 包里的资源时，调用这个方法更新记录
        /// </summary>
        public void RecordAccess()
        {
            if (Mathf.Approximately(_lastUpdateTime, -1))
            {
                _lastUpdateTime = TimeUtil.RealtimeSinceStartup;
                _slots[_currentIndex]++;
                _totalCount++;
                return;
            }
            
            // 看看是不是该往前转一格了
            var now = TimeUtil.RealtimeSinceStartup;
            var timePassed = now - _lastUpdateTime;

            if (timePassed >= _slotDuration)
            {
                // 计算需要往前转几格
                var steps = (int)(timePassed / _slotDuration);
                // 把走过的那几个格子全部清零
                for (var i = 1; i <= steps; i++)
                {
                    /*
                     当前所在的索引是包含在旧时间段的，是属于上一次旧时间段的，所以当前不能覆盖上次的，
                     而是从下一个格子开始清理，不然的话上次时间段的数据就不完整，少了一格数据（因为被当前清理了）
                    */
                    _currentIndex = (_currentIndex + 1) % _slots.Length;
                    // 更新总频率
                    _totalCount -= _slots[_currentIndex];
                    // 旧数据扔掉
                    _slots[_currentIndex] = 0;
                }
            
                // 更新时间
                _lastUpdateTime = now;
            }

            // 在当前格子里 +1
            _slots[_currentIndex]++;
            _totalCount++;
        }

        /// <summary>
        /// 获取当前 LFU 热度值（只看最近这几个格子的总和）
        /// </summary>
        /// <returns></returns>
        public int GetCurrentHotness()
        {
            // 清理过期数据，确保精确
            CatchUp();
            return _totalCount;
        }

        private void CatchUp()
        {
            if (Mathf.Approximately(_lastUpdateTime, -1))
            {
                return;
            }
            
            // 看看是不是该往前转一格了
            var now = TimeUtil.RealtimeSinceStartup;
            var timePassed = now - _lastUpdateTime;
            if (timePassed >= _slotDuration)
            {
                // 计算需要往前转几格
                var steps = (int)(timePassed / _slotDuration);
                // 把走过的那几个格子全部清零
                for (var i = 1; i <= steps; i++)
                {
                    /*
                     当前所在的索引是包含在旧时间段的，是属于上一次旧时间段的，所以当前不能覆盖上次的，
                     而是从下一个格子开始清理，不然的话上次时间段的数据就不完整，少了一格数据（因为被当前清理了）
                    */
                    _currentIndex = (_currentIndex + 1) % _slots.Length;
                    // 更新总频率
                    _totalCount -= _slots[_currentIndex];
                    // 旧数据扔掉
                    _slots[_currentIndex] = 0;
                }
            
                // 更新时间
                _lastUpdateTime = now;
            }
        }
    }
}

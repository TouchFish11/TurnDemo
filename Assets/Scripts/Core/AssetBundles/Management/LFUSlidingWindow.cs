using Core.Utility;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// LFU滑动窗口
    /// </summary>
    public class LFUSlidingWindow
    {
        // 环形数组，每个元素存这T秒的访问次数
        private readonly int[] _slots;
        // 当前指在哪个格子
        private int _currentIndex;     
        // 上一次移动指针的时间
        private double _lastUpdateTime; 
        // 每个格子代表多少秒
        private readonly float _slotDuration;

        public LFUSlidingWindow(int windowSizeSlots, float slotDuration)
        {
            _slots = new int[windowSizeSlots];
            _slotDuration = slotDuration;
            _currentIndex = 0;
            _lastUpdateTime = TimeUtil.RealtimeSinceStartupAsDouble;
        }

        /// <summary>
        /// 每次加载/访问 AB 包里的资源时，调用这个方法更新记录
        /// </summary>
        public void RecordAccess()
        {
            // 看看是不是该往前转一格了
            var now = TimeUtil.RealtimeSinceStartupAsDouble;
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
                    // 旧数据扔掉
                    _slots[_currentIndex] = 0; 
                }
            
                // 更新时间
                _lastUpdateTime = now;
            }

            // 在当前格子里 +1
            _slots[_currentIndex]++;
        }

        /// <summary>
        /// 获取当前 LFU 热度值（只看最近这几个格子的总和）
        /// </summary>
        /// <returns></returns>
        public int GetCurrentHotness()
        {
            // 简单粗暴：把所有格子里的数加起来就是最近 N 秒的热度
            var total = 0;
            foreach (var t in _slots) total += t;
            return total;
        }
    }
}

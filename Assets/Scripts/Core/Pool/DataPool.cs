using System.Collections.Generic;
using Core.Time;

namespace Core.Pool
{
    /// <summary>
    /// 缓存不继承Mono对象的数据池
    /// </summary>
    public sealed class DataPool<T> : IPool<T> where T : class, IPoolData
    {
        // 存储未使用的数据对象队列
        private readonly Queue<T> _unUsedDatas = new();
        // 活跃时间阈值，大于等于该数值活跃，小于则惰性
        private readonly float _activeTimeThreshold;
        // 最小缓存数量
        private int _minSize;
        // 最大缓存容量
        private int _maxSize;

        public string PoolId { get; }
        
        public bool IsLazy => _activeTimeThreshold > TimeUtil.RealtimeSinceStartup - LastUsedTime;
        
        public float LastUsedTime { get; private set; }
        
        public int ActiveCount { get; private set; }
        
        public int InactiveCount => _unUsedDatas.Count;

        public DataPool(float activeTimeThreshold, int minSize, int maxSize)
        {
            _activeTimeThreshold = activeTimeThreshold;
            _minSize = minSize;
            _maxSize = maxSize;
            ActiveCount = 1;
            PoolId = typeof(T).FullName;
        }

        /// <summary>
        /// 获取缓存的数据对象，外部判断是否存在缓存
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            --ActiveCount;
            LastUsedTime = TimeUtil.RealtimeSinceStartup;
            return _unUsedDatas.Dequeue();
        }
        
        /// <summary>
        /// 缓存对象
        /// </summary>
        /// <param name="data">不使用的数据对象</param>
        public void Push(T data)
        {
            ++ActiveCount;
            LastUsedTime = TimeUtil.RealtimeSinceStartup;
            //重置数据
            data.ResetData();
            _unUsedDatas.Enqueue(data);
        }
        
        public void Trim()
        {
            while (_unUsedDatas.TryDequeue(out _) && _unUsedDatas.Count > _minSize)
            {
                
            }
        }
        
        public void ClearAll()
        {
            _unUsedDatas.Clear();
        }
    }
}

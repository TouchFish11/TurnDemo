using System.Collections.Generic;

namespace Core.Pool
{
    /// <summary>
    /// 不继承Mono对象
    /// </summary>
    public sealed class PoolData<T> : BasePoolData where T : class, IPoolData, new()
    {
        //存储未使用的数据对象队列
        private readonly Queue<T> _unUsedDatas = new();

        /// <summary>
        /// 获取缓存的数据对象，外部判断是否存在缓存
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            return _unUsedDatas.Dequeue();
        }
        
        /// <summary>
        /// 缓存对象
        /// </summary>
        /// <param name="data">不使用的数据对象</param>
        public void Push(T data)
        {
            //重置数据
            data.ResetData();
            _unUsedDatas.Enqueue(data);
        }

        /// <summary>
        /// 清空所有缓存的C#类
        /// </summary>
        public void Clear()
        {
            _unUsedDatas.Clear();
        }

        /// <summary>
        /// 未使用对象数量
        /// </summary>
        public int UnUsedCount => _unUsedDatas.Count;
    }
}

using System.Collections.Generic;

namespace Core.Pool
{
    /// <summary>
    /// 不继承Mono对象
    /// </summary>
    public sealed class PoolData<T> : BasePoolData where T : class, IPoolData, new()
    {
        //存储未使用的数据对象队列
        private readonly Queue<T> _unUsedDataList = new Queue<T>();

        /// <summary>
        /// 获取缓存的数据对象
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            return _unUsedDataList.Dequeue();
        }
        
        /// <summary>
        /// 缓存对象
        /// </summary>
        /// <param name="data">不使用的数据对象</param>
        public void Push(T data)
        {
            //重置数据
            data.ResetData();
            _unUsedDataList.Enqueue(data);
        }

        /// <summary>
        /// 未使用对象数量
        /// </summary>
        public int UnUsedCount => _unUsedDataList.Count;
    }
}

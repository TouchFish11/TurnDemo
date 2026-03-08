using System.Collections.Generic;

namespace Core.Collection
{
    public interface ICollection
    {
        
    }
    
    public interface ICollection<in TKey, TValue> : ICollection
    {
        /// <summary>
        /// 索引器，通过键快速获取对应的值
        /// </summary>
        /// <param name="key">要查找的键</param>
        /// <exception cref="KeyNotFoundException">当指定的键不存在时抛出</exception>
        /// <returns>键对应的值</returns>
        TValue this[TKey key] { get; }

        /// <summary>
        /// 获取集合中键值对的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 检查集合中是否包含指定的键
        /// </summary>
        /// <param name="key">要检查的键</param>
        /// <returns>存在返回true，不存在返回false</returns>
        bool ContainsKey(TKey key);

        /// <summary>
        /// 尝试向集合中添加键值对
        /// </summary>
        /// <param name="key">要添加的键</param>
        /// <param name="value">要添加的值</param>
        /// <returns>添加成功返回true；若键已存在，打印日志并返回false</returns>
        bool TryAdd(TKey key, TValue value);

        /// <summary>
        /// 从集合中移除指定键对应的键值对
        /// </summary>
        /// <param name="key">要移除的键</param>
        /// <returns>移除成功返回true；键不存在返回false</returns>
        bool Remove(TKey key);

        /// <summary>
        /// 尝试获取指定键对应的值
        /// </summary>
        /// <param name="key">要查找的键</param>
        /// <param name="value">输出参数，找到则为对应值，未找到则为类型默认值</param>
        /// <returns>找到键返回true，未找到返回false</returns>
        bool TryGetValue(TKey key, out TValue value);

        /// <summary>
        /// 清空集合中所有的键值对
        /// </summary>
        void Clear();
    }
}

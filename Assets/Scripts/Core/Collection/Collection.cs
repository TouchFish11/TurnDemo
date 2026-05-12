using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.Collection
{
    /// <summary>
    /// 可序列化的键值对集合基类
    /// 继承自该类的集合支持Unity序列化，内部通过Dictionary保证查找性能，通过List存储键值对用于序列化
    /// </summary>
    /// <typeparam name="TKey">键的类型</typeparam>
    /// <typeparam name="TValue">值的类型</typeparam>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Collection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, ISerializationCallbackReceiver, ICollection<TKey, TValue>
    {
        // 核心键值映射容器，用于高效的键值对查找、添加、删除操作
        [JsonProperty] protected readonly Dictionary<TKey, TValue> keyToValueMap = new();
        // 序列化用的键列表，与values列表一一对应
        [SerializeField] private List<TKey> keys = new();
        // 序列化用的值列表，与keys列表一一对应
        [SerializeField] private List<TValue> values = new();

        /// <summary>
        /// 索引器，通过键快速获取对应的值
        /// </summary>
        /// <param name="key">要查找的键</param>
        /// <exception cref="KeyNotFoundException">当指定的键不存在时抛出</exception>
        /// <returns>键对应的值</returns>
        public TValue this[TKey key] { get => keyToValueMap[key]; set => keyToValueMap[key] = value; }

        /// <summary>
        /// 获取集合中键值对的数量
        /// </summary>
        public int Count => keyToValueMap.Count;

        public Dictionary<TKey,TValue>.KeyCollection Keys => keyToValueMap.Keys;
        
        public Dictionary<TKey, TValue>.ValueCollection Values => keyToValueMap.Values;

        /// <summary>
        /// 检查集合中是否包含指定的键
        /// </summary>
        /// <param name="key">要检查的键</param>
        /// <returns>存在返回true，不存在返回false</returns>
        public virtual bool ContainsKey(TKey key)
        {
            return keyToValueMap.ContainsKey(key);
        }

        /// <summary>
        /// 尝试向集合中添加键值对
        /// </summary>
        /// <param name="key">要添加的键</param>
        /// <param name="value">要添加的值</param>
        /// <returns>添加成功返回true；若键已存在，打印日志并返回false</returns>
        public virtual bool TryAdd(TKey key, TValue value)
        {
            if (keyToValueMap.TryAdd(key, value))
            {
                return true;
            }

            Logger.Log($"已存在键{key}，值为{value}，添加失败");
            return false;
        }

        /// <summary>
        /// 从集合中移除指定键对应的键值对
        /// </summary>
        /// <param name="key">要移除的键</param>
        /// <returns>移除成功返回true；键不存在返回false</returns>
        public virtual bool Remove(TKey key)
        {
            return keyToValueMap.Remove(key);
        }

        /// <summary>
        /// 尝试获取指定键对应的值
        /// </summary>
        /// <param name="key">要查找的键</param>
        /// <param name="value">输出参数，找到则为对应值，未找到则为类型默认值</param>
        /// <returns>找到键返回true，未找到返回false</returns>
        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            if (keyToValueMap.TryGetValue(key, out var findValue))
            {
                value = findValue;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 清空集合中所有的键值对
        /// </summary>
        public virtual void Clear()
        {
            keyToValueMap.Clear();
        }

        /// <summary>
        /// 获取集合的枚举器，用于遍历所有键值对
        /// </summary>
        /// <returns>键值对枚举器</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var pair in keyToValueMap)
            {
                yield return pair;
            }
        }

        /// <summary>
        /// 非泛型枚举器实现，调用泛型版本
        /// </summary>
        /// <returns>非泛型枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Unity序列化前的回调方法
        /// 将Dictionary中的键值对分别存入keys和values列表，用于序列化
        /// </summary>
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var pair in keyToValueMap)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        /// <summary>
        /// Unity反序列化后的回调方法
        /// 将序列化的keys和values列表重新填充到Dictionary中
        /// </summary>
        public void OnAfterDeserialize()
        {
            keyToValueMap.Clear();
            // 取键列表和值列表的最小长度，避免索引越界
            var count = Mathf.Min(keys.Count, values.Count);
            for (var i = 0; i < count; i++)
            {
                keyToValueMap.TryAdd(keys[i], values[i]);
            }

            // 键列表和值列表长度不一致时输出错误日志
            if (keys.Count != values.Count)
            {
                Debug.LogError($"{nameof(keys)}与{nameof(values)}长度不匹配，已取最小长度进行反序列化");
            }
        }
    }
}
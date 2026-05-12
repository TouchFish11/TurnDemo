using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Collection
{
    /// <summary>
    /// 序列化字典
    /// </summary>
    /// <typeparam name="TKey">泛型Key</typeparam>
    /// <typeparam name="TValue">泛型Value</typeparam>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class SerializableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, ISerializationCallbackReceiver
    {
        // 核心键值映射容器，用于高效的键值对查找、添加、删除操作
        [JsonProperty] private readonly Dictionary<TKey, TValue> keyToValueMap = new();
        // 序列化用的键列表，与values列表一一对应
        [SerializeField] private List<TKey> keys = new();
        // 序列化用的值列表，与keys列表一一对应
        [SerializeField] private List<TValue> values = new();
        
        /// <summary>
        /// 获取集合中键值对的数量
        /// </summary>
        public int Count => keyToValueMap.Count;
        
        public Dictionary<TKey,TValue>.KeyCollection Keys => keyToValueMap.Keys;
        
        public Dictionary<TKey, TValue>.ValueCollection Values => keyToValueMap.Values;
        
        /// <summary>
        /// 索引器，通过键快速获取对应的值
        /// </summary>
        /// <param name="key">要查找的键</param>
        /// <exception cref="KeyNotFoundException">当指定的键不存在时抛出</exception>
        /// <returns>键对应的值</returns>
        public TValue this[TKey key] { get => keyToValueMap[key]; set => keyToValueMap[key] = value; }
        
        /// <summary>
        /// 检查集合中是否包含指定的键
        /// </summary>
        /// <param name="key">要检查的键</param>
        /// <returns>存在返回true，不存在返回false</returns>
        public bool ContainsKey(TKey key)
        {
            return keyToValueMap.ContainsKey(key);
        }

        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="key">要添加的键</param>
        /// <param name="value">要添加的值</param>
        public void Add(TKey key, TValue value)
        {
            keyToValueMap.Add(key, value);
        }
        
        /// <summary>
        /// 尝试向集合中添加键值对
        /// </summary>
        /// <param name="key">要添加的键</param>
        /// <param name="value">要添加的值</param>
        /// <returns>添加成功返回true；若键已存在，打印日志并返回false</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            return keyToValueMap.TryAdd(key, value);
        }
        
        /// <summary>
        /// 从集合中移除指定键对应的键值对
        /// </summary>
        /// <param name="key">要移除的键</param>
        /// <returns>移除成功返回true；键不存在返回false</returns>
        public bool Remove(TKey key)
        {
            return keyToValueMap.Remove(key);
        }
        
        /// <summary>
        /// 尝试获取指定键对应的值
        /// </summary>
        /// <param name="key">要查找的键</param>
        /// <param name="value">输出参数，找到则为对应值，未找到则为类型默认值</param>
        /// <returns>找到键返回true，未找到返回false</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (keyToValueMap.TryGetValue(key, out value))
            {
                return true;
            }

            value = default;
            return false;
        }
        
        /// <summary>
        /// 清空集合中所有的键值对
        /// </summary>
        public void Clear()
        {
            keyToValueMap.Clear();
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var pair in keyToValueMap)
            {
                yield return pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            
            foreach (var pair in keyToValueMap)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            keyToValueMap.Clear();
            // 取键列表和值列表的最小长度，避免索引越界
            var count = Mathf.Min(keys.Count, values.Count);
            for (var i = 0; i < count; i++)
            {
                keyToValueMap.TryAdd(keys[i], values[i]);
            }
        }
    }
}

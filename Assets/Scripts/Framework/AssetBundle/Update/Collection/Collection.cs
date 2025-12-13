using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 集合类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public abstract class Collection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, ISerializationCallbackReceiver
    {
        // 键到值的映射
        protected readonly Dictionary<TKey, TValue> keyToValueMap = new Dictionary<TKey, TValue>();
        [SerializeField]
        private List<TKey> keys = new List<TKey>();
        [SerializeField]
        private List<TValue> values = new List<TValue>();

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key] => keyToValueMap[key];

        /// <summary>
        /// 集合元素数量
        /// </summary>
        public int Count => keyToValueMap.Count;

        /// <summary>
        /// 是否包含键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return keyToValueMap.ContainsKey(key);
        }

        /// <summary>
        /// 尝试添加信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAdd(TKey key, TValue value)
        {
            if (keyToValueMap.TryAdd(key, value))
            {
                return true;
            }
            else
            {
                LogManager.Log($"已存在键：{key}，值：{value}");
                return false;
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            return keyToValueMap.Remove(key);
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (keyToValueMap.TryGetValue(key, out TValue findValue))
            {
                value = findValue;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            keyToValueMap.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, TValue> pair in keyToValueMap)
            {
                yield return pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in keyToValueMap)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            keyToValueMap.Clear();
            int count = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < count; i++)
            {
                keyToValueMap.Add(keys[i], values[i]);
            }

            if (keys.Count != values.Count)
            {
                Debug.LogError($"{nameof(keys)}和{nameof(values)}长度不匹配，已取较小长度");
            }
        }
    }
}

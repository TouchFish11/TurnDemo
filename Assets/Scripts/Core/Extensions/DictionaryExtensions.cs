using System;
using System.Collections.Generic;
using Core.Collection;

namespace Core.Extensions
{
    /// <summary>
    /// 字典拓展类
    /// </summary>
    public static class DictionaryExtensions
    {
        public static TReturn[] ToArray<TKey, TValue, TReturn>(this Dictionary<TKey,TValue>.ValueCollection valueCollection, Func<TValue, TReturn> func)
        {
            var uniList = CollectionUtil.GetUniList<TReturn>();
            foreach (var value in valueCollection)
            {
                uniList.List.Add(func(value));
            }
            
            return uniList.List.ToArray();
        }
        
        public static TReturn[] ToArray<TKey, TValue, TReturn>(this Dictionary<TKey,TValue>.KeyCollection keyCollection, Func<TKey, TReturn> func)
        {
            var uniList = CollectionUtil.GetUniList<TReturn>();
            foreach (var value in keyCollection)
            {
                uniList.List.Add(func(value));
            }
            
            return uniList.List.ToArray();
        }

        /// <summary>
        /// 轮询所有元素是否满足条件
        /// func返回值等于方法返回值，检查集合中的所有元素是否满足func的自定义逻辑，可自定义func的满足规则
        /// 外部可通过方法返回值循环判断，决定是否继续判断下去
        /// </summary>
        /// <param name="valueCollection"></param>
        /// <param name="func"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IEnumerable<bool> MeetConditions<TKey, TValue>(this Dictionary<TKey,TValue>.ValueCollection valueCollection, Func<TValue, bool> func)
        {
            var values = valueCollection.ToArray(value => value);
            foreach (var tValue in values)
            {
                yield return func.Invoke(tValue);
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Core.Extensions
{
    /// <summary>
    /// 字典拓展类
    /// </summary>
    public static class DictionaryExtensions
    {
        public static TReturn[] ToArray<TKey, TValue, TReturn>(this Dictionary<TKey,TValue>.ValueCollection valueCollection, Func<TValue, TReturn> func)
        {
            var list = new List<TReturn>(valueCollection.Count);
            foreach (var value in valueCollection)
            {
                list.Add(func(value));
            }
            
            return list.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using Core.Pool;

namespace Core.Collection.Generic
{
    /// <summary>
    /// 通用列表
    /// </summary>
    public class UniList<T> : IPoolData
    {
        public List<T> List { get; } = new();
        
        public T this[int index] => List[index];

        public void Add(T item)
        {
            List.Add(item);
        }

        public UniList<T> AddRange(IEnumerable<T> collection)
        {
            List.AddRange(collection);
            return this;
        }
        
        public bool Remove(T item)
        {
            return List.Remove(item);
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }
        
        public void Clear()
        {
            List.Clear();
        }

        public void Sort(Comparison<T> comparison = null)
        {
            if (comparison == null)
            {
                List.Sort();
            }
            else
            {
                List.Sort(comparison);
            }
        }
        
        public void ResetData()
        {
            List.Clear();
        }
    }
}

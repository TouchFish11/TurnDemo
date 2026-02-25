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

        public void ResetData()
        {
            List.Clear();
        }
    }
}

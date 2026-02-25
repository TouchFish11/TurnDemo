using Core.Collection.Generic;
using Core.Pool;
using Core.Service;

namespace Core.Collection
{
    /// <summary>
    /// 集合工具类
    /// </summary>
    public static class CollectionUtil
    {
        public static UniList<T> GetUniList<T>()
        {
            return ServiceLocator.Get<IPoolManager>().GetData<UniList<T>>();
        }
    }
}

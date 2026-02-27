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
        /// <summary>
        /// 获取可复用的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static UniList<T> GetUniList<T>()
        {
            return ServiceLocator.Get<IPoolManager>().GetData<UniList<T>>();
        }

        /// <summary>
        /// 缓存可复用的List
        /// </summary>
        /// <param name="uniList"></param>
        /// <typeparam name="T"></typeparam>
        public static void CollectUniList<T>(UniList<T> uniList)
        {
            ServiceLocator.Get<IPoolManager>().PushData(uniList);
        }
    }
}

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Core.Singleton
{
    /// <summary>
    /// 单例基类
    /// </summary>
    public abstract class SingletonBase<T> : IInitializable where T : class
    {
        // 单例对象
        private static volatile T _instance;
        // 锁引用对象
        private static readonly object LockObj = new();

        /// <summary>
        /// 单例对象
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                lock (LockObj)
                {
                    if (_instance != null)
                    {
                        return _instance;
                    }
                    var type = typeof(T);
                    var info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                    if (info != null)
                    {
                        _instance = info.Invoke(null) as T;
                    }
                    else
                    {
                        throw new Exception($"{typeof(T).Name}没有实现私有无参构造函数");
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 单例是否存在
        /// </summary>
        public bool IsLive => _instance != null;

        public abstract int Priority { get; }
        
        public abstract Task InitAsync();
    }
}

using System;
using System.Reflection;

namespace Framework
{
    /// <summary>
    /// 单例基类
    /// </summary>
    public abstract class SingletonBase<T> where T : class
    {
        private static T _instance;

        //锁引用对象
        private static readonly object _lockObj = new object();

        /// <summary>
        /// 单例对象
        /// </summary>
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock (_lockObj)
                    {
                        if(_instance == null)
                        {
                            Type type = typeof(T);
                            ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                            if (info != null)
                                _instance = info.Invoke(null) as T;
                            else
                                throw new Exception($"没有实现私有无参构造函数, 缺失类：{typeof(T).Name}");
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 单例是否存在
        /// </summary>
        public bool IsLive
        {
            get
            {
                if (_instance == null)
                    return false;
                else
                    return true;
            }
        }
    }
}

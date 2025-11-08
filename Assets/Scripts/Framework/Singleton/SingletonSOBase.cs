using UnityEngine;

namespace Framework
{
    /// <summary>
    /// ScriptableObject单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonSOBase<T> : ScriptableObject where T : ScriptableObject
    {
        //锁引用
        private static readonly object _lock = new object();

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = Resources.Load<ScriptableObject>(typeof(T).Name) as T;
                            if (instance != null)
                                return instance;
                            else
                            {
                                //创建ScriptableObject实例
                                instance = ScriptableObject.CreateInstance<T>();
                                Debug.Log($"没有在Resources文件夹中找到{typeof(T).Name}，已创建新的实例");
                            }
                        }
                    }
                }
                return instance;
            }
        }
    }
}

using UnityEngine;

namespace Core.Singleton
{
    /// <summary>
    /// ScriptableObject单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonSOBase<T> : UnityEngine.ScriptableObject where T : UnityEngine.ScriptableObject
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
                            instance = Resources.Load<UnityEngine.ScriptableObject>($"Global/{typeof(T).Name}") as T;
                            if (instance != null)
                            {
                                return instance;
                            }
                            else
                            {
                                //创建ScriptableObject实例
                                instance = CreateInstance<T>();
                                Debug.Log($"没有在Resources/Global文件夹中找到{typeof(T).Name}，已创建新的实例");
                            }
                        }
                    }
                }
                return instance;
            }
        }
    }
}

using Core.Log;
using Core.Utility;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.Singleton
{
    /// <summary>
    /// ScriptableObject单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonSOBase<T> : ScriptableObject where T : ScriptableObject
    {
        //锁引用
        private static readonly object _lock = new();

        private static T instance;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    lock (_lock)
                    {
                        if (!instance)
                        {
                            var loadPath = PathUtility.GetGlobalSettingsPath($"{typeof(T).Name}");
                            instance = Resources.Load<ScriptableObject>(loadPath) as T;
                            if (instance)
                                return instance;

                            // 创建ScriptableObject实例
                            instance = CreateInstance<T>();
                            Logger.LogDebug(ELogTags.SO, $"{typeof(T).Name} was not found in {loadPath}. A new instance has been created.");
                        }
                    }
                }
                return instance;
            }
        }
    }
}

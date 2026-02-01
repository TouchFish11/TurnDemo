using UnityEngine;

namespace Core.Singleton
{
    /// <summary>
    /// 继承Mono单例(自动生成)
    /// </summary>
    public abstract class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        /// <summary>
        /// 单例实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject autoSingletoObj = new GameObject(typeof(T).Name);
                    _instance = autoSingletoObj.AddComponent<T>();
                    DontDestroyOnLoad(autoSingletoObj);
                }
                return _instance;
            }
        }

        /// <summary>
        /// 单例是否存在
        /// </summary>
        public static bool IsLIve => _instance != null;

        protected virtual void OnDestroy()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }
            _instance = null;
        }
    }
}

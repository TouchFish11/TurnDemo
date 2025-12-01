using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 继承Mono单例(自动生成)
    /// </summary>
    public abstract class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
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

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}

using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 继承Mono的单例(手动挂载)
    /// </summary>
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance => _instance;

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

        protected virtual void Awake()
        {
            //已经存在该单例对象，为了避免在切换场景时重复创建
            if (_instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}

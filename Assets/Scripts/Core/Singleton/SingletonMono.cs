using UnityEngine;

namespace Core.Singleton
{
    /// <summary>
    /// �̳�Mono�ĵ���(�ֶ�����)
    /// </summary>
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance => _instance;

        /// <summary>
        /// �����Ƿ����
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
            //�Ѿ����ڸõ�������Ϊ�˱������л�����ʱ�ظ�����
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}

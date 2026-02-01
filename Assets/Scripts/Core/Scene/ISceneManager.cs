using System;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core.Scene
{
    /// <summary>
    /// �����������ӿ�
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        /// �����첽����
        /// </summary>
        /// <param name="scenePath">����·��</param>
        /// <param name="mode">����ģʽ</param>
        /// <param name="onLoadProgress"></param>
        /// <param name="completed">�����ص�</param>
        void LoadSceneAsync(string scenePath, LoadSceneMode mode, Action<float> onLoadProgress, Func<Task> completed);
    }
}

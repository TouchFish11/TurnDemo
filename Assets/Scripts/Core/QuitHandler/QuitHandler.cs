using System;
using System.Threading.Tasks;
using Core.Log;
using Core.Singleton;
using UnityEngine;

namespace Core.QuitHandler
{
    /// <summary>
    /// 退出处理器
    /// </summary>
    public class QuitHandler : SingletonAutoMono<QuitHandler>, IQuitHandler
    {
        /// <summary>
        /// 应用程序退出事件
        /// </summary>
        public event Func<Task> OnAppQuit;

        /// <summary>
        /// 激活退出处理器
        /// </summary>
        public void ActiveHandler()
        {
            LogManager.Instance.EnableLog = true;
            LogManager.Log(Application.persistentDataPath);
            LogManager.Log($"退出处理器已激活");
        }

        private async void OnApplicationQuit()
        {
            try
            {
                await OnAppQuit?.Invoke();
                OnAppQuit = null;
            }
            catch (Exception e)
            {
                LogManager.LogException(e);
            }
        }
    }
}

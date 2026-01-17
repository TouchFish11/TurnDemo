using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Framework.Mock
{
    /// <summary>
    /// 模拟场景管理器
    /// </summary>
    public class MockSceneManager : SingletonBase<MockSceneManager>, ISceneManager
    {
        private MockSceneManager()
        {

        }

        public async void LoadSceneAsync(string scenePath, LoadSceneMode mode, UnityAction<float> onLoadProgress, Func<Task> completed)
        {
            // 异步加载场景
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, mode);
            // 禁止自动激活场景
            ao.allowSceneActivation = false;
            float currentProgress = 0;
            while (ao.progress < 0.9f)
            {
                currentProgress = ao.progress / 0.9f;
                onLoadProgress?.Invoke(Mathf.Clamp01(currentProgress));
                await Task.Yield();
            }
            // 此时进度已到0.9f，先将进度回调置为1.0f，给用户完整的进度反馈
            onLoadProgress?.Invoke(1.0f);
            // 激活场景
            ao.allowSceneActivation = true;

            while (!ao.isDone)
            {
                await Task.Yield();
            }

            // 执行加载完成事件
            await completed?.Invoke();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Framework
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public class SceneManager : SingletonBase<SceneManager>
    {
        // 当前加载进度
        private float currentProgress = 0;

        private SceneManager()
        {

        }

        /// <summary>
        /// 场景异步加载
        /// </summary>
        /// <param name="scenePath">场景路径</param>
        /// <param name="mode">加载模式</param>
        /// <param name="overCallBack">结束回调</param>
        public async void LoadSceneAsync(string scenePath, LoadSceneMode mode, UnityAction<float> onLoadProgress, Func<Task> completed)
        {
            if (!AssetBundleManager.Instance.ContainPath(scenePath))
            {
                LogManager.LogError($"不存在该场景：{scenePath}");
                return;
            }

            // 加载场景所需的AB包
            if (!await AssetBundleManager.Instance.LoadSceneBundleAsync())
            {
                LogManager.Log($"场景加载失败：{scenePath}");
                return;
            }

            // 异步加载场景
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, mode);
            // 禁止自动激活场景
            ao.allowSceneActivation = false;
            // 记录上一次加载的进度
            float lastProgress = 0;
            while (lastProgress != ao.progress || ao.progress != 0.9f)
            {
                lastProgress = ao.progress - lastProgress;
                currentProgress += lastProgress / 0.9f;
                onLoadProgress?.Invoke(currentProgress);
                await Task.Yield();
            }

            // 执行加载完成事件
            await completed?.Invoke();

            // 激活场景
            ao.allowSceneActivation = true;
        }
    }
}

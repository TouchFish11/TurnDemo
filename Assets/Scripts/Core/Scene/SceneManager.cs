using System;
using System.Collections;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Log;
using Core.Mono;
using Core.Service;
using Core.Singleton;
using Core.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core.Scene
{
    /// <summary>
    /// 场景管理类，负责场景的异步加载，继承单例基类并实现ISceneManager接口
    /// </summary>
    public class SceneManager : SingletonBase<SceneManager>, ISceneManager
    {
        /// <summary>
        /// 私有构造函数，确保单例模式下无法外部实例化
        /// </summary>
        private SceneManager()
        {

        }

        /// <summary>
        /// 异步加载场景的方法
        /// </summary>
        /// <param name="scenePath">要加载的场景路径</param>
        /// <param name="mode">场景加载模式（叠加/替换）</param>
        /// <param name="onLoadProgress">加载进度回调，参数为0~1的进度值</param>
        /// <param name="completed">加载完成后执行的异步委托</param>
        public async void LoadSceneAsync(string scenePath, LoadSceneMode mode, [CanBeNull] Action<float> onLoadProgress, [CanBeNull] Func<Task> completed)
        {
            // 加载场景对应的AssetBundle资源包
            if (!await ServiceLocator.Get<IAssetBundleManager>().LoadSceneBundleAsync())
            {
                LogManager.LogError($"场景资源包加载失败：{scenePath}");
                return;
            }

            // 检查AssetBundle中是否包含指定路径的场景
            if (!ServiceLocator.Get<IAssetBundleManager>().ContainPath(scenePath))
            {
                LogManager.LogError($"AssetBundle中不存在该场景路径：{scenePath}");
                return;
            }
            
            // 异步加载场景（Unity原生接口）
            var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, mode);
            // 开启更新进度协程
            ServiceLocator.Get<IMonoManager>().StartCoroutine(UpdateProgress_Cor(ao, onLoadProgress));
            // 等待场景加载结束
            await TaskUtility.WaitUntil(() => ao.isDone);
            // 执行场景加载完成后的异步回调逻辑
            await completed?.Invoke();
            
            try
            {

            }
            catch (Exception e)
            {
                LogManager.LogError($"{typeof(SceneManager).FullName}.{nameof(LoadSceneAsync)}：{e.Message}");
            }
        }

        /// <summary>
        /// 更新进度协程
        /// </summary>
        /// <param name="ao"></param>
        /// <param name="onLoadProgress"></param>
        /// <returns></returns>
        private static IEnumerator UpdateProgress_Cor(AsyncOperation ao, Action<float> onLoadProgress)
        {
            // 禁止场景加载完成后自动激活，用于精准控制加载进度展示
            ao.allowSceneActivation = false;
            // 循环监听加载进度，Unity的LoadSceneAsync进度在完成前最大为0.9f
            while (ao.progress < 0.9f)
            {
                // 将0~0.9的进度值转换为0~1的进度比例，便于外部统一处理
                var currentProgress = ao.progress / 0.9f;
                // 回调当前加载进度，确保进度值在0~1的范围内
                onLoadProgress?.Invoke(Mathf.Clamp01(currentProgress));
                // 让出当前帧执行权，等待下一帧继续检测进度，避免阻塞主线程
                yield return null;
            }
            
            // 进度达到0.9f时，强制将进度回调为1.0f，告知外部加载完成（剩余0.1为激活场景的过程）
            onLoadProgress?.Invoke(1.0f);
            // 允许场景激活，完成最终的场景加载流程
            ao.allowSceneActivation = true;

            // 等待场景激活完成（isDone变为true）
            while (!ao.isDone)
            {
                yield return null;
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Dependence;
using Core.Log;
using Core.Mono;
using Core.Service;
using Core.Singleton;
using Core.Utility;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scene
{
    /// <summary>
    /// 场景管理类，负责场景的异步加载，继承单例基类并实现ISceneManager接口
    /// </summary>
    public class SceneManager : SingletonBase<SceneManager>, ISceneManager, IDependable
    {
        // 场景路径缓存
        private List<string> _scenePaths;

        private SceneManager()
        {
            ServiceLocator.Get<IDependencyManager>().RegisterDependable(typeof(IAssetBundleManager), this);
        }
        
        public async Task OnDependcyInited()
        {
            // 初始化场景包
            await InitSceneBundle();
        }
        
        public async void LoadSceneAsync(string scenePath, LoadSceneMode mode, [CanBeNull] Action<float> onLoadProgress, [CanBeNull] Func<Task> completed)
        {
            try
            {
                // 检查是否包含指定路径的场景
                if (!ContainPath(scenePath))
                {
                    LogManager.LogError($"不存在该场景路径：{scenePath}");
                    return;
                }
                
                // 异步加载场景
                var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, mode);
                // 开启更新进度协程
                ServiceLocator.Get<IMonoAdapter>().StartCoroutine(UpdateProgress_Cor(ao, onLoadProgress));
                // 等待场景加载结束
                await TaskUtility.WaitUntil(() => ao != null && ao.isDone);
                // 执行场景加载完成后的异步回调逻辑
                await completed?.Invoke();
                
            }
            catch (Exception exception)
            {
                LogManager.LogError($"{typeof(SceneManager).FullName}.{nameof(LoadSceneAsync)}：{exception.Message}");
                LogManager.LogError($"场景资源包加载失败：{scenePath}");
            }
        }

        private async Task InitSceneBundle()
        {
            // 缓存所有场景名称
            if (_scenePaths == null)
            {
                // 加载场景对应的AssetBundle资源包
                var sceneBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(EAssetBundleType.Scene);
                _scenePaths = new List<string>();
                foreach (var scenePath in sceneBundle.GetAllScenePaths())
                {
                    var sceneNames = scenePath.Split('/');
                    var sceneName = sceneNames[sceneNames.Length - 1];
                    _scenePaths.Add(sceneName.Substring(0, sceneName.LastIndexOf('.')));
                }
            }
            else
            {
                LogManager.LogError($"{nameof(SceneManager)}.{nameof(InitSceneBundle)}；重复初始化");
            }
        }

        /// <summary>
        /// 是否包含该场景路径
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private bool ContainPath(string sceneName)
        {
            return _scenePaths.Contains(sceneName);
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
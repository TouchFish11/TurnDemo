using System.Collections;
using System.Collections.Generic;
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
        //缓存场景路径列表
        private readonly List<string> _scenePaths = new List<string>();
        /// <summary>
        /// AB包加载权重
        /// </summary>
        private const float Weight_ABLoad = 0.5f;
        /// <summary>
        /// 场景加载权重
        /// </summary>
        private const float Weight_SceneLoad = 0.5f;
        //当前加载进度
        private float currentProgress = 0;

        private SceneManager()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            AssetBundleLoadManager.Instance.GetAllScenePaths((paths) =>
            {
                if (_scenePaths.Count == 0)
                {
                    _scenePaths.AddRange(paths);
                }
            });
        }

        /// <summary>
        /// 场景异步加载
        /// </summary>
        /// <param name="scenePath">场景路径</param>
        /// <param name="mode">加载模式</param>
        /// <param name="overCallBack">结束回调</param>
        public void LoadSceneAsync(string scenePath, LoadSceneMode mode, UnityAction<float> onLoadProgress, UnityAction onLoadComplete)
        {
            if (!_scenePaths.Contains(scenePath))
            {
                LogMgr.LogError($"不存在该场景：{scenePath}");
            }
            else
            {
                MonoManager.Instance.StartCoroutine(LoadSceneAsync_Cor());
            }

            //处理自定义增量和自定义逻辑的协程
            IEnumerator LoadSceneAsync_Cor()
            {
                //异步加载所有相关AB包
                yield return AssetBundleLoadManager.Instance.LoadSceneAssetBundleAsync(E_AssetBundleType.Scene, (abProgress) =>
                {
                    onLoadProgress?.Invoke(currentProgress += abProgress * Weight_ABLoad);
                });

                //异步加载场景
                AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenePath, mode);
                //禁止自动激活场景
                ao.allowSceneActivation = false;
                //记录上一次加载的进度
                float lastProgress = 0;
                while (lastProgress != ao.progress || ao.progress != 0.9f)
                {
                    lastProgress = ao.progress - lastProgress;
                    currentProgress += lastProgress / 0.9f * Weight_SceneLoad;
                    onLoadProgress?.Invoke(currentProgress);
                    yield return null;
                }

                //激活场景
                ao.allowSceneActivation = true;
                //执行回调
                onLoadComplete?.Invoke();
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            _scenePaths.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using Core.Log;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 场景包包装器
    /// </summary>
    public class SceneBundleWrapper : BundleWrapper
    {
        // 场景名称列表
        private readonly List<string> _sceneNames = new List<string>();

        public SceneBundleWrapper(string abName, string path) : base(abName, path)
        {

        }

        /// <summary>
        /// 是否包含指定场景路径
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public bool ContainPath(string sceneName)
        {
            if (assetBundle == null)
            {
                LogManager.LogError($"获取场景路径失败，场景包：{bundelName}未加载");
                return false;
            }

            // 缓存场景名
            if (_sceneNames.Count == 0)
            {
                CacheSceneNames();
            }

            return _sceneNames.Contains(sceneName);
        }

        /// <summary>
        /// 获取所有场景路径
        /// </summary>
        /// <returns></returns>
        public string[] GetAllScenePaths()
        {
            if (assetBundle != null)
            {
                return assetBundle.GetAllScenePaths();
            }
            
            LogManager.LogError($"获取场景路径失败，场景包：{bundelName}未加载");
            return Array.Empty<string>();
        }

        /// <summary>
        /// 缓存场景名
        /// </summary>
        private void CacheSceneNames()
        {
            var scenePaths = assetBundle.GetAllScenePaths();
            foreach (var scenePath in scenePaths)
            {
                var sceneNames = scenePath.Split('/');
                var sceneName = sceneNames[sceneNames.Length - 1];
                _sceneNames.Add(sceneName.Substring(0, sceneName.LastIndexOf('.')));
            }
        }

        /// <summary>
        /// 场景包引用计数
        /// </summary>
        public override uint RefCount => 1;
    }
}

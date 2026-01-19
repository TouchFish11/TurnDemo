using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 场景包包装器
    /// </summary>
    public class SceneBundleWrapper : BundleWrapper
    {
        // 场景名称列表
        private List<string> sceneNames = new List<string>();

        public SceneBundleWrapper(string abName, string path) : base(abName, path)
        {

        }

        /// <summary>
        /// 是否包含指定场景路径
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public bool ContainPath(string sceneName)
        {
            if (assetBundle == null)
            {
                LogManager.LogError($"获取场景路径失败，场景包：{bundelName}未加载");
                return false;
            }

            // 缓存场景名
            if (sceneNames.Count == 0)
            {
                CacheSceneNames();
            }

            if (sceneNames.Contains(sceneName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取所有场景路径
        /// </summary>
        /// <returns></returns>
        public string[] GetAllScenePaths()
        {
            if (assetBundle == null)
            {
                LogManager.LogError($"获取场景路径失败，场景包：{bundelName}未加载");
                return new string[0];
            }

            return assetBundle.GetAllScenePaths();
        }

        /// <summary>
        /// 缓存场景名
        /// </summary>
        private void CacheSceneNames()
        {
            var scenePaths = assetBundle.GetAllScenePaths();
            foreach (var scenePath in scenePaths)
            {
                string[] strs = scenePath.Split('/');
                string sceneName = strs[strs.Length - 1];
                sceneNames.Add(sceneName.Substring(0, sceneName.LastIndexOf('.')));
            }
        }

        /// <summary>
        /// 场景包引用计数
        /// </summary>
        public override uint RefCount => 1;
    }
}

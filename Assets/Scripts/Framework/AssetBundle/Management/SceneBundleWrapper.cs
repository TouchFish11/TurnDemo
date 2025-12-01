using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 场景包包装器
    /// </summary>
    public class SceneBundleWrapper : BundleWrapper
    {
        public SceneBundleWrapper(string abName, string path) : base(abName, path)
        {

        }

        /// <summary>
        /// 是否包含指定场景路径
        /// </summary>
        /// <param name="scenePath"></param>
        /// <returns></returns>
        public bool ContainPath(string scenePath)
        {
            if (assetBundle == null)
            {
                LogMgr.LogError($"获取场景路径失败，场景包：{bundelName}未加载");
                return false;
            }

            // 获取所有场景路径
            List<string> paths = new List<string>(assetBundle.GetAllScenePaths());
            if (paths.Contains(scenePath))
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
                LogMgr.LogError($"获取场景路径失败，场景包：{bundelName}未加载");
                return new string[0];
            }

            return assetBundle.GetAllScenePaths();
        }

        /// <summary>
        /// 场景包引用计数
        /// </summary>
        public override uint RefCount => 1;
    }
}


namespace Framework
{
    /// <summary>
    /// 场景包加载器
    /// </summary>
    public class SceneBundleLoader : BundleLoader
    {
        public SceneBundleLoader(string abName, string path) : base(abName, path)
        {

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
        /// 场景引用数
        /// </summary>
        public override uint RefCount => 1;
    }
}

using System.IO;
using Core.Utility;
using UnityEditor;

namespace Core.Editor.AssetBundle.Core
{
    /// <summary>
    /// AB包工具类
    /// </summary>
    public static class AssetBundleUtility
    {
        /// <summary>
        /// 获取当前平台对应的主包名（如 StandaloneWindows64.assetBundle）
        /// </summary>
        public static string GetPlatformBundleName(BuildTarget target)
        {
            return target.ToString().WithAbSuffix();
        }

        /// <summary>
        /// 确保目录存在，不存在则创建
        /// </summary>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 清空目录下的所有文件
        /// </summary>
        public static void ClearDirectory(string path)
        {
            if (!Directory.Exists(path)) return;
            var dirInfo = new DirectoryInfo(path);
            foreach (var file in dirInfo.GetFiles())
                file.Delete();
        }
    }
}

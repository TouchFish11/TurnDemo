using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Utility
{
    /// <summary>
    /// 文件工具类
    /// </summary>
    public static class FileUtility
    {
        #region 默认文件

        /// <summary>
        /// 本地日志文件名
        /// </summary>
        /// <value>
        /// app_Log.txt
        /// </value>
        public static string LocalLogFileName => "app_Log.txt";

        /// <summary>
        /// 本地音乐数据文件名
        /// </summary>
        /// <value>
        /// MusicData.bytes
        /// </value>
        public static string LocalMusicDataFileName => "MusicData.bytes";

        /// <summary>
        /// 本地输入数据文件名
        /// </summary>
        /// <value>
        /// InputData.bytes
        /// </value>
        public static string LocalInputDataFileName => "InputData.bytes";

        /// <summary>
        /// AB包清单文件默认名称
        /// </summary>
        /// <value>
        /// AssetBundleListInfo.json
        /// </value>
        public static string ListFileDefaultName => "AssetBundleListInfo.json";

        /// <summary>
        /// AB包临时清单文件默认名称
        /// </summary>
        /// <value>
        /// ABListInfo_Temp.json
        /// </value>
        public static string TempListFileDefaultName => "ABListInfo_Temp.json";

        /// <summary>
        /// AB包缓存文件默认名称
        /// </summary>
        /// <value>
        /// ABCacheFile.json
        /// </value>
        public static string CacheDefaultName => "ABCacheFile.json";

        /// <summary>
        /// 本地输入动作配置文件名
        /// </summary>
        /// <value>
        /// PlayerActionAssets.json
        /// </value>
        public static string InputActionLocalFileName => "MainActionMap.json";

        /// <summary>
        /// AB包自定义后缀
        /// </summary>
        /// <value>
        /// .assetbundle
        /// </value>
        public static string AbSuffix => ".assetbundle";
        #endregion

        /// <summary>
        /// 本地登录数据缓存文件名
        /// </summary>
        /// <value>
        /// LoginCacheData.bytes
        /// </value>
        public static string LocalLoginDataFileName => "LoginCacheData.bytes";
        
        /// <summary>
        /// 任务系统数据缓存文件名称
        /// </summary>
        /// <value>
        /// TaskData.json
        /// </value>
        public static string LocalTaskDataFileName => "TaskData.json";
        
        /// <summary>
        /// 活动系统数据缓存文件名称
        /// </summary>
        /// <value>
        /// ActivityData.json
        /// </value>
        public static string LocalActivityDataFileName => "ActivityData.json";

        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="fileInfos"></param>
        /// <param name="filterSuffixes"></param>
        /// <returns></returns>
        public static List<FileInfo> GetTotalFiles(DirectoryInfo directoryInfo, List<FileInfo> fileInfos, string[] filterSuffixes)
        {
            //获取并存储当前文件夹的所有文件
            var temps = directoryInfo.GetFiles().ToList();
            for (var i = temps.Count - 1; i >= 0; i--)
            {
                if (filterSuffixes.Contains(temps[i].Extension))
                {
                    temps.RemoveAt(i);
                }
            }

            fileInfos.AddRange(temps);
            //获取下一级的所有子文件夹
            var subDirectoryInfos = directoryInfo.GetDirectories();
            //存储该级的所有子文件夹信息
            foreach (var info in subDirectoryInfos)
            {
                GetTotalFiles(info, fileInfos, filterSuffixes);
            }
            return fileInfos;
        }
    }
}

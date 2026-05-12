using System.IO;
using Core.Global;
using UnityEngine;
using static Core.Global.GlobalSettings;

namespace Core.Utility
{
    /// <summary>
    /// 路径工具类
    /// 提供游戏中各类文件/目录的路径管理、路径拼接、目录创建等功能
    /// 封装Unity不同平台下的持久化路径、流式资源路径等，统一对外提供路径访问入口
    /// </summary>
    public static class PathUtility
    {
        // 基础路径字段 - 不同平台通用
        /// <summary>
        /// 持久化数据路径
        /// 对应Unity的Application.persistentDataPath，不同平台路径不同，用于存储用户可读写的游戏数据
        /// </summary>
        private static readonly string _persistentPath;

        /// <summary>
        /// 流式资源路径
        /// 对应Unity的Application.streamingAssetsPath，用于存储打包资源
        /// </summary>
        private static readonly string _streamingAssetsPath;

        /// <summary>
        /// 应用程序数据路径
        /// 对应Unity的Application.dataPath，指向游戏安装目录的Assets文件夹，不同平台访问权限不同
        /// </summary>
        private static readonly string _dataPath;

        /// <summary>
        /// 用户数据本地存储根路径
        /// 根据全局配置决定使用StreamingAssets或PersistentPath
        /// </summary>
        public static string UserDataLocalSavePath => GlobalSettings.Instance.userDataPath == EDataLoadPath.Streaming ? Path.Combine(_streamingAssetsPath, "UserData") : Path.Combine(_persistentPath, "UserData");

        /// <summary>
        /// 日志文件本地存储根路径
        /// 固定指向持久化路径下的Log目录，确保日志可写入
        /// </summary>
        public static string LogLocalSavePath { get; }

        /// <summary>
        /// 游戏表格数据本地加载根路径
        /// 固定指向持久化路径下的GameData目录，用于加载/存储游戏配置表
        /// </summary>
        public static string TableInfoLocalLoadPath { get; }

        /// <summary>
        /// AssetBundle资源加载根路径
        /// 根据全局配置决定使用StreamingAssets或PersistentPath
        /// </summary>
        public static string LoadAbPath => GlobalSettings.Instance.abLoadPath == EDataLoadPath.Streaming ? Path.Combine(_streamingAssetsPath, "AssetBundles") : Path.Combine(_persistentPath, "AssetBundles");
        
        /// <summary>
        /// Json文件运行时加载路径
        /// 指向持久化路径下的Json目录，运行时读取/写入Json配置
        /// </summary>
        public static string JsonRuntimeLoadPath { get; }
        
        /// <summary>
        /// 全局配置SO加载路径
        /// </summary>
        public static string GlobalSettingsPath { get; }

        /// <summary>
        /// 静态构造函数
        /// 初始化各类基础路径和业务路径，自动创建所需目录（确保目录存在）
        /// 静态构造函数在类第一次被访问时执行，且仅执行一次
        /// </summary>
        static PathUtility()
        {
            // 初始化Unity基础路径
            _persistentPath = Application.persistentDataPath;
            _streamingAssetsPath = Application.streamingAssetsPath;
            _dataPath = Application.dataPath;

            // 初始化各业务模块的根路径
            LogLocalSavePath = Path.Combine(_persistentPath, "Log"); // 日志目录
            TableInfoLocalLoadPath = Path.Combine(_persistentPath, "GameData"); // 表格数据目录
            JsonRuntimeLoadPath = Path.Combine(_persistentPath, "Json"); // 运行时Json目录
            GlobalSettingsPath = Path.Combine("Global");
            
            // 自动创建所有核心业务目录（不存在则创建）
            CreateDirectory(UserDataLocalSavePath);
            CreateDirectory(LogLocalSavePath);
            CreateDirectory(TableInfoLocalLoadPath);
            CreateDirectory(LoadAbPath);
            CreateDirectory(JsonRuntimeLoadPath);
            CreateDirectory(GlobalSettingsPath);
        }

        /// <summary>
        /// 获取用户数据文件的完整存储路径
        /// </summary>
        /// <param name="fileName">文件名，可从FileUtility中获取</param>
        /// <returns>拼接后的用户数据文件完整路径</returns>
        public static string GetUserDataLocalSavePath(string fileName)
        {
            return Path.Combine(UserDataLocalSavePath, fileName);
        }

        /// <summary>
        /// 获取日志文件的完整存储路径
        /// </summary>
        /// <param name="fileName">日志文件名，可从FileUtility中获取</param>
        /// <returns>拼接后的日志文件完整路径</returns>
        public static string GetLogLocalSavePath(string fileName)
        {
            return Path.Combine(LogLocalSavePath, fileName);
        }

        /// <summary>
        /// 获取游戏表格数据文件的完整加载路径
        /// </summary>
        /// <param name="fileName">表格文件名，可从FileUtility中获取</param>
        /// <returns>拼接后的表格数据文件完整路径</returns>
        public static string GetTableInfoLocalLoadPath(string fileName)
        {
            return Path.Combine(TableInfoLocalLoadPath, fileName);
        }

        /// <summary>
        /// 获取AssetBundle资源文件的完整加载路径
        /// </summary>
        /// <param name="fileName">AB包名（包含扩展名）</param>
        /// <returns>拼接后的AB包完整加载路径</returns>
        public static string GetAbLoadPath(string fileName)
        {
            return Path.Combine(LoadAbPath, fileName);
        }

        /// <summary>
        /// 获取全局配置SO对象加载路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetGlobalSettingsPath(string fileName)
        {
            return Path.Combine(GlobalSettingsPath, fileName);
        }

        /// <summary>
        /// 获取运行时Json文件的完整加载路径
        /// </summary>
        /// <param name="fileName">Json文件名（包含扩展名，如config_runtime.json）</param>
        /// <returns>拼接后的运行时Json文件完整路径</returns>
        public static string GetJsonRuntimeLoadPath(string fileName)
        {
            return Path.Combine(JsonRuntimeLoadPath, fileName);
        }

        /// <summary>
        /// 检测并创建目录（若目录不存在则创建）
        /// 避免因目录不存在导致文件读写失败
        /// </summary>
        /// <param name="path">需要创建的目录路径</param>
        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
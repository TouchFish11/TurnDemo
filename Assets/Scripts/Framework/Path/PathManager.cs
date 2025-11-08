using System.IO;
using UnityEngine;

/// <summary>
/// 路径工具类
/// </summary>
public static class PathManager
{
    // 基础路径（各平台通用）
    private static readonly string _persistentPath; // 持久化数据路径（可读写）
    private static readonly string _streamingAssetsPath; //  StreamingAssets路径（只读）
    private static readonly string _dataPath; // 应用程序数据路径（通常只读）

    /// <summary>
    /// 用户数据本地存储路径
    /// </summary>
    public static string UserDataLocalSavePath { get; private set; }

    /// <summary>
    /// 日志本地存储路径
    /// </summary>
    public static string LogLocalSavePath { get; private set; }

    /// <summary>
    /// 二进制数据本地加载路径
    /// </summary>
    public static string TableInfoLocalLoadPath { get; private set; }

    /// <summary>
    /// 本地AB包加载路径
    /// 与存储路径一致
    /// </summary>
    public static string LoadAbPath { get; private set; }

    /// <summary>
    /// 本地AB加载测试路径
    /// </summary>
    public static string AbTestLoadPath { get; private set; }

    /// <summary>
    /// Json文件的加载路径（调试路径）
    /// </summary>
    public static string JsonDebugLoadPath { get; private set; }

    /// <summary>
    /// Json文件运行时加载路径
    /// </summary>
    public static string JsonRuntimeLoadPath { get; private set; }

    static PathManager()
    {
        // 初始化基础路径（仅在类加载时执行一次）
        _persistentPath = Application.persistentDataPath;
        _streamingAssetsPath = Application.streamingAssetsPath;
        _dataPath = Application.dataPath;

        // 初始化功能路径（确保目录存在）
        UserDataLocalSavePath = Path.Combine(_persistentPath, "UserData");
        LogLocalSavePath = Path.Combine(_persistentPath, "Log");
        TableInfoLocalLoadPath = Path.Combine(_persistentPath, "GameData");
        LoadAbPath = Path.Combine(_persistentPath, "AssetBundle");
        AbTestLoadPath = Path.Combine(_streamingAssetsPath);
        JsonDebugLoadPath = Path.Combine(_dataPath, "Editor", "ArtRes", "GameData", "Json");
        JsonRuntimeLoadPath = Path.Combine(_persistentPath, "Json");

        // 创建所有必要的目录（避免运行时因目录不存在报错）
        CreateDirectory(UserDataLocalSavePath);
        CreateDirectory(LogLocalSavePath);
        CreateDirectory(TableInfoLocalLoadPath);
        CreateDirectory(LoadAbPath);
        CreateDirectory(AbTestLoadPath);
        CreateDirectory(JsonDebugLoadPath);
        CreateDirectory(JsonRuntimeLoadPath);
    }

    /// <summary>
    /// 获取用户数据本地存储路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetUserDataLocalSavePath(string fileName)
    {
        return Path.Combine(UserDataLocalSavePath, fileName);
    }

    /// <summary>
    /// 获取日志本地存储路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetLogLocalSavePath(string fileName)
    {
        return Path.Combine(LogLocalSavePath, fileName);
    }

    /// <summary>
    /// 获取二进制数据本地加载路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetTableInfoLocalLoadPath(string fileName)
    {
        return Path.Combine(TableInfoLocalLoadPath, fileName);
    }

    /// <summary>
    /// 获取本地AB包加载路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetAbLoadPath(string fileName)
    {
        return Path.Combine(LoadAbPath, fileName);
    }

    /// <summary>
    /// 获取本地AB加载测试路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetAbTestLoadPath(string fileName)
    {
        return Path.Combine(AbTestLoadPath, fileName);
    }

    /// <summary>
    /// 获取Json文件的加载路径（调试路径）
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetJsonDebugLoadPath(string fileName)
    {
        return Path.Combine(JsonDebugLoadPath, fileName);
    }

    /// <summary>
    /// 获取Json文件运行时加载路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetJsonRuntimeLoadPath(string fileName)
    {
        return Path.Combine(JsonRuntimeLoadPath, fileName);
    }

    /// <summary>
    /// 创建文件夹（若不存在）
    /// </summary>
    /// <param name="path"></param>
    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}

using CustomEditor.ScriptGeneration;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器工具类
/// </summary>
public class EditorTools
{
    /// <summary>
    /// SO资源存储路径
    /// </summary>
    public const string AssetPath = "Assets/Resources/Global/";

    /// <summary>
    /// 创建全局配置文件
    /// </summary>
    [MenuItem("GameTool/SO Config/Create GlobalSettings")]
    public static void CreateGlobalSetting()
    {
        //不存在则创建
        if (!Directory.Exists(AssetPath))
        {
            Directory.CreateDirectory(AssetPath);
        }

        //创建ScriptableObject实例
        GlobalSettings settings = ScriptableObject.CreateInstance<GlobalSettings>();
        //创建配置文件
        AssetDatabase.CreateAsset(settings, AssetPath + "GlobalSettings.asset");
        //保存文件
        AssetDatabase.SaveAssets();
        //刷新
        AssetDatabase.Refresh();

        Debug.Log($"已创建GlobalSetting，路径：{AssetPath}{nameof(GlobalSettings)}");
    }

    /// <summary>
    /// 生成ResKeyCollection脚本
    /// </summary>
    [MenuItem("GameTool/Generate/Generate ResKeyCollection")]
    public static void GenerateResKeyCollectionScript()
    {
        ResKeyCollectionClassGenerator resKeyCollectionClassGenerator = new ResKeyCollectionClassGenerator();
        resKeyCollectionClassGenerator.GenerateScript();
    }
}

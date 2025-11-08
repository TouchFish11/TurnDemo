/// <remarks>
/// 作者: Qiu
/// 创建时间: 2025-04-27
/// 修改时间: 2025-05-11
/// </remarks>



using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Lua脚本拷贝至文本
/// </summary>
public sealed class LuaCopy2txt : Editor
{
    /// <summary>
    /// lua文件在编辑器的存储文件夹
    /// </summary>
    private static readonly string LUA_EDITOR_SAVEPATH = Application.dataPath + "/Editor/Lua/";

    /// <summary>
    /// lua转文本后缀之后在编辑器的存储文件夹
    /// </summary>
    private static readonly string LUA2TXT_EDITOR_SAVEPATH = Application.dataPath + "/Editor/ArtRes/Lua/";

    [MenuItem("GameTool/Lua/Lua2txt")]
    public static void CopyLuaToTxt()
    {
        //判断路径是否存在
        if (!Directory.Exists(LUA_EDITOR_SAVEPATH))
        {
            Directory.CreateDirectory(LUA_EDITOR_SAVEPATH);
            Debug.Log($"lua文件存储位置：{LUA_EDITOR_SAVEPATH}");
            AssetDatabase.Refresh();
            return;
        }

        //拷贝lua文件到新文件夹中
        //只获取后缀为.lua的文件
        string[] luaFileNames = Directory.GetFiles(LUA_EDITOR_SAVEPATH, "*.lua");
        //判断路径是否存在，不存在则自动创建
        if (!Directory.Exists(LUA2TXT_EDITOR_SAVEPATH))
        {
            Directory.CreateDirectory(LUA2TXT_EDITOR_SAVEPATH);
            Debug.Log($"lua文本文件存储位置：{LUA2TXT_EDITOR_SAVEPATH}");
            AssetDatabase.Refresh();
        }
        //存在则删除原有文件
        else
        {
            string[] oldFiles = Directory.GetFiles(LUA2TXT_EDITOR_SAVEPATH, "*.txt");
            for (int i = 0; i < oldFiles.Length; i++)
            {
                File.Delete(oldFiles[i]);
            }
        }

        string newFileName;
        //拷贝文件
        for (int i = 0; i < luaFileNames.Length; i++)
        {
            newFileName = LUA2TXT_EDITOR_SAVEPATH + luaFileNames[i][(luaFileNames[i].LastIndexOf('/') + 1)..] + ".txt";
            File.Copy(luaFileNames[i], newFileName);
        }

        //刷新
        AssetDatabase.Refresh();
    }
}

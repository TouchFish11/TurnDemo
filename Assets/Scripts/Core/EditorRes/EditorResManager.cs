using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Log;
using Core.Singleton;
using Core.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Core.EditorRes
{
    /// <summary>
    /// 编辑器资源管理器
    /// </summary>
    public class EditorResManager : SingletonBase<EditorResManager>, IEditorResManager
    {
        public override int Priority => 0;

        /// <summary>
        /// 编辑器资源根目录
        /// </summary>
        private const string RootPath = "Assets/Editor/ArtRes/";
        // 文件信息列表
        private List<FileInfo> _fileInfoList = new();

        private EditorResManager(){}

        public override Task InitAsync()
        {
            return Task.CompletedTask;
        }

        public T LoadEditorAsset<T>(string assetName, string suffixName = "") where T : Object
        {
#if UNITY_EDITOR
            // 文件夹不存在
            if (!Directory.Exists(RootPath))
            {
                LogManager.Log($"路径不存在:{RootPath}");
                return null;
            }

            // 获取路径下的所有文件
            var directoryInfo = Directory.CreateDirectory(RootPath);
            // 没有文件缓存
            if (_fileInfoList.Count == 0)
            {
                // 递归获取所有文件
                _fileInfoList = FileUtility.GetTotalFiles(directoryInfo, new List<FileInfo>(), new[] { ".meta" });
            }

            // 根据类型自动添加后缀
            suffixName = suffixName switch
            {
                "" when typeof(T) == typeof(GameObject) => ".prefab",
                "" when typeof(T) == typeof(Material) => ".mat",
                "" when typeof(T) == typeof(Texture) => ".png",
                "" when typeof(T) == typeof(AudioClip) => ".mp3",
                "" when typeof(T) == typeof(TextAsset) => ".txt",
                "" when typeof(T) == typeof(Sprite) => ".png",
                "" when typeof(T) == typeof(SpriteAtlas) => ".spriteatlasv2",
                "None" => "",
                _ => suffixName
            };
            
            // 根据名称匹配对应文件
            var targetInfo = _fileInfoList.Find(fileInfo => fileInfo.Name == $"{assetName}{suffixName}");
            if (targetInfo == null)
            {
                LogManager.LogError($"未找到该资源:{assetName}{suffixName}");
                return null;
            }

            // 加载资源
            var res = AssetDatabase.LoadAssetAtPath<T>(targetInfo.FullName[targetInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)..]);
            if (res != null)
            {
                return res;
            }
            LogManager.LogError($"不存在该文件路径:{targetInfo.FullName[targetInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)..]}");
            return null;
#else
            LogManager.LogError("发布环境不允许使用编辑器API");
            return null;
#endif
        }
    }
}

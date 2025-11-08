using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace Framework
{
    /// <summary>
    /// 编辑器资源管理器
    /// </summary>
    public class EditorResMgr : SingletonBase<EditorResMgr>
    {
        /// <summary>
        /// 编辑器下的资源路径
        /// </summary>
        private const string RootPath = "Assets/Editor/ArtRes/";

        //存储所有文件信息的列表
        private readonly List<FileInfo> _fileInfoList = new List<FileInfo>();

        private EditorResMgr() { }

        /// <summary>
        /// 加载编辑器资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">资源名</param>
        /// <param name="suffixName">资源后缀</param>
        /// <returns>资源</returns>
        public T LoadEditorAsset<T>(string assetName, string suffixName = "") where T : Object
        {
#if UNITY_EDITOR
            //该文件夹路径不存在
            if (!Directory.Exists(RootPath))
            {
                LogMgr.Log($"该路径{RootPath}不存在");
                return null;
            }

            //获取根文件夹信息
            DirectoryInfo directoryInfo = Directory.CreateDirectory(RootPath);
            //第一次调用才去获取所有文件
            if (_fileInfoList.Count == 0)
            {
                //获取根文件夹下的所有子文件夹
                GetAllFiles(directoryInfo);
            }

            //后缀名
            if (suffixName == "")
            {
                if (typeof(T) == typeof(GameObject))
                    suffixName = ".prefab";
                else if (typeof(T) == typeof(Material))
                    suffixName = ".mat";
                else if (typeof(T) == typeof(Texture))
                    suffixName = ".png";
                else if (typeof(T) == typeof(AudioClip))
                    suffixName = ".mp3";
                else if (typeof(T) == typeof(TextAsset))
                    suffixName = ".txt";
                else if (typeof(T) == typeof(Sprite))
                    suffixName = ".png";
                else if (typeof(T) == typeof(SpriteAtlas))
                    suffixName = ".spriteatlasv2";
            }

            FileInfo targetInfo = null;
            //遍历所有文件
            for (int i = 0; i < _fileInfoList.Count; i++)
            {
                if (_fileInfoList[i].Name == $"{assetName}{suffixName}")
                {
                    targetInfo = _fileInfoList[i];
                    break;
                }
            }

            if (targetInfo == null)
            {
                LogMgr.LogError($"未找到该资源，{assetName}{suffixName}");
                return null;
            }

            T res = AssetDatabase.LoadAssetAtPath<T>(targetInfo.FullName[targetInfo.FullName.IndexOf("Assets")..]);
            if (res == null)
            {
                LogMgr.LogError($"编辑器资源加载失败，资源路径：{targetInfo.FullName[targetInfo.FullName.IndexOf("Assets")..]}");
                return null;
            }
            return res;
#else
            LogMgr.LogError("发布模式不允许使用编辑器下的资源加载方法");
            return null;
#endif
        }

        /// <summary>
        /// 加载图集图片资源
        /// </summary>
        /// <param name="spritesName">图集名</param>
        /// <param name="spriteName">图片名</param>
        /// <returns>图片</returns>
        public Sprite LoadSprite(string spritesName, string spriteName)
        {
#if UNITY_EDITOR
            //该文件夹路径不存在，则自动创建
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
                LogMgr.Log($"该路径{RootPath}不存在，已自动创建");
                AssetDatabase.Refresh();
            }

            //获取根文件夹信息
            DirectoryInfo directoryInfo = Directory.CreateDirectory(RootPath);
            if (_fileInfoList.Count == 0)
                //获取根文件夹下的所有子文件夹
                GetAllFiles(directoryInfo);

            FileInfo targetInfo = null;
            //遍历所有文件夹
            for (int i = 0; i < _fileInfoList.Count; i++)
            {
                if (_fileInfoList[i].Name == $"{spritesName}{".spriteatlasv2"}")
                {
                    targetInfo = _fileInfoList[i];
                    break;
                }
            }

            if (targetInfo == null)
            {
                LogMgr.LogError($"未找到该资源，{spritesName}{".spriteatlasv2"}");
                return null;
            }

            Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(targetInfo.FullName[targetInfo.FullName.IndexOf("Assets")..]);
            foreach (Object obj in sprites)
            {
                if (spriteName == obj.name)
                    return obj as Sprite;
            }
            return null;
#else
            return null;
#endif
        }

        /// <summary>
        /// 加载图集所有子资源
        /// </summary>
        /// <param name="spritesName">图集名</param>
        /// <returns>键：图片名，值：图片</returns>
        public Dictionary<string, Sprite> LoadAllSprite(string spritesName)
        {
#if UNITY_EDITOR
            //该文件夹路径不存在，则自动创建
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
                LogMgr.Log($"该路径{RootPath}不存在，已自动创建");
                AssetDatabase.Refresh();
            }

            //获取根文件夹信息
            DirectoryInfo directoryInfo = Directory.CreateDirectory(RootPath);
            if (_fileInfoList.Count == 0)
                //获取根文件夹下的所有子文件夹
                GetAllFiles(directoryInfo);

            FileInfo targetInfo = null;
            //遍历所有文件夹
            for (int i = 0; i < _fileInfoList.Count; i++)
            {
                if (_fileInfoList[i].Name == $"{spritesName}{".spriteatlasv2"}")
                {
                    targetInfo = _fileInfoList[i];
                    break;
                }
            }

            if (targetInfo == null)
            {
                LogMgr.LogError($"未找到该资源，{spritesName}{".spriteatlasv2"}");
                return null;
            }

            Dictionary<string, Sprite> spritesDic = new Dictionary<string, Sprite>();
            Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(targetInfo.FullName[targetInfo.FullName.IndexOf("Assets")..]);
            foreach (Object obj in sprites)
            {
                spritesDic.Add(obj.name, obj as Sprite);
            }
            return spritesDic;
#else
            return null;
#endif
        }

        /// <summary>
        /// 获取根文件夹下的的所有文件
        /// </summary>
        /// <param name="directoryInfo">文件夹信息</param>
        private void GetAllFiles(DirectoryInfo directoryInfo)
        {
            List<FileInfo> fileInfos = new List<FileInfo>(directoryInfo.GetFiles());
            for (int i = fileInfos.Count - 1; i >= 0; --i)
            {
                if (fileInfos[i].Extension == ".meta")
                {
                    fileInfos.RemoveAt(i);
                }
            }
            _fileInfoList.AddRange(fileInfos);
            //获取下一级的所有子文件夹
            DirectoryInfo[] subDirectoryInfos = directoryInfo.GetDirectories();
            
            //存储该级的所有子文件夹信息
            foreach (DirectoryInfo info in subDirectoryInfos)
            {
                GetAllFiles(info);
            }
        }
    }
}

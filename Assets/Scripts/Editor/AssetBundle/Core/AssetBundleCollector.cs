using System;
using System.Collections.Generic;
using System.IO;
using Core.AssetBundles.Management;
using Core.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace Editor.AssetBundle.Core
{
    /// <summary>
    /// 负责资源收集、标签设置与清除
    /// </summary>
    public class AssetBundleCollector
    {
        private readonly string assetsInputPath;
        private readonly string[] filterSuffixes;
        private readonly string[] filterDirectories;
        private readonly Action<string> logAction;
        private readonly Action<string, float> progressAction;

        public AssetBundleCollector(string inputPath, string[] suffixes, string[] directories, 
            Action<string> logAction = null, Action<string, float> progressAction = null)
        {
            assetsInputPath = inputPath;
            filterSuffixes = suffixes;
            filterDirectories = directories;
            this.logAction = logAction;
            this.progressAction = progressAction;
        }

        private void Log(string msg) => logAction?.Invoke(msg);
        private void Progress(string msg, float val) => progressAction?.Invoke(msg, val);

        /// <summary>
        /// 收集最新资源信息，生成临时的 AssetBundlesCollections 文件
        /// </summary>
        public AssetBundlesCollections CollectLatestInfos(string savePath, string tempFileName)
        {
            Log("--- Starting Collect AssetInfos ---");
            var startTime = DateTime.Now;

            var collection = AssetDatabase.LoadAssetAtPath<AssetBundlesCollections>($"{savePath}{tempFileName}");
            if (!collection)
                collection = CreateCollection(savePath, tempFileName);
            else
                collection.Clear();

            if (!Directory.Exists(assetsInputPath))
            {
                Log($"资源输入路径不存在：{assetsInputPath}，已自动创建，请放入待打包资源后重试");
                Directory.CreateDirectory(assetsInputPath);
                Log("--- End Collect AssetInfos ---\n");
                return collection;
            }

            var directoryInfo = new DirectoryInfo(assetsInputPath);
            var fileInfoDic = new Dictionary<string, List<FileInfo>>();

            // 获取子目录
            foreach (var dir in directoryInfo.GetDirectories())
            {
                if (filterDirectories != null && Array.Exists(filterDirectories, d => d == dir.Name))
                    continue;
                var files = FileUtility.GetTotalFiles(dir, new List<FileInfo>(), filterSuffixes);
                fileInfoDic.Add(dir.Name, files);
            }

            var total = 0;
            foreach (var list in fileInfoDic.Values) total += list.Count;
            var index = 0;

            foreach (var abName in fileInfoDic.Keys)
            {
                foreach (var fileInfo in fileInfoDic[abName])
                {
                    var dataPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal));
                    Progress($"Collecting Path：{dataPath}", (float)index++ / (total - 1));

                    EAssetType assetType;
                    var type = AssetDatabase.GetMainAssetTypeAtPath(dataPath);
                    if (type == typeof(SpriteAtlas))
                    {
                        assetType = EAssetType.SpiteAtlas;
                    }
                    else if (type == typeof(SceneAsset))
                    {
                        assetType = EAssetType.Scene;
                    }
                    else
                    {
                        assetType = EAssetType.Object;
                    }
                    
                    var assetInfo = new AssetBundlesCollections.AssetInfo(
                        dataPath,
                        fileInfo.Length,
                        fileInfo.Name,
                        HashUtility.GenerateFileSHA256Hash(fileInfo.FullName),
                        assetType
                    );
                    collection.Add(abName.ToLower(), assetInfo);
                }
            }

            EditorUtility.ClearProgressBar();

            Log($"--- Took Seconds：{(DateTime.Now - startTime).TotalSeconds:F2}s ---");
            Log($"Collect AssetBundle Count：{collection.assetBundleInfos.Count}");
            var assetCount = 0;
            foreach (var info in collection.assetBundleInfos) assetCount += info.assetInfos.Count;
            Log($"--- Collect Asset Count：{assetCount} ---");
            Log("--- End Collect AssetInfos ---\n");

            return collection;
        }

        /// <summary>
        /// 为资源设置 AssetBundle 标签（根据差异字典和发布配置）
        /// </summary>
        public void SetLabels(Dictionary<string, List<AssetBundlesCollections.AssetInfo>> abNameToDifferenceInfos, AssetBundlesCollections releaseCollection, List<string> waitRemoveAbNames)
        {
            if (abNameToDifferenceInfos.Count == 0 && waitRemoveAbNames.Count == 0)
            {
                Log("No Differences\n");
                return;
            }

            // 待移除包的标签将在后续循环中自动清空
            if (waitRemoveAbNames.Count > 0)
                Log($"Exist Will Remove AssetBundle Labels：[{string.Join('、', waitRemoveAbNames)}].\n");

            // 全量设置标签的情况
            if (!releaseCollection || abNameToDifferenceInfos.Count == releaseCollection.assetBundleInfos.Count)
            {
                var total = 0;
                foreach (var list in abNameToDifferenceInfos.Values) total += list.Count;
                var index = 0;
                foreach (var abName in abNameToDifferenceInfos.Keys)
                {
                    foreach (var assetInfo in abNameToDifferenceInfos[abName])
                    {
                        Progress($"handing File: {assetInfo.name}", (float)index++ / (total - 1));
                        var importer = AssetImporter.GetAtPath(assetInfo.assetPath);
                        if (importer) 
                            importer.assetBundleName = abName.ToLower();
                        else 
                            Log($"Setting Label error: {assetInfo.assetPath}");
                    }
                }
                Log($"Setting Lables：[{string.Join('、', abNameToDifferenceInfos.Keys)}]\n");
                EditorUtility.ClearProgressBar();
                return;
            }

            // 增量设置标签
            var tempDic = new Dictionary<string, List<AssetBundlesCollections.AssetInfo>>();
            foreach (var abInfo in releaseCollection.assetBundleInfos)
            {
                if (abNameToDifferenceInfos.ContainsKey(abInfo.assetBundleName))
                    tempDic.Add(abInfo.assetBundleName, abInfo.assetInfos);
            }

            var total2 = 0;
            foreach (var list in tempDic.Values) total2 += list.Count;
            var index2 = 0;

            // 先处理变化/待移除的包的资源
            foreach (var abInfo in releaseCollection.assetBundleInfos)
            {
                if (abNameToDifferenceInfos.ContainsKey(abInfo.assetBundleName))
                {
                    foreach (var assetInfo in abNameToDifferenceInfos[abInfo.assetBundleName])
                    {
                        Progress($"handing File: {assetInfo.name}", (float)index2++ / (total2 - 1));
                        var importer = AssetImporter.GetAtPath(assetInfo.assetPath);
                        if (importer) 
                            importer.assetBundleName = abInfo.assetBundleName.ToLower();
                        else 
                            Log($"Setting Label error: {assetInfo.assetPath}");
                    }
                }
                else
                {
                    foreach (var assetInfo in abInfo.assetInfos)
                    {
                        var importer = AssetImporter.GetAtPath(assetInfo.assetPath);
                        if (importer && importer.assetBundleName != "")
                            importer.assetBundleName = "";
                    }
                }
            }

            // 再处理新增包的资源
            foreach (var abName in abNameToDifferenceInfos.Keys)
            {
                if (!releaseCollection.assetBundleInfos.Exists(info => info.assetBundleName == abName))
                {
                    foreach (var assetInfo in abNameToDifferenceInfos[abName])
                    {
                        var importer = AssetImporter.GetAtPath(assetInfo.assetPath);
                        if (importer) importer.assetBundleName = abName;
                    }
                }
            }

            Log($"Setting Lables：[{string.Join(',', abNameToDifferenceInfos.Keys)}]");
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 清除输入目录下所有资源的 AssetBundle 标签
        /// </summary>
        public void ClearAllLabels()
        {
            if (!Directory.Exists(assetsInputPath))
            {
                Log($"资源输入路径不存在：{assetsInputPath}，已自动创建，请放入待打包资源后重试");
                Directory.CreateDirectory(assetsInputPath);
                return;
            }

            Log("--- Starting Clear All Asset Label ---");
            var directoryInfo = new DirectoryInfo(assetsInputPath);
            var fileInfoDic = new Dictionary<string, List<FileInfo>>();

            foreach (var dir in directoryInfo.GetDirectories())
            {
                var files = FileUtility.GetTotalFiles(dir, new List<FileInfo>(), filterSuffixes);
                fileInfoDic.Add(dir.Name, files);
            }

            foreach (var abName in fileInfoDic.Keys)
            {
                foreach (var fileInfo in fileInfoDic[abName])
                {
                    var dataPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal));
                    var importer = AssetImporter.GetAtPath(dataPath);
                    if (importer && importer.assetBundleName != "")
                        importer.assetBundleName = "";
                }
            }
            Log("--- End Clear Asset Label ---\n");
        }

        private AssetBundlesCollections CreateCollection(string savePath, string fileName)
        {
            AssetBundleUtility.EnsureDirectoryExists(savePath);
            var collection = ScriptableObject.CreateInstance<AssetBundlesCollections>();
            AssetDatabase.CreateAsset(collection, $"{savePath}{fileName}");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Log($"Config ScriptableObject be created at：\n{savePath}{fileName}");
            return collection;
        }
    }
}
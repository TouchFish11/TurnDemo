using System;
using System.Collections.Generic;
using System.IO;
using Core.AssetBundles.Collection;
using Core.AssetBundles.Management;
using Core.Serialize.Json;
using Core.Utility;
using Editor.AssetBundle;
using UnityEditor;
using UnityEngine;

namespace Core.Editor.AssetBundle.Core
{
    /// <summary>
    /// 依赖分析与清单文件生成
    /// </summary>
    public class AssetBundleDependencyResolver
    {
        private readonly Action<string> logAction;
        private readonly Action<string, float> progressAction;

        public AssetBundleDependencyResolver(Action<string> logAction = null, Action<string, float> progressAction = null)
        {
            this.logAction = logAction;
            this.progressAction = progressAction;
        }

        private void Log(string msg) => logAction?.Invoke(msg);
        private void Progress(string msg, float val) => progressAction?.Invoke(msg, val);

        /// <summary>
        /// 分析主包中所有包的依赖关系（打印到日志）
        /// </summary>
        public void AnalyzeDependencies(string mainBundlePath, BuildTarget target)
        {
            if (string.IsNullOrEmpty(mainBundlePath) || !File.Exists(mainBundlePath))
            {
                Log("Invalid Main AssetBundle file path.\n");
                return;
            }

            Log("--- Analyzing Dependencies ---");
            Log($"Loading AssetBundle from: {mainBundlePath}");

            UnityEngine.AssetBundle mainBundle = null;
            try
            {
                mainBundle = UnityEngine.AssetBundle.LoadFromFile(mainBundlePath);
                if (!mainBundle)
                {
                    Log($"Failed to load main AssetBundle from: {mainBundlePath}");
                    return;
                }

                var manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (!manifest)
                {
                    Log("Failed to load AssetBundleManifest object from the bundle.");
                    return;
                }

                var allNames = manifest.GetAllAssetBundles();
                Log($"Found {allNames.Length} bundles in manifest：");
                foreach (var name in allNames)
                {
                    var deps = manifest.GetAllDependencies(name);
                    Log($"Bundle '{name}' depends on: [{string.Join(", ", deps)}]");
                }
            }
            catch (Exception e)
            {
                Log($"Analysis error: {e.Message}\n{e.StackTrace}");
            }
            finally
            {
                if (mainBundle) mainBundle.Unload(false);
            }
            Log("--- Analysis End ---\n");
        }

        /// <summary>
        /// 生成 AB 包清单文件（JSON格式）
        /// </summary>
        [Obsolete("仅保留，不在使用",true)]
        public void CreateListFile(string outputPath, string listFilePath, string mainBundlePath, BuildTarget target, JsonManager jsonManager)
        {
            AssetBundleUtility.EnsureDirectoryExists(outputPath);

            Log("---Start Create List File---");
            UnityEngine.AssetBundle mainBundle = null;
            try
            {
                mainBundle = UnityEngine.AssetBundle.LoadFromFile(mainBundlePath);
                if (!mainBundle)
                {
                    Log($"Failed to load main AssetBundle from: {mainBundlePath}");
                    return;
                }

                var manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (!manifest)
                {
                    Log("Failed to load AssetBundleManifest object from the bundle.");
                    return;
                }

                var dirInfo = new DirectoryInfo(outputPath);
                var fileInfos = new List<FileInfo>(dirInfo.GetFiles());
                var collection = new ABPackageCollection();
                int index = 0;

                foreach (var fileInfo in fileInfos)
                {
                    if (fileInfo.Extension != FileUtility.AbSuffix || fileInfo.Name.Contains(target.ToString()))
                        continue;

                    Progress($"Handing :{fileInfo.Name} dependencies...", (float)index++ / (fileInfos.Count - 2));

                    string bundleName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    var deps = manifest.GetAllDependencies(bundleName);
                    var pkgInfo = new ABPackageInfo(fileInfo.Name, fileInfo.Length, 
                        HashUtility.GenerateFileSHA256Hash(fileInfo.FullName), deps);
                    collection.TryAdd(fileInfo.Name, pkgInfo);
                }

                EditorUtility.ClearProgressBar();

                var assetCatalog = new AssetCatalog();
                jsonManager.SaveToJson(collection, listFilePath);
                AssetDatabase.Refresh();
                Log($"AssetBundle List File Created : {listFilePath}");
            }
            catch (Exception e)
            {
                Log($"Create error: {e.Message}\n{e.StackTrace}");
            }
            finally
            {
                if (mainBundle) mainBundle.Unload(false);
                UnityEngine.AssetBundle.UnloadAllAssetBundles(true);
                Log("---End Create List File---\n");
            }
        }

        /// <summary>
        /// 从 manifest 文件中获取指定包的所有反向依赖（即哪些包依赖了 targetBundle）
        /// </summary>
        public List<string> GetReverseDependencies(string manifestPath, string targetBundle)
        {
            var result = new List<string>();
            if (!File.Exists(manifestPath)) return result;

            UnityEngine.AssetBundle mainBundle = null;
            try
            {
                mainBundle = UnityEngine.AssetBundle.LoadFromFile(manifestPath);
                if (!mainBundle) return result;

                var manifest = mainBundle.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
                if (!manifest) return result;
                
                var allBundles = manifest.GetAllAssetBundles();
                foreach (var bundle in allBundles)
                {
                    var deps = manifest.GetDirectDependencies(bundle);
                    if (Array.Exists(deps, d => d == targetBundle))
                        result.Add(bundle);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"GetReverseDependencies error: {e.Message}");
            }
            finally
            {
                if (mainBundle) mainBundle.Unload(false);
            }
            return result;
        }

        /// <summary>
        /// 扩展差异结果：将因依赖变化而受影响的上层包加入重打包列表
        /// </summary>
        public void ExpandWithDependencies(Dictionary<string, List<AssetBundlesCollections.AssetInfo>> bundlesToRebuild,
            List<string> bundlesToRemove,
            AssetBundlesCollections releaseCollection,
            string manifestPath,
            HashSet<string> forceUploadBundles) // 新增参数
        {
            if (!File.Exists(manifestPath)) return;

            // 存储所有变化的包，重建的包和移除的包都算变化的包
            var changedBundles = new HashSet<string>(bundlesToRebuild.Keys);
            foreach (var name in bundlesToRemove) changedBundles.Add(name);

            // 受影响的包集合
            var affected = new HashSet<string>();
            // 转换为队列，执行BFS
            var queue = new Queue<string>(changedBundles);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                // 获取当前包的所有反向依赖
                var reverseDeps = GetReverseDependencies(manifestPath, current);
                // 遍历所有反向依赖
                foreach (var dep in reverseDeps)
                {
                    // 若不是变化的包，且没有缓存过，则首次缓存
                    if (!changedBundles.Contains(dep) && !affected.Contains(dep))
                    {
                        affected.Add(dep);
                        // 放入队列，找到这个依赖的反向依赖（依赖反向传播）
                        queue.Enqueue(dep);
                    }
                }
            }

            foreach (var bundleName in affected)
            {
                var releaseAbInfo = releaseCollection?.assetBundleInfos.Find(info => info.assetBundleName == bundleName);
                if (releaseAbInfo != null && !bundlesToRebuild.ContainsKey(bundleName))
                {
                    bundlesToRebuild.Add(bundleName, new List<AssetBundlesCollections.AssetInfo>(releaseAbInfo.assetInfos));
                    forceUploadBundles.Add(bundleName); // 标记为强制上传
                    Log($"---由于依赖变化，额外标记重打包并强制上传：{bundleName}---");
                }
            }
        }
    }
}
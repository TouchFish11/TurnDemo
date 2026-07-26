using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.AssetBundles.Collection;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Json;
using Core.Utility;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Editor.AssetBundle.Core
{
    /// <summary>
    /// 负责 AB 包的构建、清理、拷贝到 ServerData/StreamingAssets
    /// </summary>
    public class AssetBundleBuilder
    {
        private readonly Action<string> logAction;
        private readonly Action<string, float> progressAction;
        private readonly IJsonManager jsonManager = DIContainer.Create<JsonManager>();
        public const string AssetCatalogName = "AssetCatalog.json";
        public const string BootConfigName = "BootConfig.json";
        public const string hotfixDllBundleName = "hotupdate";
        public const string aotDllBundleName = "hotupdateaot";

        public AssetBundleBuilder(Action<string> logAction = null, Action<string, float> progressAction = null)
        {
            this.logAction = logAction;
            this.progressAction = progressAction;
        }

        private void Log(string msg) => logAction?.Invoke(msg);
        private void Progress(string msg, float val) => progressAction?.Invoke(msg, val);

        /// <summary>
        /// 构建 AssetBundles
        /// </summary>
        public bool Build(string outputPath, BuildTarget target, BuildAssetBundleOptions options, string assetsInputPath, AssetBundlesCollections releaseCollection)
        {
            if (!Directory.Exists(outputPath))
            {
                Log($"Output path does not exist：{outputPath}，Please create path");
                return false;
            }

            if (Directory.CreateDirectory(assetsInputPath).GetFiles().Length == 0)
            {
                Log("no files to be packaged at the resource input path");
                return false;
            }

            Log("--- Starting Build ---");
            AssetBundleUtility.ClearDirectory(outputPath);

            var startTime = DateTime.Now;
            var manifest = BuildPipeline.BuildAssetBundles(outputPath, options, target);
            var duration = DateTime.Now - startTime;

            if (manifest)
            {
                Log($"Build successful! Took {duration.TotalSeconds:F2} seconds.");
                Log($"Build Count：{manifest.GetAllAssetBundles().Length}");
                Log($"Build Include：{string.Join('、', manifest.GetAllAssetBundles())}.");
            }
            else
            {
                Log("Build failed! Check console for errors.");
                return false;
            }

            AssetDatabase.Refresh();

            // 重命名为 .assetBundle
            var dirInfo = new DirectoryInfo(outputPath);
            foreach (var file in dirInfo.GetFiles())
            {
                if (file.Extension == ".manifest" || file.Extension == ".meta")
                {
                    if (file.Extension == ".meta") file.Delete();
                    continue;
                }
                var newPath = Path.ChangeExtension(file.FullName, FileUtility.AbSuffix);
                if (File.Exists(newPath)) File.Delete(newPath);
                File.Move(file.FullName, newPath);
            }
            Log($"Rename Extension To：{FileUtility.AbSuffix}");
            AssetDatabase.Refresh();
            Log("--- Build End ---\n");

            var catalog = GenerateAssetCatalog(outputPath, target, releaseCollection);
            return catalog != null;
        }

        /// <summary>
        /// 拷贝构建好的 AB 包到 ServerData 目录，并合并更新目录文件
        /// </summary>
        public void CopyToServerData(string outputPath, string serverDataPath, AssetBundlesCollections releaseCollection, BuildTarget target)
        { 
            if (!Directory.Exists(outputPath))
            {
                Log($"Output path does not exist：{outputPath}");
                return;
            }

            AssetBundleUtility.EnsureDirectoryExists(serverDataPath);
            Log("--- Start Copy To ServerData ---");

            var srcCatalogPath = Path.Combine(outputPath, AssetCatalogName);
            var dstCatalogPath = Path.Combine(serverDataPath, AssetCatalogName);

            if (!File.Exists(srcCatalogPath))
            {
                Log($"The source directory is missing {AssetCatalogName}. Please build it first.");
                return;
            }

            // 读取本次生成的资源目录
            var newCatalogJson = File.ReadAllText(srcCatalogPath);
            var newCatalog = jsonManager.FromJson<AssetCatalog>(newCatalogJson, settings: NewtonsoftJsonUtility.DefaultSerializerSettings);

            AssetCatalog serverCatalog = null;
            if (File.Exists(dstCatalogPath))
            {
                serverCatalog = jsonManager.FromJson<AssetCatalog>(File.ReadAllText(dstCatalogPath), settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
            }

            // 拷贝所有 .assetBundle 文件（只拷贝变化的）
            foreach (var (bundleName, newAbInfo) in newCatalog.ABPackageCollection)
            {
                var fileName = bundleName.WithAbSuffix();
                var srcFilePath = Path.Combine(outputPath, fileName);
                var dstFilePath = Path.Combine(serverDataPath, fileName);

                var needCopy = true;
                if (serverCatalog != null && serverCatalog.ABPackageCollection.TryGetValue(bundleName, out var oldInfo))
                {
                    if (oldInfo.Hash == newAbInfo.Hash)
                    {
                        needCopy = false;
                        Log($"跳过未变化：{bundleName}");
                    }
                }

                if (needCopy)
                {
                    File.Copy(srcFilePath, dstFilePath, true);
                    Log($"已拷贝：{fileName}");
                }
            }

            // 处理移除：releaseCollection 中不存在的包，从服务器目录删除
            if (releaseCollection)
            {
                var toDeletes = new List<string>();
                foreach (var file in Directory.GetFiles(serverDataPath, $"*{FileUtility.AbSuffix}"))
                {
                    var fileName = Path.GetFileName(file);
                    var bundleName = Path.GetFileNameWithoutExtension(fileName);
                    if (!releaseCollection.assetBundleInfos.Exists(ab => ab.assetBundleName == bundleName))
                    {
                        toDeletes.Add(file);
                    }
                }
                foreach (var file in toDeletes)
                {
                    File.Delete(file);
                    Log($"已删除：{Path.GetFileName(file)}");
                }
            }
            
            // 合并资源文件
            var finalCatalog = serverCatalog ?? new AssetCatalog();

            // 更新 Bundles 信息（以新构建的为准），这里处理的是新增/变化的包信息
            foreach (var kv in newCatalog.ABPackageCollection)
            {
                if (!finalCatalog.ABPackageCollection.ContainsKey(kv.Key))
                {
                    finalCatalog.ABPackageCollection.Add(kv.Key, kv.Value);
                }
                else
                {
                    finalCatalog.ABPackageCollection[kv.Key] = kv.Value;
                }
            }

            // 移除服务器上已不存在的包信息，这里处理不存在的包信息
            if (releaseCollection)
            {
                var bundlesToRemove = finalCatalog.ABPackageCollection.Keys
                    .Where(bName => !releaseCollection.assetBundleInfos.Exists(ab =>
                        $"{ab.assetBundleName}" == bName))
                    .ToList();
                foreach (var fileName in bundlesToRemove)
                {
                    finalCatalog.ABPackageCollection.Remove(fileName);
                }
            }

            /*
             不需要再手动遍历所有包去清理依赖项（即：其它包依赖已经移除的包，不需要手动处理，之前的逻辑已经隐式处理过了）
             如果某个包既没有变化，也不是移除包的上层依赖，它根本没有被重打包，那么它的依赖列表在旧清单中是什么样，在新清单中还是什么样。
             如果被移除的包恰好在它的旧依赖列表里，这种在逻辑上不可能发生：
             如果包 A 依赖了被移除的包 B，那么 A 一定会被反向依赖逻辑捕获并重打包。
             如果 A 没有被重打包，说明它不依赖 B，自然旧清单里也不会有 B 的依赖记录。
            */
                        
            // 更新 Assets 映射（合并策略）
            // 1. 移除所有被重打包的包（即在 newCatalog 中出现的包）在 finalCatalog 中的旧资源条目
            var rebuiltBundles = new HashSet<string>(newCatalog.ABPackageCollection.Keys);
            var assetsToRemove = finalCatalog.Assets
                .Where(entry => rebuiltBundles.Contains(entry.bundleName))
                .Select(entry => entry.key)
                .ToList();
            foreach (var key in assetsToRemove)
            {
                finalCatalog.RemoveEntry(key);
            }

            // 2. 移除属于已删除包的孤儿条目（包在 finalCatalog.ABPackageCollection 中已不存在）
            var orphanKeys = finalCatalog.Assets
                .Where(entry => !finalCatalog.ABPackageCollection.ContainsKey(entry.bundleName))
                .Select(entry => entry.key)
                .ToList();
            foreach (var key in orphanKeys)
            {
                finalCatalog.RemoveEntry(key);
            }

            // 3. 将 newCatalog 的所有资源条目添加/更新到 finalCatalog
            foreach (var entry in newCatalog.Assets)
            {
                finalCatalog.AddOrUpdateEntry(entry.key, entry);
            }
            
            // 保存合并后的清单到服务器目录
            var finalJson = jsonManager.ToJson(finalCatalog, settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
            File.WriteAllText(dstCatalogPath, finalJson);
            Log($"{AssetCatalogName} 已合并更新。");

            GenerateBootConfigAndCopyStreamingAssets(serverDataPath);
            
            AssetDatabase.Refresh();
            Log("--- End Copy To ServerData ---\n");
        }

        public void GenerateBootConfigAndCopyStreamingAssets(string serverDataPath)
        {
            // 生成启动配置
            // 可以通过配置或约定获取这个名称，这里简化为常量或从 releaseCollection 推断
            var bootConfig = new BootConfig
            {
                aotDllBundleName = aotDllBundleName,
                hotfixDllBundleName = hotfixDllBundleName,
                version = DateTime.Now.Ticks.ToString(),
                hotfixObjKey = "HotUpdateEntry"
            };
            
            var bootConfigJson = jsonManager.ToJson(bootConfig);
            var bootConfigPath = Path.Combine(serverDataPath, BootConfigName);
            File.WriteAllText(bootConfigPath, bootConfigJson);
            Log($"启动配置已生成：{bootConfigPath}");

            // 同步拷贝到 StreamingAssets（可选，也可以在 Build 流程中单独处理）
            var streamingBootConfigPath = Path.Combine(Application.streamingAssetsPath, BootConfigName);
            AssetBundleUtility.EnsureDirectoryExists(Path.GetDirectoryName(streamingBootConfigPath));
            File.Copy(bootConfigPath, streamingBootConfigPath, true);
            Log($"启动配置已拷贝到 StreamingAssets：{streamingBootConfigPath}");
        }

        /// <summary>
        /// 将选中的 AB 包拷贝到 StreamingAssets
        /// </summary>
        public void MoveToStreamingAssets(string streamingAssetsPath, string outputPath, Object[] selectedAssets)
        {
            AssetBundleUtility.EnsureDirectoryExists(streamingAssetsPath);
            AssetBundleUtility.ClearDirectory(streamingAssetsPath);
            AssetDatabase.Refresh();

            // 记录本次拷贝的包名
            var copiedBundles = new List<string>();

            // 在 Unity 编辑器中，当你选择一个 .assetBundle 文件时，
            // Unity 的 Selection 系统会自动把同名的 .manifest 文件也视为选中状态（虽然界面上可能只高亮了一个文件）
            int total = selectedAssets.Count(asset => AssetDatabase.GetAssetPath(asset).Contains(FileUtility.AbSuffix));
            for (int i = 0; i < selectedAssets.Length; i++)
            {
                Progress($"Processing：{selectedAssets[i].name}", (float)i / total);
                string assetPath = AssetDatabase.GetAssetPath(selectedAssets[i]);
                string fileName = Path.GetFileName(assetPath);
                if (!fileName.Contains(FileUtility.AbSuffix)) continue;

                AssetDatabase.CopyAsset(assetPath, Path.Combine(streamingAssetsPath, fileName));
                string bundleName = Path.GetFileNameWithoutExtension(fileName);
                copiedBundles.Add(bundleName);
            }

            // 生成首包精简目录（基于 outputPath 中的完整目录，但只保留拷贝过的包）
            GenerateStreamingCatalog(streamingAssetsPath, outputPath, copiedBundles);

            EditorUtility.ClearProgressBar();
        }
        
        private void GenerateStreamingCatalog(string streamingAssetsPath, string outputPath, List<string> copiedBundles)
        {
            if (copiedBundles.Count == 0)
            {
                Log("没有拷贝任何 AB 包，首包目录未生成。");
                return;
            }

            string fullCatalogPath = Path.Combine(outputPath, AssetCatalogName);
            if (!File.Exists(fullCatalogPath))
            {
                Log($"错误：完整目录文件不存在 {fullCatalogPath}，请先执行 Build 生成。");
                return;
            }

            var fullCatalog = jsonManager.FromJson<AssetCatalog>(File.ReadAllText(fullCatalogPath));
            Log($"完整目录加载成功，包含 {fullCatalog.ABPackageCollection.Count} 个包。");

            var streamingCatalog = new AssetCatalog();
            foreach (var bundleName in copiedBundles)
            {
                if (fullCatalog.ABPackageCollection.TryGetValue(bundleName, out var pkgInfo))
                {
                    streamingCatalog.ABPackageCollection.Add(bundleName, pkgInfo);
                    Log($"添加包信息：{bundleName}");

                    // 添加资源映射
                    var entries = fullCatalog.Assets.Where(e => e.bundleName == bundleName);
                    foreach (var entry in entries)
                    {
                        streamingCatalog.AddOrUpdateEntry(entry.key, entry);
                    }
                }
                else
                {
                    Log($"警告：完整目录中未找到包 {bundleName}，已跳过。");
                }
            }

            // 保存
            string streamingCatalogPath = Path.Combine(streamingAssetsPath, AssetCatalogName);
            File.WriteAllText(streamingCatalogPath, jsonManager.ToJson(streamingCatalog));
            Log($"首包资源目录已生成：{streamingCatalogPath}，包含 {streamingCatalog.ABPackageCollection.Count} 个包。");
        }

        /// <summary>
        /// 清理输出目录
        /// </summary>
        public void CleanOutputDirectory(string outputPath)
        {
            if (Directory.Exists(outputPath))
            {
                try
                {
                    Directory.Delete(outputPath, true);
                    Directory.CreateDirectory(outputPath);
                    Log($"Cleaned output directory: {outputPath}");
                }
                catch (Exception e)
                {
                    Log($"Failed to clean directory: {e.Message}");
                }
                AssetDatabase.Refresh();
            }
            else
            {
                Log($"Output directory does not exist: {outputPath}");
            }
        }
        
        /// <summary>
        /// 生成资源目录文件（包含包信息和资源映射）
        /// </summary>
        private AssetCatalog GenerateAssetCatalog(string outputPath, BuildTarget target, AssetBundlesCollections releaseCollection)
        {
            if (!releaseCollection)
            {
                Log("发布配置为空，无法生成资源目录。");
                return null;
            }
            
            var platformBundleName = AssetBundleUtility.GetPlatformBundleName(target);
            // 先获取主包 manifest，用于提取每个包的依赖信息（依赖信息仍需从 manifest 获取）
            var catalogPath = Path.Combine(outputPath, platformBundleName);
            if (!File.Exists(catalogPath))
            {
                Log($"主 Manifest 文件不存在：{catalogPath}，无法生成资源目录。");
                return null;
            }

            var catalog = new AssetCatalog();
            UnityEngine.AssetBundle mainBundle = null;
            try
            {
                mainBundle = UnityEngine.AssetBundle.LoadFromFile(catalogPath);
                if (!mainBundle)
                {
                    Log("无法加载主 AssetBundleManifest");
                    return null;
                }
                
                var manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (!manifest)
                {
                    Log("无法获取 AssetBundleManifest 对象");
                    return null;
                }

                // 遍历发布配置中的所有包
                foreach (var abInfo in releaseCollection.assetBundleInfos)
                {
                    var bundleName = abInfo.assetBundleName; // 不带后缀
                    var fileName = bundleName.WithAbSuffix();
                    var filePath = Path.Combine(outputPath, fileName);

                    if (!File.Exists(filePath))
                    {
                        Log($"AB包文件不在当前构建中 {filePath}，跳过该包的资源映射。");
                        continue;
                    }

                    // 获取依赖信息（从 manifest）
                    var deps = manifest.GetAllDependencies(bundleName) ?? Array.Empty<string>();
                    var fileInfo = new FileInfo(filePath);
                    var hash = HashUtility.GenerateFileSHA256Hash(filePath);
                    var pkgInfo = new ABPackageInfo(bundleName, fileInfo.Length, hash, deps);
                    catalog.ABPackageCollection.TryAdd(bundleName, pkgInfo);

                    // 加载该包的 manifest 以获取内部资源列表
                    var assetBundle = UnityEngine.AssetBundle.LoadFromFile(filePath);
                    // 判断资源类型
                    EAssetType assetType;
                    var type = AssetDatabase.GetMainAssetTypeAtPath(filePath.Substring(filePath.IndexOf("Assets")));
                    if (type == typeof(SpriteAtlas))
                    {
                        assetType = EAssetType.SpiteAtlas;
                    }
                    else if (type == typeof(SceneAsset) || assetBundle.isStreamedSceneAssetBundle)
                    {
                        assetType = EAssetType.Scene;
                    }
                    else
                    {
                        assetType = EAssetType.Object;
                    }

                    Log($"主资源类型：{type.Name}，实际资源类型：{assetType}");
                    // 遍历该包下的所有资源（直接来自发布配置）
                    foreach (var assetInfo in abInfo.assetInfos)
                    {
                        // 使用资源在项目中的原始文件名（保留大小写）
                        var key = Path.GetFileNameWithoutExtension(assetInfo.name);
                        // 重名处理：如果 key 已存在，使用完整路径作为备用
                        if (catalog.ContainsKey(key))
                        {
                            var fallbackKey = assetInfo.assetPath.ToLowerInvariant();
                            Log($"资源名称重复：{key}，已使用路径替代：{fallbackKey}，请调整命名");
                            key = fallbackKey;
                        }
                        
                        var entry = new AssetEntry(key, bundleName, assetInfo.assetPath, assetType);
                        catalog.AddOrUpdateEntry(key, entry);
                        
                        // 若是图集，将其子图片也要添加进资源目录，因为收集资源信息不会收集图片
                        if (assetInfo.assetType == EAssetType.SpiteAtlas)
                        { 
                           // 根据图集路径加载图集
                           var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetInfo.assetPath);
                           // 获取图集的所有打包对象
                           var packables = atlas.GetPackables();
                           foreach (var packable in packables)
                           {
                               var path = AssetDatabase.GetAssetPath(packable);
                               // packable 本身就是 Sprite / Texture2D 等资产
                               Log($"packable path: {path}");
                               // 如果它就是 图片相关，直接记录即可
                               if (packable is Sprite || packable is Texture2D)
                               {
                                   catalog.AddOrUpdateEntry(packable.name,
                                       new SpriteAssetEntry(packable.name, bundleName, path, EAssetType.Texture, assetInfo.assetPath,
                                           assetInfo.name));
                                   continue;
                               }
                               
                               // 如果是文件夹类 packable，就枚举这个文件夹下的 图片
                               if (packable is DefaultAsset folderAsset)
                               {
                                   var folderPath = AssetDatabase.GetAssetPath(folderAsset);
                                   var guids = AssetDatabase.FindAssets("t:Object", new[] { folderPath });
                                   foreach (var guid in guids)
                                   {
                                       var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                                       var importerType = AssetDatabase.GetImporterType(assetPath);
                                       if (importerType == typeof(TextureImporter))
                                       {
                                           var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                                           if (importer && importer.textureType == TextureImporterType.Sprite)
                                           {
                                               // 这是 Sprite 图
                                               var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                                               catalog.AddOrUpdateEntry(sprite.name,
                                                   new SpriteAssetEntry(sprite.name, bundleName, assetPath, EAssetType.Texture, assetInfo.assetPath,
                                                        assetInfo.name));
                                           }
                                           else
                                           {
                                               // 这是普通 Texture2D / 其他纹理，不当 Sprite 处理
                                               var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                                               catalog.AddOrUpdateEntry(texture2D.name,
                                                   new SpriteAssetEntry(texture2D.name, bundleName, assetPath, EAssetType.Texture, assetInfo.assetPath,
                                                       assetInfo.name));
                                           }
                                       }
                                   }
                               }
                           }
                        }
                    }
                    // 卸载该包
                    assetBundle.Unload(false);
                }
                
                // 清理主包
                if (mainBundle)
                    mainBundle.Unload(false);
                
                // 保存 JSON
                var json = jsonManager.ToJson(catalog, settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
                var savePath = Path.Combine(outputPath, AssetCatalogName);
                File.WriteAllText(savePath, json);
                Log($"资源目录已生成：{savePath}\n");
                AssetDatabase.Refresh();
                return catalog;
            }
            catch (Exception e)
            {
                Log($"生成合并清单失败：{e.Message}\n{e.StackTrace}");
            }
            finally
            {
                if (mainBundle) mainBundle.Unload(false);
                UnityEngine.AssetBundle.UnloadAllAssetBundles(true);
                EditorUtility.ClearProgressBar();
            }
            return null;
        }
    }
}
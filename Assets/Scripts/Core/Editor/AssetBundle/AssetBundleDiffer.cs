using System;
using System.Collections.Generic;
using System.Linq;
using Editor.AssetBundle;
using UnityEditor;

namespace Core.Editor.AssetBundle.Core
{
    /// <summary>
    /// 对比新旧 AssetBundlesCollections，计算差异
    /// </summary>
    public class AssetBundleDiffer
    {
        private readonly Action<string> logAction;
        private readonly Action<string, float> progressAction;

        public AssetBundleDiffer(Action<string> logAction = null, Action<string, float> progressAction = null)
        {
            this.logAction = logAction;
            this.progressAction = progressAction;
        }

        private void Log(string msg) => logAction?.Invoke(msg);
        private void Progress(string msg, float val) => progressAction?.Invoke(msg, val);

        /// <summary>
        /// 执行差异对比，返回结果数据
        /// </summary>
        public DifferenceResult Compare(AssetBundlesCollections latest, AssetBundlesCollections release)
        {
            var result = new DifferenceResult();
            Log("--- Starting Handle Difference---");

            // 日志记录用的临时容器
            var log_NewAdd_Bundles = new Dictionary<string, List<AssetBundlesCollections.AssetInfo>>();
            var log_Unused_Bundles = new Dictionary<string, List<AssetBundlesCollections.AssetInfo>>();
            var log_Unused_Assets = new Dictionary<string, List<AssetBundlesCollections.AssetInfo>>();
            var log_NewAdd_Assets = new Dictionary<string, List<AssetBundlesCollections.AssetInfo>>();
            var log_Changed_Assets = new Dictionary<string, List<AssetBundlesCollections.AssetInfo>>();

            if (release == null)
            {
                // 首次构建，全量
                foreach (var abInfo in latest.assetBundleInfos)
                    result.BundlesToRebuild.Add(abInfo.assetBundleName, new List<AssetBundlesCollections.AssetInfo>(abInfo.assetInfos));
                Log("Check First comparison Difference, will full build");
                Log("--- End Handle Difference---\n");
                return result;
            }

            // 1. 查找待移除的包（在 release 中存在，但 latest 中不存在）
            for (var i = release.assetBundleInfos.Count - 1; i >= 0; i--)
            {
                var abInfo = release.assetBundleInfos[i];
                if (!latest.assetBundleInfos.Exists(info => info.assetBundleName == abInfo.assetBundleName))
                {
                    log_Unused_Bundles[abInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>(abInfo.assetInfos);
                    result.BundlesToRemove.Add(abInfo.assetBundleName);
                }
            }

            // 2. 查找待移除的资源（包存在，但资源在 latest 中消失）
            foreach (var abInfo in release.assetBundleInfos)
            {
                var latestAbInfo = latest.assetBundleInfos.Find(info => info.assetBundleName == abInfo.assetBundleName);
                if (latestAbInfo == null) continue;

                for (int j = abInfo.assetInfos.Count - 1; j >= 0; j--)
                {
                    var assetInfo = abInfo.assetInfos[j];
                    if (!latestAbInfo.assetInfos.Exists(a => a.name == assetInfo.name))
                    {
                        // 记录当前包移除的资源信息，打印日志
                        if (!log_Unused_Assets.ContainsKey(abInfo.assetBundleName))
                            log_Unused_Assets[abInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>();
                        log_Unused_Assets[abInfo.assetBundleName].Add(assetInfo);

                        // 真正把改资源信息记录到要移除的集合中，没有这个包说明检查到这个包的资源首次被移除，添加Key，否则直接添加到value的list中
                        if (!result.AssetsToRemovePerBundle.ContainsKey(abInfo.assetBundleName))
                            result.AssetsToRemovePerBundle[abInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>();
                        result.AssetsToRemovePerBundle[abInfo.assetBundleName].Add(assetInfo);

                        // 标记这个包需要重打包（如果尚未标记）
                        if (!result.BundlesToRebuild.ContainsKey(abInfo.assetBundleName))
                        {
                            var rebuildList = new List<AssetBundlesCollections.AssetInfo>();
                            // 这个的所有资源都要重新打包（排除待移除的资源）
                            foreach (var a in abInfo.assetInfos)
                                if (a.name != assetInfo.name)
                                    rebuildList.Add(new AssetBundlesCollections.AssetInfo(a.assetPath, a.size, a.name, a.hash, a.assetType));
                            result.BundlesToRebuild.Add(abInfo.assetBundleName, rebuildList);
                        }
                        else
                        {
                            // 如果已经标记，只需确保这个被移除的资源不在重打包列表中，直接移除即可
                            var rebuildList = result.BundlesToRebuild[abInfo.assetBundleName];
                            rebuildList.RemoveAll(a => a.name == assetInfo.name);   // 问题：同名资源怎么办？？？
                        }
                    }
                }
            }

            // 3. 对比最新配置，找出新增/变化的包和资源
            for (int i = 0; i < latest.assetBundleInfos.Count; i++)
            {
                var latestAbInfo = latest.assetBundleInfos[i];
                Progress($"Handing：{latestAbInfo.assetBundleName}", (float)i / latest.assetBundleInfos.Count);

                var releaseAbInfo = release.assetBundleInfos.Find(info => info.assetBundleName == latestAbInfo.assetBundleName);
                if (releaseAbInfo == null)
                {
                    // 新增包
                    log_NewAdd_Bundles[latestAbInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>(latestAbInfo.assetInfos);
                    result.BundlesToRebuild[latestAbInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>(latestAbInfo.assetInfos);
                    continue;
                }

                // 包存在，对比资源
                foreach (var latestAsset in latestAbInfo.assetInfos)
                {
                    var releaseAsset = releaseAbInfo.assetInfos.Find(a => a.name == latestAsset.name);
                    if (releaseAsset == null)
                    {
                        // 新增资源
                        if (!log_NewAdd_Assets.ContainsKey(latestAbInfo.assetBundleName))
                            log_NewAdd_Assets[latestAbInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>();
                        log_NewAdd_Assets[latestAbInfo.assetBundleName].Add(latestAsset);

                        AddToRebuild(result, latestAbInfo.assetBundleName, latestAsset, releaseAbInfo);
                    }
                    else
                    {
                        var same = releaseAsset.hash == latestAsset.hash &&
                                   releaseAsset.size == latestAsset.size &&
                                   releaseAsset.assetPath == latestAsset.assetPath;
                        if (same) continue;

                        // 仅路径变化但内容不变：更新路径，不重打包（但你的后续逻辑可能需要更新清单，这里仍放入 Changed 日志）
                        if (releaseAsset.hash == latestAsset.hash && releaseAsset.size == latestAsset.size &&
                            releaseAsset.assetPath != latestAsset.assetPath)
                        {
                            releaseAsset.assetPath = latestAsset.assetPath;
                            if (!log_Changed_Assets.ContainsKey(latestAbInfo.assetBundleName))
                                log_Changed_Assets[latestAbInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>();
                            log_Changed_Assets[latestAbInfo.assetBundleName].Add(latestAsset);
                        }
                        else
                        {
                            // 内容变化
                            if (!result.AssetsToRemovePerBundle.ContainsKey(latestAbInfo.assetBundleName))
                                result.AssetsToRemovePerBundle[latestAbInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>();
                            result.AssetsToRemovePerBundle[latestAbInfo.assetBundleName].Add(releaseAsset);

                            AddToRebuild(result, latestAbInfo.assetBundleName, latestAsset, releaseAbInfo);
                            if (!log_Changed_Assets.ContainsKey(latestAbInfo.assetBundleName))
                                log_Changed_Assets[latestAbInfo.assetBundleName] = new List<AssetBundlesCollections.AssetInfo>();
                            log_Changed_Assets[latestAbInfo.assetBundleName].Add(latestAsset);
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            // 输出日志
            PrintDifferenceLog(log_NewAdd_Bundles, log_Unused_Bundles, log_Unused_Assets, log_NewAdd_Assets, log_Changed_Assets, result);

            Log("--- End Handle Difference---\n");
            return result;
        }

        private void AddToRebuild(DifferenceResult result, string bundleName, AssetBundlesCollections.AssetInfo latestAsset, AssetBundlesCollections.AssetBundleInfo releaseAbInfo)
        {
            if (!result.BundlesToRebuild.ContainsKey(bundleName))
            {
                var list = new List<AssetBundlesCollections.AssetInfo> { new(latestAsset.assetPath, latestAsset.size, latestAsset.name, latestAsset.hash,latestAsset.assetType) };
                // 添加该包原有且未被移除的资源
                foreach (var oldAsset in releaseAbInfo.assetInfos)
                {
                    if (oldAsset.name == latestAsset.name) continue;
                    if (result.AssetsToRemovePerBundle.TryGetValue(bundleName, out var removedList) && removedList.Exists(a => a.name == oldAsset.name))
                        continue;
                    list.Add(new AssetBundlesCollections.AssetInfo(oldAsset.assetPath, oldAsset.size, oldAsset.name, oldAsset.hash, oldAsset.assetType));
                }
                result.BundlesToRebuild.Add(bundleName, list);
            }
            else
            {
                var list = result.BundlesToRebuild[bundleName];
                // 移除旧资源，添加新资源
                var existing = list.Find(a => a.name == latestAsset.name);
                if (existing != null) list.Remove(existing);
                list.Add(new AssetBundlesCollections.AssetInfo(latestAsset.assetPath, latestAsset.size, latestAsset.name, latestAsset.hash, latestAsset.assetType));
            }
        }

        private void PrintDifferenceLog(Dictionary<string, List<AssetBundlesCollections.AssetInfo>> newBundles,
                                        Dictionary<string, List<AssetBundlesCollections.AssetInfo>> unusedBundles,
                                        Dictionary<string, List<AssetBundlesCollections.AssetInfo>> unusedAssets,
                                        Dictionary<string, List<AssetBundlesCollections.AssetInfo>> newAssets,
                                        Dictionary<string, List<AssetBundlesCollections.AssetInfo>> changedAssets,
                                        DifferenceResult result)
        {
            if (result.BundlesToRebuild.Count == 0 && result.BundlesToRemove.Count == 0 && result.AssetsToRemovePerBundle.Count == 0)
            {
                Log("No Differences\n");
                return;
            }

            foreach (var kv in unusedBundles)
                Log($"Found Unused Bundle：{kv.Key}，Include Assets：[{string.Join('、', kv.Value.Select(a => a.name))}]");
            Log("");
            foreach (var kv in newBundles)
                Log($"Found NewAdd Bundle：{kv.Key}，Include Assets：[{string.Join('、', kv.Value.Select(a => a.name))}]");
            Log("");

            foreach (var bundleName in result.BundlesToRebuild.Keys)
            {
                string unusedLog = unusedAssets.TryGetValue(bundleName, out var ua) ? $"[{string.Join('、', ua.Select(a => a.name))}]" : "";
                string newLog = newAssets.TryGetValue(bundleName, out var na) ? $"[{string.Join('、', na.Select(a => a.name))}]" : "";
                string changedLog = changedAssets.TryGetValue(bundleName, out var ca) ? $"[{string.Join('、', ca.Select(a => a.name))}]" : "";
                string includeLog = $"[{string.Join('、', result.BundlesToRebuild[bundleName].Select(a => a.name))}]";
                Log($"// {bundleName}\nFound Unused Assets：{unusedLog}\nFound NewAdd Assets：{newLog}\nFound Changed Assets：{changedLog}\nRebuild Bundle：{bundleName}，Include Assets：{includeLog}\n");
            }
        }

        public class DifferenceResult
        {
            public readonly Dictionary<string, List<AssetBundlesCollections.AssetInfo>> BundlesToRebuild = new();
            public readonly List<string> BundlesToRemove = new();
            public readonly Dictionary<string, List<AssetBundlesCollections.AssetInfo>> AssetsToRemovePerBundle = new();
            // 新增：需要强制上传的包名（不含 .assetBundle 后缀）
            public HashSet<string> ForceUploadBundles = new();
        }
    }
}
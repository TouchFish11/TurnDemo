using System;
using System.Collections.Generic;
using System.Linq;
using Core.AssetBundles.Collection;
using Newtonsoft.Json;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 资源目录
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class AssetCatalog
    {
        // 资源Key到资源映射条目的映射
        [JsonProperty] private Dictionary<string, AssetEntry> assetMap = new();
        // AB包集合
        [JsonProperty] private ABPackageCollection abPackageCollection = new();
        // 包名到该包内所有资源的 Key 列表
        [JsonProperty] private Dictionary<string, List<string>> bundleToAssetKeys = new();
        
        /// <summary>
        /// AB包清单集合
        /// </summary>
        public ABPackageCollection ABPackageCollection => abPackageCollection;
        
        /// <summary>
        /// 资源的所有Key
        /// </summary>
        public Dictionary<string, AssetEntry>.KeyCollection AssetKeys => assetMap.Keys;
        
        /// <summary>
        /// 所有资源的Values
        /// </summary>
        public Dictionary<string, AssetEntry>.ValueCollection Assets => assetMap.Values;

        public AssetEntry this[string key]
        {
            get => assetMap[key];
            set => assetMap[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return assetMap.ContainsKey(key);
        }

        /// <summary>
        /// 添加或更新条目
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entry"></param>
        public void AddOrUpdateEntry(string key, AssetEntry entry)
        {
            // 如果 key 已存在，先移除旧的记录（维护 bundleToAssetKeys）
            if (assetMap.TryGetValue(key, out var oldEntry))
            {
                // 如果旧包名与新包名不同，需要从旧包列表中移除
                if (oldEntry.bundleName != entry.bundleName)
                {
                    if (bundleToAssetKeys.TryGetValue(oldEntry.bundleName, out var oldList))
                        oldList.Remove(key);
                }
                else
                {
                    // 包名相同，直接覆盖 assetMap 即可，bundleToAssetKeys 无需变动
                    assetMap[key] = entry;
                    return;
                }
            }

            // 添加或更新 assetMap
            assetMap[key] = entry;

            // 维护 bundleToAssetKeys
            if (!bundleToAssetKeys.ContainsKey(entry.bundleName))
                bundleToAssetKeys[entry.bundleName] = new List<string>();
            bundleToAssetKeys[entry.bundleName].Add(key);
        }

        public bool RemoveEntry(string key)
        {
            if (assetMap.Remove(key, out var entry))
            {
                if (bundleToAssetKeys.TryGetValue(entry.bundleName, out var list))
                    list.Remove(key);
            }

            return false;
        }
        
        /// <summary>
        /// 获取资源条目
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AssetEntry GetEntry(string key)
        {
            return assetMap.GetValueOrDefault(key);
        }
        
        /// <summary>
        /// 获取某个包内的所有资源 Key
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAssetKeysByBundle(string bundleName)
        {
            return bundleToAssetKeys.TryGetValue(bundleName, out var keys) ? keys : Enumerable.Empty<string>();
        }

        public AssetEntry[] GetEntries(params string[] keys)
        {
            List<AssetEntry> list = new();
            foreach (var assetName in keys)
            {
                if (assetMap.TryGetValue(assetName, out var entry))
                {
                    list.Add(entry);
                }
            }
            return list.ToArray();
        }
    }
}

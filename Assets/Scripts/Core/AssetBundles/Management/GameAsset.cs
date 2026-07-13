using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DI;
using Core.Exceptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 游戏资源
    /// </summary>
    public static class GameAsset
    {
        // 资源管理器
        private static AssetManager _assetManager;
        // 句柄ID到资源定位对象的映射
        private static readonly Dictionary<int, AssetLocation> _assetIdToLocationsMap = new();
        // 全局句柄ID
        private static int _nextId;
        // 句柄id缓存池
        private static readonly Queue<int> _idPool = new();
        
        internal static void Init(IAssetBundleManager assetBundleManager)
        {
            _assetManager = DIContainer.Create<AssetManager>(parameterValues: assetBundleManager);
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AssetHandle<T> LoadAsset<T>(string key) where T : Object
        {
            var assetWrapper = _assetManager.LoadAsset<T>(key);
            AssetHandle newAssetHandle = CreateSingleHandle<T>(key);
            assetWrapper.OnUnload += () =>
            {
                // 回收ID
                _idPool.Enqueue(newAssetHandle.HandleId);
            };

            return newAssetHandle.ConvertTo<T>();
        }
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="key">资源Key</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static async Task<AssetHandle<T>> LoadAssetAsync<T>(string key) where T : Object
        {
            // 异步加载资源
            var assetWrapper = await _assetManager.LoadAssetAsync<T>(key);
            // 创建新句柄
            AssetHandle newAssetHandle = CreateSingleHandle<T>(key);
            // 资源包装不为空才去监听事件，否则直接返回句柄，外部通过句柄获取的资源就是null
            if (assetWrapper != null)
            {
                assetWrapper.OnUnload += () =>
                {
                    // 回收句柄ID
                    _idPool.Enqueue(newAssetHandle.HandleId);
                };
            }
            return newAssetHandle.ConvertTo<T>();
        }
        
        /// <summary>
        /// 异步加载相同类型的多个资源
        /// </summary>
        /// <param name="keys">相同类型的多个资源键，传入不同类型的键其返回的资源为null</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public static async Task<AssetHandle<T>> LoadAssetsAsync<T>(params string[] keys) where T : Object
        {
            // 先对 keys 排序，确保同一组资源无论传入顺序如何都能命中同一缓存
            var sortedKeys = keys.OrderBy(k => k).ToArray();
            var combinedKey = KeysToKey(sortedKeys);
            
            var tasks = new List<Task<AssetHandle<T>>>();
            var allHandles = new List<AssetHandle>();
            foreach (var key in keys)
            {
                tasks.Add(LoadAssetAsync<T>(key));
            }
            
            // 等待所有资源加载完成
            var newHandles = await Task.WhenAll(tasks);
            foreach (var handle in newHandles) 
                allHandles.Add(handle);
            
            // 返回组合句柄
            return CreateCombineHandle<T>(combinedKey, allHandles);
        }

        /// <summary>
        /// 异步加载指定AB包中的所有资源
        /// </summary>
        /// <param name="bundleName">AB包名</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public static async Task<AssetHandle<T>> LoadAllAssetAsync<T>(string bundleName) where T : Object
        {
            // 包名加类型名作为Key
            var bundleKey = $"{bundleName}_{typeof(T)}";
            // 异步加载包所有资源
            var assetWrappers = await _assetManager.LoadAllAssetAsync<T>(bundleName);

            var allHandles = new List<AssetHandle>();
            // 遍历所有资源包装，创建每个资源的句柄
            foreach (var assetWrapper in assetWrappers)
            {
                var assetHandle = CreateSingleHandle<T>(assetWrapper.AssetKey);
                allHandles.Add(assetHandle);
            }
            
            // 返回组合句柄
            return CreateCombineHandle<T>(bundleKey, allHandles);
        }

        /// <summary>
        /// 异步加载场景资源
        /// </summary>
        /// <param name="sceneKey"></param>
        /// <returns></returns>
        public static Task LoadSceneAsync(string sceneKey)
        {
            return _assetManager.LoadSceneBundleAsync(sceneKey);
        }

        /// <summary>
        /// 创建简单句柄，非组合句柄对象
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static AssetHandle<T> CreateSingleHandle<T>(string key) where T : class
        {
            // 创建新Handle
            var newHandle = new AssetHandle { HandleId = GenerateNewId(), Version = 0 };
            // 获取资源条目
            var entry = _assetManager.GetAssetEntry(key);

            string assetKey;
            string spriteKey;
            AssetLocation.ELocationType locationType;
            if (entry is SpriteAssetEntry spriteAssetEntry)
            {
                assetKey = spriteAssetEntry.atlasKey;
                spriteKey = spriteAssetEntry.key;
                locationType = AssetLocation.ELocationType.Sprite;
            }
            else
            {
                assetKey = entry.key;
                spriteKey = string.Empty;
                locationType = AssetLocation.ELocationType.NonSprite;
            }
            
            // 判断ID是否存在，存在就复用定位对象
            if (_assetIdToLocationsMap.TryGetValue(newHandle.HandleId, out var location))
            {
                location.AssetKey = assetKey;
                location.SpriteKey = spriteKey;
                location.LocationType = locationType;
                ++location.Version;
                // 同步新句柄的版本和定位对象的版本一致
                newHandle.Version = location.Version;
                return newHandle.ConvertTo<T>();
            }
            
            // 工厂创建新定位对象
            var newLocation = AssetLocationFactory.GetAssetLocation<T>(entry);
            // 缓存定位对象
            _assetIdToLocationsMap.Add(newHandle.HandleId, newLocation);
            return newHandle.ConvertTo<T>();
        }

        /// <summary>
        /// 创建组合句柄
        /// </summary>
        /// <param name="combineKey"></param>
        /// <param name="subHandles"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static AssetHandle<T> CreateCombineHandle<T>(string combineKey, IEnumerable<AssetHandle> subHandles) where T : class
        {
            // 创建新Handle，并且是组合句柄，该句柄对应的定位对象不直接指向资源，该句柄存储所有持有资源的子句柄
            var newHandle = new AssetHandle
            {
                HandleId = GenerateNewId(), Version = 0, IsCombine = true,
                CombineHandles = new List<AssetHandle>(subHandles)
            };
            
            // 判断ID是否存在，存在就复用定位对象
            if (_assetIdToLocationsMap.TryGetValue(newHandle.HandleId, out var location))
            {
                location.AssetKey = combineKey;
                ++location.Version;
                // 同步定位对象的版本到句柄
                newHandle.Version = location.Version;
                return newHandle.ConvertTo<T>();
            }
            
            // 工厂创建新定位对象
            var newLocation = AssetLocationFactory.GetAssetLocationCombine(combineKey);
            // 缓存定位对象
            _assetIdToLocationsMap.Add(newHandle.HandleId, newLocation);
            return newHandle.ConvertTo<T>();
        }
        
        /// <summary>
        /// 获取所有的场景路径（key）
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllSceneKey()
        {
            return _assetManager.GetAllSceneKey();
        }

        /// <summary>
        /// 释放句柄
        /// </summary>
        /// <param name="handle"></param>
        public static void Release(AssetHandle handle)
        {
            if(!IsValidate(handle.HandleId,  handle.Version))
                return;

            if (handle.IsCombine)
            {
                foreach (var assetHandle in handle.CombineHandles)
                {
                    Release(assetHandle);
                }
                
                // 需要主动移除该句柄本身，因为组合的句柄本身不直接指向某个资源，所以不会响应资源卸载的回调
                _idPool.Enqueue(handle.HandleId);
            }
            else
            {
                var location = _assetIdToLocationsMap.GetValueOrDefault(handle.HandleId);
                if (location == null)
                    return;
                // 因为资源管理器是按照资源Key到资源包装的映射缓存
                // 若资源是图集，则资源包装映射的是图集资源Key，所以要使用图集Key，而不是图片Key
                var assetKey = location.AssetKey;
                // 减少引用计数
                _assetManager.ReleaseWrapper(assetKey);
            }
        }

        /// <summary>
        /// 获取类型资源，若验证的id和版本无效，返回null；泛型句柄提供资源给外部时使用该方法返回对应的资源
        /// </summary>
        /// <param name="handleId">句柄唯一ID</param>
        /// <param name="version">版本号</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>若该句柄对应的资源是GameObject，则T返回该资源身上的组件；非GameObject直接返回该资源</returns>
        internal static T GetAsset<T>(int handleId, int version) where T : class
        {
            // 无效的句柄访问，抛出异常
            if (!IsValidate(handleId, version))
                throw ExceptionHelper.ThrowInvalidHandleAccessException(handleId, version, null);
            
            // 找不到定位对象，抛出异常，实际上不会找不到，因为都是复用的
            var location = _assetIdToLocationsMap.GetValueOrDefault(handleId);
            if(location == null)
                throw new NullReferenceException($"Location does not exist, handle(id {handleId}, version {version})");
            
            // 若是图集图片资源的定位对象，则通过图集名和图片名访问对应的资源
            if (location.LocationType == AssetLocation.ELocationType.Sprite)
            {
                return _assetManager.GetSprite(location.AssetKey, location.SpriteKey) as T;
            }

            // 非图集资源的定位对象，只需直接访问资源即可
            var asset = _assetManager.GetAsset(location.AssetKey);
            // 若资源本身是GameObject，T要是组件类型，从资源中获取对应组件类型返回
            if (asset is GameObject objAsset && typeof(Component).IsAssignableFrom(typeof(T)))
                return objAsset.GetComponent<T>();
            // 否则直接转换为T返回，比如非实例化资源，纯GameObject
            return asset as T;
        }
        
        /// <summary>
        /// 生成句柄ID
        /// </summary>
        /// <returns></returns>
        private static int GenerateNewId()
        {
            int reusedId;
            if (_idPool.Count > 0)
            {
                reusedId = _idPool.Dequeue();
            }
            else
            {
                reusedId = ++_nextId;
            }
            
            return reusedId;
        }

        /// <summary>
        /// 多个键拼接成一个Key缓存
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private static string KeysToKey(params string[] keys)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < keys.Length; i++)
            {
                sb.Append(keys[i]);
                if(i <  keys.Length - 1)
                    sb.Append("_");
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// 校验句柄有效性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private static bool IsValidate(int id, int version)
        {
            return _assetIdToLocationsMap.TryGetValue(id, out var location) && location.Version == version;
        }
    }
}

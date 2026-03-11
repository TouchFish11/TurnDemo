using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Collection;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.Tasks.Extensions;
using HybridCLR;
using UnityEngine;

namespace Core.HotUpdate
{
    /// <summary>
    /// 热更新管理器
    /// </summary>
    public class HotUpdateManager : SingletonBase<HotUpdateManager>, IHotUpdateManager
    {
        public override int Priority => 2;
        // 缓存热更程序集名称
        private readonly ConcurrentBag<string> _assemblyNames = new();
        private IAssetBundleManager _assetBundleManager;
        
        private HotUpdateManager(){}

        public override Task InitAsync()
        {
            _assetBundleManager = ServiceLocator.Get<IAssetBundleManager>();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 加载指定程序集
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assemblyNames"></param>
        public async Task LoadAssembliesAsync(string abName, params string[] assemblyNames)
        {
            var uniList = ListUtility.GetUniList<string>();
            uniList.AddRange(assemblyNames);
            var assetBundle = await _assetBundleManager.LoadBundleAsync(abName);
            // 加载热更新AB包资源
            var dllTexts = ListUtility.GetUniList<TextAsset>();
            await assetBundle.LoadAllAssetsAsync<TextAsset>().ToTask(dllTexts.List);
            foreach (var dllText in dllTexts.List)
            {
                if (!uniList.Contains(dllText.name))
                {
                    continue;
                }
                
#if !UNITY_EDITOR
                // 多线程加载程序集
                await LoadAssemblyAsyncInternal(dllText.bytes);
#else
                // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
                // Editor下无需加载，直接查找获得HotUpdate程序集
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name != dllText.name.Substring(0, dllText.name.LastIndexOf('.')))
                    {
                        continue;
                    }
                    
                    _assemblyNames.Add(assembly.GetName().Name);
                    LogManager.Log($"已缓存编辑器加载热更程序集名称，{dllText.name}");
                    TestFun(assembly);
                }
#endif
            }

            ListUtility.CollectUniList(uniList);
            _assetBundleManager.UnloadBundle(abName);
        }

        public async Task LoadAssembliesAsync(string abName)
        {
            var assetBundle = await _assetBundleManager.LoadBundleAsync(abName);
            // 加载热更新AB包资源
            var dllTexts = ListUtility.GetUniList<TextAsset>();
            await assetBundle.LoadAllAssetsAsync<TextAsset>().ToTask(dllTexts.List);
            foreach (var dllText in dllTexts.List)
            {
                if (_assemblyNames.Contains(dllText.name.Substring(0, dllText.name.LastIndexOf('.'))))
                {
                    continue;
                }
#if !UNITY_EDITOR
                // 多线程加载程序集
                await LoadAssemblyAsyncInternal(dllText.bytes);
#else
                // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
                // Editor下无需加载，直接查找获得HotUpdate程序集
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name != dllText.name.Substring(0, dllText.name.LastIndexOf('.')))
                    {
                        continue;
                    }
                    
                    _assemblyNames.Add(assembly.GetName().Name);
                    LogManager.Log($"已缓存编辑器加载热更程序集名称，{dllText.name}");
                    TestFun(assembly);
                }
#endif
            }
            
            _assetBundleManager.UnloadBundle(abName);
        }

        public Assembly GetAssembly(string assemblyName)
        {
            return Assembly.Load(assemblyName);
        }
        
        public Assembly GetCoreModule()
        {
            return Assembly.Load("CoreModule");
        }
        
        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <returns></returns>
        public Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>
            {
                GetCoreModule(),
            };
            
            // 获取所有热更后的程序集
            assemblies.AddRange(ServiceLocator.Get<IHotUpdateManager>().GetHotAssemblies()); 
            return assemblies.ToArray();
        }

        public Assembly[] GetHotAssemblies()
        {
            var assemblies = new List<Assembly>(_assemblyNames.Count);
            foreach (var assemblyName in _assemblyNames)
            {
                assemblies.Add(Assembly.Load(assemblyName));
            }
            return assemblies.ToArray();
        }

        /// <summary>
        /// 异步加载程序集
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal Task LoadAssemblyAsyncInternal(byte[] bytes)
        {
            return Task.Run(() =>
            {
                var assembly = Assembly.Load(bytes);
                RuntimeApi.LoadMetadataForAOTAssembly(bytes, HomologousImageMode.SuperSet);
                _assemblyNames.Add(assembly.GetName().Name);
                LogManager.Log($"{nameof(HotUpdateManager)}.{nameof(LoadAssembliesAsync)}：已加载热更程序集，{assembly.GetName().Name}");
                TestFun(assembly);
            });
        }

        private static void TestFun(Assembly assembly)
        {
            try
            {
                // 1. 验证程序集是否真的加载成功
                if (assembly.GetName().Name != "HotUpdate.Config")
                {
                    return;
                }
                LogManager.Log($"✅ 程序集加载成功：{assembly.FullName}");

                // 2. 遍历程序集中所有类型，打印完整信息（关键！）
                Type[] allTypes = assembly.GetTypes();
                LogManager.Log($"✅ 程序集中包含 {allTypes.Length} 个类型");

                bool foundItemInfo = false;
                foreach (Type t in allTypes)
                {
                    LogManager.Log($"类型：{t.FullName} | 命名空间：{t.Namespace} | 名称：{t.Name}");

                    // 精准匹配 ItemInfo（不区分大小写，避免拼写错误）
                    if (t.Name.Equals("ItemInfo", StringComparison.OrdinalIgnoreCase))
                    {
                        foundItemInfo = true;
                        LogManager.Log($"✅ 找到 ItemInfo：FullName={t.FullName}, Namespace={t.Namespace}");
                    }
                }

                // 3. 验证是否真的找不到 ItemInfo
                if (!foundItemInfo)
                {
                    LogManager.Log("❌ 程序集中未找到任何名称为 ItemInfo 的类型！");
                }
                else
                {
                    // 4. 尝试直接获取类型（模拟你的加载逻辑）
                    Type itemInfoType = assembly.GetType("ItemInfo"); // 无命名空间则写纯类型名
                    if (itemInfoType == null)
                    {
                        // 尝试带命名空间（兜底）
                        itemInfoType = assembly.GetType("HotUpdate.Config.ItemInfo");
                    }

                    if (itemInfoType != null)
                    {
                        LogManager.Log($"✅ 成功获取 ItemInfo 类型：{itemInfoType}");
                    }
                    else
                    {
                        LogManager.Log("❌ 能遍历到 ItemInfo，但 GetType 无法获取！");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Log($"❌ 调试代码报错：{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name != dllText.name.Substring(0, dllText.name.LastIndexOf('.')))
                    {
                        continue;
                    }
                    
                    _assemblyNames.Add(assembly.GetName().Name);
                    LogManager.Log($"已缓存编辑器加载热更程序集名称，{dllText.name}");
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
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name != dllText.name.Substring(0, dllText.name.LastIndexOf('.')))
                    {
                        continue;
                    }
                    
                    _assemblyNames.Add(assembly.GetName().Name);
                    LogManager.Log($"已缓存编辑器加载热更程序集名称，{dllText.name}");
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

        public Assembly GetGameModule()
        {
            return Assembly.Load("GameModule");
        }
        
        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <returns></returns>
        public Assembly[] GetAssemblies()
        {
            ListUtility.GetUniList<Assembly>();
            var assemblies = new List<Assembly>
            {
                GetCoreModule(),
            };
            
            // 获取所有热更后的程序集
            assemblies.AddRange(ServiceLocator.Get<IHotUpdateManager>().GetHotAssemblies()); 
            return assemblies.ToArray();
        }
        
        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public int GetAssemblies(List<Assembly> assemblies)
        {
            assemblies.Add(GetCoreModule());
            assemblies.Add(GetGameModule());
            // 获取所有热更后的程序集
            assemblies.AddRange(ServiceLocator.Get<IHotUpdateManager>().GetHotAssemblies()); 
            return assemblies.Count;
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
        /// 获取所有热更程序集
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public int GetHotAssemblies(List<Assembly> assemblies)
        {
            foreach (var assemblyName in _assemblyNames)
            {
                assemblies.Add(Assembly.Load(assemblyName));
            }
            return assemblies.Count;
        }

        /// <summary>
        /// 异步加载程序集
        /// </summary>
        /// <param name="bytes">程序集字节数组</param>
        /// <returns></returns>
        internal Task LoadAssemblyAsyncInternal(byte[] bytes)
        {
            return Task.Run(() =>
            {
                try
                {
                    var assembly = Assembly.Load(bytes);
                    _assemblyNames.Add(assembly.GetName().Name);
                    LogManager.Log($"{nameof(HotUpdateManager)}.{nameof(LoadAssembliesAsync)}：已加载热更程序集{assembly.GetName().Name}");
                }
                catch (Exception e)
                {
                    LogManager.LogError($"{nameof(HotUpdateManager)}.{nameof(LoadAssemblyAsyncInternal)}：热更程序集加载错误{e.Message}");
                }
            });
        }

        public void LoadMetadataForAOTAssemblies(List<string> aotDlls)
        {
            foreach (var aotDllName in aotDlls)
            {
                var assemblyBytes = GetAssemblyBytes(aotDllName);
                var errorCode = RuntimeApi.LoadMetadataForAOTAssembly(assemblyBytes, HomologousImageMode.SuperSet);
                LogManager.Log($"{nameof(HotUpdateManager)}.{nameof(LoadMetadataForAOTAssemblies)}：已补充元数据{aotDllName}，错误码：{errorCode}");
            }
        }

        public void LoadAssemblyAsyncByFile(params string[] assemblyNames)
        {
            try
            {
                foreach (var assemblyName in assemblyNames)
                {
                    var assemblyBytes = GetAssemblyBytes(assemblyName);
                    var assembly = Assembly.Load(assemblyBytes);
                    _assemblyNames.Add(assembly.GetName().Name);
                    LogManager.Log($"{nameof(HotUpdateManager)}.{nameof(LoadAssembliesAsync)}：已加载热更程序集{assembly.GetName().Name}");
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(HotUpdateManager)}.{nameof(LoadAssemblyAsyncByFile)}：程序集加载错误，{e.Message}");
            }
        }

        /// <summary>
        /// 获取程序集字节数组
        /// </summary>
        /// <param name="assemblyNameWithExtension">包含拓展名的程序集名称</param>
        /// <returns></returns>
        private static byte[] GetAssemblyBytes(string assemblyNameWithExtension)
        {
            return File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, $"{assemblyNameWithExtension}.bytes"));
        }
    }
}

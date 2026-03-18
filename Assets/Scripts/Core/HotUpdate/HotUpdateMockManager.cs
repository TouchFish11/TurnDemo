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
using UnityEngine;

namespace Core.HotUpdate
{
    /// <summary>
    /// 模拟热更新管理器
    /// </summary>
    public class HotUpdateMockManager : SingletonBase<HotUpdateMockManager>, IHotUpdateManager
    {
        public override int InitPriority => 2;
        // 缓存热更程序集名称
        private readonly ConcurrentBag<string> _assemblyNames = new();
        private IAssetBundleManager _assetBundleManager;
        
        private HotUpdateMockManager(){}

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
            var uniList = ListUtility.GetUniList<string>().AddRange(assemblyNames);
            // 加载热更新AB包资源
            var assetBundle = await _assetBundleManager.LoadBundleAsync(abName);
            var dllTexts = ListUtility.GetUniList<TextAsset>();
            await assetBundle.LoadAllAssetsAsync<TextAsset>().ToTask(dllTexts.List);
            foreach (var dllText in dllTexts.List)
            {
                if (!uniList.Contains(dllText.name))
                {
                    continue;
                }
                
                // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，直接查找获得HotUpdate程序集，重复加载反而会出问题。
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assemblyName = dllText.name[..dllText.name.LastIndexOf('.')];
                    if (assembly.GetName().Name != assemblyName)
                    {
                        continue;
                    }
                    
                    _assemblyNames.Add(assembly.GetName().Name);
                    LogManager.Log($"{nameof(HotUpdateMockManager)}.{nameof(LoadAssembliesAsync)}:已缓存编辑器加载热更程序集名称，{dllText.name}");
                }
            }

            ListUtility.CollectUniList(uniList);
            _assetBundleManager.UnloadBundle(abName);
        }
        
        public async Task LoadAssembliesAsync(string abName)
        {
            // 加载热更新AB包资源
            var assetBundle = await _assetBundleManager.LoadBundleAsync(abName);
            var dllTexts = ListUtility.GetUniList<TextAsset>();
            await assetBundle.LoadAllAssetsAsync<TextAsset>().ToTask(dllTexts.List);
            foreach (var dllText in dllTexts.List)
            {
                if (_assemblyNames.Contains(dllText.name[..dllText.name.LastIndexOf('.')]))
                {
                    continue;
                }
                
                // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，直接查找获得HotUpdate程序集，重复加载反而会出问题。
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name != dllText.name[..dllText.name.LastIndexOf('.')])
                    {
                        continue;
                    }
                    
                    _assemblyNames.Add(assembly.GetName().Name);
                    LogManager.Log($"{nameof(HotUpdateMockManager)}.{nameof(LoadAssembliesAsync)}:已缓存编辑器加载热更程序集{dllText.name}");
                }
            }
            
            ListUtility.CollectUniList(dllTexts);
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
        
        public Assembly[] GetAssemblies()
        {
            ListUtility.GetUniList<Assembly>();
            var assemblies = new List<Assembly>
            {
                GetCoreModule(),
            };
            
            // 获取所有热更后的程序集
            assemblies.AddRange(GetHotAssemblies()); 
            return assemblies.ToArray();
        }
        
        public int GetAssemblies(List<Assembly> assemblies)
        {
            assemblies.Add(GetCoreModule());
            assemblies.Add(GetGameModule());
            // 获取所有热更后的程序集
            assemblies.AddRange(GetHotAssemblies()); 
            return assemblies.Count;
        }
        
        public Assembly[] GetHotAssemblies()
        {
            var assemblies = ListUtility.GetUniList<Assembly>();
            foreach (var assemblyName in _assemblyNames)
            {
                assemblies.Add(Assembly.Load(assemblyName));
            }
            return assemblies.List.ToArray();
        }
        
        public int GetHotAssemblies(List<Assembly> assemblies)
        {
            foreach (var assemblyName in _assemblyNames)
            {
                assemblies.Add(Assembly.Load(assemblyName));
            }
            return assemblies.Count;
        }
        
        public void LoadMetadataForAOTAssemblies(List<string> aotDlls)
        {
            // 编辑器下不需要补充元数据
        }
    }
}

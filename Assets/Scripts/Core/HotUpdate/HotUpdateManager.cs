using System.Collections.Generic;
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
        public override int Priority => -1;

        // 缓存热更程序集名称
        private readonly List<string> _assemblyNames = new();

        private HotUpdateManager(){}

        public override Task InitAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 加载指定程序集
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assemblyNames"></param>
        public async Task LoadAssembliesAsync(string abName, params string[] assemblyNames)
        {
            var uniList = CollectionUtil.GetUniList<string>();
            uniList.AddRange(assemblyNames);
            Clear();
            var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(abName);
            // 加载热更新AB包资源
            var dllTexts = new List<TextAsset>();
            await assetBundle.LoadAllAssetsAsync<TextAsset>().ToTask(dllTexts);
            foreach (var dllText in dllTexts)
            {
                if (!uniList.List.Contains(dllText.name))
                {
                    continue;
                }
                
                // TODO：多线程加载程序集
#if !UNITY_EDITOR
                var assembly = Assembly.Load(dllText.bytes);
                RuntimeApi.LoadMetadataForAOTAssembly(dllText.bytes, HomologousImageMode.SuperSet);
                _assemblyNames.Add(assembly.GetName().Name);
                LogManager.Log($"{nameof(HotUpdateManager)}.{nameof(LoadAssemblys)}：已加载热更程序集，{assembly.GetName().Name}");
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
                }
#endif
            }

            CollectionUtil.CollectUniList(uniList);
            ServiceLocator.Get<IAssetBundleManager>().UnloadBundle(abName);
        }

        public async Task LoadAssemblysAsync(string abName)
        {
            var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(abName);
            // 加载热更新AB包资源
            var dllTexts = new List<TextAsset>();
            await assetBundle.LoadAllAssetsAsync<TextAsset>().ToTask(dllTexts);
            foreach (var dllText in dllTexts)
            {
                if (_assemblyNames.Contains(dllText.name))
                {
                    continue;
                }
                
                // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
                var assembly = Assembly.Load(dllText.bytes);
                RuntimeApi.LoadMetadataForAOTAssembly(dllText.bytes, HomologousImageMode.SuperSet);
                _assemblyNames.Add(assembly.GetName().Name);
                LogManager.Log($"{nameof(HotUpdateManager)}.{nameof(LoadAssemblys)}：已加载热更程序集，{assembly.GetName().Name}");
#else
                // Editor下无需加载，直接查找获得HotUpdate程序集
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
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
            
            ServiceLocator.Get<IAssetBundleManager>().UnloadBundle(abName);
        }

        public Assembly GetAssembly(string assemblyName)
        {
            return Assembly.Load(assemblyName);
        }
        
        public Assembly GetCoreModule()
        {
            return Assembly.Load("CoreModule");
        }
        
        public Assembly GetConfigModule()
        {
            return Assembly.Load("ConfigModule");
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
                GetConfigModule(),
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
        /// 清理名称缓存
        /// </summary>
        private void Clear()
        {
            _assemblyNames.Clear();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Log;
using Core.Service;
using Core.Singleton;
using HybridCLR;
using UnityEngine;

namespace Core.HotUpdate
{
    /// <summary>
    /// 热更新管理器
    /// </summary>
    public class HotUpdateManager : SingletonBase<HotUpdateManager>, IHotUpdateManager
    {
        // 缓存热更程序集
        private readonly Dictionary<string, Assembly>  _nameToAssemblyMap = new();
        
        private HotUpdateManager()
        {
            
        }

        public async Task LoadAssemblys()
        {
            // 卸载原来的旧缓存
            UnloadAll();
            // 加载热更新AB包资源
            var dllTexts = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetsAsync<TextAsset>(EAssetBundleType.HotUpdate);
            foreach (var dllText in dllTexts)
            {
                // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
                UnloadAssembly(dllText.name);
                var assembly = Assembly.Load(dllText.bytes);
                RuntimeApi.LoadMetadataForAOTAssembly(dllText.bytes, HomologousImageMode.SuperSet);
                _nameToAssemblyMap.Add(assembly.GetName().Name, assembly);
                LogManager.Log($"已加载热更程序集，{assembly.GetName().Name}");
#else
                // Editor下无需加载，直接查找获得HotUpdate程序集
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name == dllText.name.Substring(0, dllText.name.LastIndexOf('.')))
                    {
                        _nameToAssemblyMap.Add(assembly.GetName().Name, assembly);
                        LogManager.Log($"已缓存编辑器已自动加载热更程序集，{dllText.name}");
                    }
                }
#endif
            }

        }
        
        public async Task LoadAssembly(string assemblyName)
        {
            // 卸载原来的旧缓存
            UnloadAssembly(assemblyName);
            // 加载热更新AB包资源
            var dllText = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<TextAsset>(EAssetBundleType.HotUpdate, assemblyName);
            // 通过字节数组加载程序集
            var assembly = Assembly.Load(dllText.bytes);
            // 缓存加载的程序集
            _nameToAssemblyMap.Add(assembly.GetName().Name, assembly); 
        }

        public Assembly GetAssembly(string assemblyName)
        {
            return _nameToAssemblyMap.GetValueOrDefault(assemblyName);
        }

        public Assembly[] GetAssemblies()
        {
            return new List<Assembly>(_nameToAssemblyMap.Values).ToArray();
        }

        public void UnloadAssembly(string assemblyName)
        {
            _nameToAssemblyMap.Remove(assemblyName);
        }

        public void UnloadAll()
        {
            _nameToAssemblyMap.Clear();
        }
    }
}

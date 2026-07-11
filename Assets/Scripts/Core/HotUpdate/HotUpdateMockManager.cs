using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Log;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.HotUpdate
{
    /// <summary>
    /// 模拟热更新管理器
    /// </summary>
    public class HotUpdateMockManager : IHotUpdateManager
    {
        // 缓存热更程序集名称
        private readonly ConcurrentBag<string> _assemblyNames = new();

        private HotUpdateMockManager()
        {

        }
        
        public Task LoadAssembliesAsync(HotUpdateAssemblySettings settings, List<TextAsset> textAssets)
        {
            foreach (var dllText in textAssets)
            {
                if (_assemblyNames.Contains(dllText.name[..dllText.name.LastIndexOf('.')]))
                    continue;
                
                // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，直接查找获得HotUpdate程序集，重复加载反而会出问题。
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GetName().Name != dllText.name[..dllText.name.LastIndexOf('.')])
                        continue;
                    
                    _assemblyNames.Add(assembly.GetName().Name);
                    Logger.LogDebug(ELogTags.HotUpdate, $"Editor found hotfix dll({dllText.name})");
                }
            }
            return Task.CompletedTask;
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
            var assemblies = new List<Assembly>();
            foreach (var assemblyName in _assemblyNames)
            {
                if(assemblyName.Contains("HotUpdate"))
                    assemblies.Add(Assembly.Load(assemblyName));
            }
            return assemblies.ToArray();
        }
        
        public int GetHotAssemblies(List<Assembly> assemblies)
        {
            foreach (var assemblyName in _assemblyNames)
            {
                assemblies.Add(Assembly.Load(assemblyName));
            }
            return assemblies.Count;
        }
        
        public void LoadMetadataForAOTAssemblies(Dictionary<string, byte[]> aotDlls)
        {
            // 编辑器下不需要补充元数据
        }
    }
}

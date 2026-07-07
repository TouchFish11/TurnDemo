using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.HotUpdate
{
    /// <summary>
    /// 热更新管理器
    /// </summary>
    public class HotUpdateManager : IHotUpdateManager
    {
        // 缓存热更程序集名称
        private readonly ConcurrentBag<string> _assemblyNames = new();
        // 热更新程序集设置
        private HotUpdateAssemblySettings _hotUpdateAssemblySettings;
        // 排序后的dll列表
        private readonly List<string> _sortDlls = new();
        // 记录所有已处理节点
        private readonly HashSet<string> _visited = new();
        // 记录当前递归路径，用于检测循环
        private readonly HashSet<string> _visiting = new();
        
        private HotUpdateManager()
        {

        }
        
        public void LoadMetadataForAOTAssemblies(IReadOnlyList<string> aotDlls)
        {
            foreach (var aotDllName in aotDlls)
            {
                var assemblyBytes = GetAssemblyBytes(aotDllName);
                var errorCode = RuntimeApi.LoadMetadataForAOTAssembly(assemblyBytes, HomologousImageMode.SuperSet);
                Logger.Log($"{nameof(HotUpdateManager)}.{nameof(LoadMetadataForAOTAssemblies)}:已补充元数据{aotDllName}，错误码:{errorCode}");
            }
        }

        public async Task LoadAssembliesAsync(HotUpdateAssemblySettings settings, List<TextAsset> textAssets)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            AnalysisDependence(settings.dllDependencies);
            // 按照依赖顺序加载程序集资源
            foreach (var nameWithExtension in _sortDlls)
            {
                foreach (var dllText in textAssets)
                {
                    if (nameWithExtension != dllText.name) continue;
                    // 多线程加载程序集
                    await LoadAssemblyAsyncInternal(dllText.bytes);
                }
            }
        }
        
        /// <summary>
        /// 分析程序集依赖
        /// </summary>
        /// <param name="dllDependencies"></param>
        private void AnalysisDependence(Dictionary<string, List<string>> dllDependencies)
        {
            _visited.Clear();
            _visiting.Clear();
            _sortDlls.Clear();

            // 遍历所有涉及的 DLL
            foreach (var dllName in dllDependencies.Keys)
            {
                DFS(dllName, dllDependencies);
            }
            
            // 将依赖项提前，先加载依赖项
            _sortDlls.Reverse();
            return;

            void DFS(string dllName, Dictionary<string, List<string>> dllDependencies)
            {
                // 如果已经处理过，直接返回
                if (_visited.Contains(dllName))
                    return;

                // 检测循环依赖
                if (!_visiting.Add(dllName))
                {
                    throw new Exception($"检测到循环依赖：{dllName} 在递归中重复出现！");
                }

                // 先递归处理所有依赖项
                if (dllDependencies.TryGetValue(dllName, out var dependencies))
                {
                    foreach (var dependency in dependencies)
                    {
                        DFS(dependency, dllDependencies);
                    }
                }

                _visiting.Remove(dllName);
                _visited.Add(dllName);
    
                // 依赖项都已添加后，再添加自己
                _sortDlls.Add(dllName);
            }
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
            var assemblies = new List<Assembly>
            {
                GetCoreModule(),
                GetGameModule()
            };
            
            // 获取所有热更后的程序集
            assemblies.AddRange(GetHotAssemblies()); 
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
            assemblies.AddRange(GetHotAssemblies()); 
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
        private Task LoadAssemblyAsyncInternal(byte[] bytes)
        {
            return Task.Run(() =>
            {
                try
                {
                    var assembly = Assembly.Load(bytes);
                    _assemblyNames.Add(assembly.GetName().Name);
                    Logger.Log($"{nameof(HotUpdateManager)}.{nameof(LoadAssemblyAsyncInternal)}:已加载热更程序集{assembly.GetName().Name}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(HotUpdateManager)}.{nameof(LoadAssemblyAsyncInternal)}:热更程序集加载错误{e.Message}");
                    Logger.LogError($"{nameof(HotUpdateManager)}.{nameof(LoadAssemblyAsyncInternal)}:热更程序集加载错误{e.Message}");
                }
            });
        }
        
        /// <summary>
        /// TODO：补充的程序集单独打包AB包加载
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

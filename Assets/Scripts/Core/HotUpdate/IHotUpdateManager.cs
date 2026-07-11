using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.HotUpdate
{
    /// <summary>
    /// 热更新管理器接口
    /// </summary>
    public interface IHotUpdateManager
    {
        /// <summary>
        /// 获取加载的指定热更程序集
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        Assembly GetAssembly(string assemblyName);
        
        /// <summary>
        /// 获取所有加载的热更程序集
        /// </summary>
        /// <returns></returns>
        Assembly[] GetHotAssemblies();

        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <returns></returns>
        Assembly[] GetAssemblies();

        Assembly GetCoreModule();
        
        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        int GetAssemblies(List<Assembly> assemblies);

        /// <summary>
        /// 获取所有热更程序集
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        int GetHotAssemblies(List<Assembly> assemblies);

        Assembly GetGameModule();

        /// <summary>
        /// 补充元数据
        /// </summary>
        /// <param name="aotDlls">补充程序集名称列表</param>
        void LoadMetadataForAOTAssemblies(Dictionary<string, byte[]> aotDlls);

        Task LoadAssembliesAsync(HotUpdateAssemblySettings settings, List<TextAsset> textAssets);
    }
}

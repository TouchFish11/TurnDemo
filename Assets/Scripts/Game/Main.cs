using System;
using Core.AssetBundles.Management;
using Core.HotUpdate;
using Core.Log;
using Core.Service;
using Core.Singleton;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 主入口
    /// </summary>
    public class Main : SingletonMono<Main>
    {
        // 默认包名称数组
        private readonly string[] DefaultAbNames = { "default", "fonts", "tmp_asset", "hotupdate" };
        
        /// <summary>
        /// 游戏启动入口
        /// </summary>
        private async void Start()
        {
            try
            {
                // 注册框架核心服务
                await ServiceLocator.RegisterServices();
                // 初始化AB包管理器
                await ServiceLocator.Get<IAssetBundleManager>().Init();
                var hotUpdateManager = ServiceLocator.Get<IHotUpdateManager>();
                // 补充元数据
                hotUpdateManager.LoadMetadataForAOTAssemblies(AOTGenericReferences.PatchedAOTAssemblyList);  
                // 加载指定程序集
                await hotUpdateManager.PreLoadAssembliesAsync(DefaultAbNames[3]);
                // 实例化热更入口对象
                var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(DefaultAbNames[0]);
                var entry = assetBundle.LoadAsset<GameObject>("HotUpdateEntry");
                Instantiate(entry);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(Main)}.{nameof(Start)}: 游戏启动错误，{e.Message}");
            }
        }
    }
}

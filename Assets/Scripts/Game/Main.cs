using System;
using System.Collections.Generic;
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
        // 默认程序集名称数组，按依赖排序
        private readonly string[] DefaultAssemblyNames = { "HotUpdate.Config.dll", "HotUpdate.Common.dll", "HotUpdate.Core.dll", "HotUpdate.Entry.dll" };
        // 补充的元数据数组
        private readonly List<string> _aotDlls = new()
        {
            "CoreModule.dll",
            "GameModule.dll",
        };
        
        /// <summary>
        /// 游戏启动入口
        /// </summary>
        private async void Start()
        {
            try
            {
                // 初始化游戏设置
                InitSettings();
                // 注册框架核心服务
                await ServiceLocator.RegisterServices();
                // 初始化指定AB包
                await ServiceLocator.Get<IAssetBundleManager>().InitSpecifyAsync(DefaultAbNames);
                var hotUpdateManager = ServiceLocator.Get<IHotUpdateManager>();
                // 补充元数据
                hotUpdateManager.LoadMetadataForAOTAssemblies(_aotDlls);  
                // 加载指定程序集
                await hotUpdateManager.LoadAssembliesAsync(DefaultAbNames[3], DefaultAssemblyNames);
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
                                          
        /// <summary>
        /// 初始化设置
        /// </summary>
        private static void InitSettings()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }
    }
}

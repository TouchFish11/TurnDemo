using System;
using System.IO;
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
        
        /// <summary>
        /// 游戏启动入口
        /// </summary>
        private async void Start()
        {
            try
            {
                // 设置
                InitSettings();
                // 注册框架核心服务
                await ServiceLocator.RegisterServices();
                // 初始化指定AB包
                await ServiceLocator.Get<IAssetBundleManager>().InitSpecifyAsync(DefaultAbNames);
                var hotUpdateManager = ServiceLocator.Get<IHotUpdateManager>();
                // 加载指定程序集
                await hotUpdateManager.LoadAssembliesAsync(DefaultAbNames[1], DefaultAssemblyNames);
                var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(DefaultAbNames[0]);
                var entry = assetBundle.LoadAsset<GameObject>("HotUpdateEntry");
                Instantiate(entry);
            }
            catch (Exception e)
            {
                // var logPath = Path.Combine(Application.persistentDataPath, "Game.Main_Start_Exception_log.txt");
                // await File.WriteAllTextAsync(logPath, $"{nameof(Main)}.{nameof(Start)}: {e.Message}，{e.StackTrace}");
                LogManager.LogError($"{nameof(Main)}.{nameof(Start)}: {e.Message}，{e.StackTrace}");
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

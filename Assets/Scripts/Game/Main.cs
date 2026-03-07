using System;
using Core.AssetBundles.Management;
using Core.HotUpdate;
using Core.Log;
using Core.Quit;
using Core.Reflection;
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
        private readonly string[] DefaultAbNames = { "default", "hotupdate" };
        // 默认程序集名称数组
        private readonly string[] DefaultAssemblyName = { "HotUpdate.Entry", "HotUpdate.Common"};
        // 主入口代理名称
        private const string GameEntryName = "HotUpdate.Entry.GameEntry";
        
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
                ServiceLocator.InitService();
                // 激活退出处理器
                ServiceLocator.Get<IQuitHandler>().ActiveHandler();
                // 初始化框架工厂
                ServiceLocator.Get<IFactoryManager>().InitCoreFactorys();
                // 初始化指定AB包
                await ServiceLocator.Get<IAssetBundleManager>().InitSpecifyAsync(DefaultAbNames);
                
                var hotUpdateManager = ServiceLocator.Get<IHotUpdateManager>();
                // 加载指定程序集
                await hotUpdateManager.LoadAssembliesAsync(DefaultAbNames[1], DefaultAssemblyName);
                // 获取HotUpdate.Entry程序集
                var assembly = hotUpdateManager.GetAssembly(DefaultAssemblyName[0]);
                var type = assembly.GetType(GameEntryName);
                if (type == null)
                {
                    throw new Exception($"未找到该类型：{GameEntryName}");
                }

                var methodInfo = type.GetMethod($"Run");
                if (methodInfo == null)
                {
                    throw new Exception($"未找到该类型方法：Run");
                }
                
                methodInfo.Invoke(null, null);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(Main)}.{nameof(Start)}: {e.Message}，StackTrace：{e.StackTrace}");
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

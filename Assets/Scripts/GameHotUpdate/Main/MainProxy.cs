using System;
using System.Diagnostics;
using System.Reflection;
using Core.AssetBundles.Management;
using Core.AssetBundles.Update;
using Core.HotUpdate;
using Core.Log;
using Core.Quit;
using Core.Reflection;
using Core.Scene;
using Core.Service;
using Core.Singleton;
using Core.UI;
using GameHotUpdate.Config;
using GameHotUpdate.Update.UI;
using UnityEngine;

namespace GameHotUpdate.Main
{
    /// <summary>
    /// 主入口代理
    /// </summary>
    public class MainProxy : SingletonMono<MainProxy>
    {
        /// <summary>
        /// 游戏启动入口方法（Unity生命周期）
        /// 异步执行游戏初始化全流程，包含框架、数据、管理器、场景等初始化逻辑
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
                // 初始化AB包更新器
                ServiceLocator.Get<IAssetBundleUpdater>().Init();
                // 初始化默认AB包
                await ServiceLocator.Get<IAssetBundleManager>().InitDefault(AbKeyCollection.Default);
                // 初始化UI管理器，创建画布和UI相机
                await ServiceLocator.Get<IUIManager>().InitUIManagerAsync(
                    AbKeyCollection.Default, ResKeyCollection.Canvas, ResKeyCollection.UICamera);
                
                // 显示开始界面
                var controller = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BeginView, BeginModel, BeginController>(AbKeyCollection.Default, E_UILayer.Mid, ResKeyCollection.BeginView);
                // 检查更新
                await controller.CheckUpdate();

                controller.OnEnterGame += async () =>
                {
                    // 更新成功，初始化AB包管理器，加载本地AB包资源
                    await ServiceLocator.Get<IAssetBundleManager>().Init();
                    LogManager.Log($"初始化AB包管理器，加载本地AB包资源成功");
                
                    // 重新初始化UI管理器
                    await ServiceLocator.Get<IUIManager>().InitUIManagerAsync(
                        AbKeyCollection.Default, ResKeyCollection.Canvas, ResKeyCollection.UICamera);
                    // 初始化场景
                    await ServiceLocator.Get<ISceneManager>().Init(AbKeyCollection.Scene);
            
                    // 加载热更程序集，应该更新后再加载程序集
                    await ServiceLocator.Get<IHotUpdateManager>().LoadAssemblys(AbKeyCollection.Hotupdate);
                    LogManager.Log($"加载热更程序集成功");
                    
                    InitHotUpdateGameMain();
                };
                
                {
                    // 
                    // ...
                    // 更新后重启
                    // // 获取当前程序的路径和参数
                    // var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                    // var arguments = Environment.CommandLine.Replace($"\"{exePath}\"", "").Trim();
                    // // 启动新进程
                    // Process.Start(new ProcessStartInfo(exePath, arguments));
                    // // 关闭当前进程
                    // Application.Quit();
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MainProxy)}.{nameof(Start)}: {e.Message}");
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
        
        /// <summary>
        /// 初始化热更新游戏入口
        /// </summary>
        private static void InitHotUpdateGameMain()
        {
            const string mainTypeName = "GameHotUpdate.Main.HotfixGameMain";
            // 通过游戏热更初始化
            foreach (var hotUpdateAssembly in ServiceLocator.Get<IHotUpdateManager>().GetHotAssemblies())
            {
                var main = hotUpdateAssembly.GetType(mainTypeName);
                if (main != null)
                {
                    var methodInfo = main.GetMethod("Init", BindingFlags.Static | BindingFlags.NonPublic);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(null, null);
                    }
                    else
                    {
                        LogManager.LogError($"未找到方法：Init");
                    }
                    break;
                }

                LogManager.LogError($"未找到类型：{mainTypeName}");
            }
        }
        
        public static void RestartGame()
        {
            if (Application.isEditor)
            {

                return;
            }

            try
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (string.IsNullOrEmpty(exePath)) return;

                // 防止无限重启
                if (Environment.CommandLine.Contains("--noRestart"))
                    return;

                // 构造新参数（保留原参数 + 添加防重入标记）
                string originalArgs = Environment.CommandLine;
                string argsWithoutExe = originalArgs
                    .Substring(originalArgs.IndexOf('"', 1) + 1) // 跳过第一个引号包围的 exe 路径
                    .Trim();
                string newArgs = argsWithoutExe + " --noRestart";

                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = newArgs,
                    UseShellExecute = true,      // 关键！
                    CreateNoWindow = false
                };

                Process.Start(startInfo);
                Application.Quit();
            }
            catch (Exception e)
            {
                Application.Quit(); // 至少退出
            }
        }
    }
}

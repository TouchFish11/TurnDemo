using System;
using System.Reflection;
using Core.AssetBundles.Management;
using Core.AssetBundles.Update;
using Core.HotUpdate;
using Core.Log;
using Core.Quit;
using Core.Service;
using Core.Singleton;
using Core.Utility;
using UnityEngine;

namespace Game.Main
{
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
                Application.targetFrameRate = 60;
                Application.runInBackground = true;
            
                // 注册框架核心服务（依赖注入初始化）
                ServiceLocator.InitService();
                LogManager.Log($"{nameof(MainProxy)}.{nameof(Start)}：注册框架核心服务成功");
            
                // 激活退出处理器
                ServiceLocator.Get<IQuitHandler>().ActiveHandler();

                // 初始化AB包更新器
                ServiceLocator.Get<IAssetBundleUpdater>().Init();
                LogManager.Log($"{nameof(MainProxy)}.{nameof(Start)}：初始化AB包更新器成功");
            
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

                // 初始化AB包管理器，加载本地AB包资源
                await ServiceLocator.Get<IAssetBundleManager>().Init();
                LogManager.Log($"{nameof(MainProxy)}.{nameof(Start)}：初始化AB包管理器，加载本地AB包资源成功");
            
                // 加载热更程序集，应该更新后再加载程序集
                await ServiceLocator.Get<IHotUpdateManager>().LoadAssemblys();
                LogManager.Log($"{nameof(MainProxy)}.{nameof(Start)}：加载热更程序集成功");
            
                const string mainTypeName = "GameHotUpdate.Main.HotfixGameMain";
                // 通过游戏热更初始化
                foreach (var hotUpdateAssembly in AssemblyUtility.GetHotUpdateAssemblies())
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
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MainProxy)}.{nameof(Start)}: {e.Message}");
            }
        }
    }
}

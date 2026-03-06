using System;
using System.Diagnostics;
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
        // 默认包名称
        private const string DefaultAbName = "default";
        // 默认程序集名称
        private const string DefaultAssemblyName = "HotUpdate.Default";
        // 主入口代理名称
        private const string MainProxyName = "HotUpdate.Default.MainProxy";
        
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
                // 初始化默认AB包
                await ServiceLocator.Get<IAssetBundleManager>().InitDefault(DefaultAbName);
                
                // 加载默认包的程序集
                var hotUpdateManager = ServiceLocator.Get<IHotUpdateManager>();
                await hotUpdateManager.LoadAssemblys(DefaultAbName);
                var assembly = hotUpdateManager.GetAssembly(DefaultAssemblyName);
                
                var type = assembly.GetType(MainProxyName);
                if (type == null)
                {
                    throw new Exception($"未找到该类型：{MainProxyName}");
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
            catch (Exception)
            {
                Application.Quit(); // 至少退出
            }
        }
    }
}

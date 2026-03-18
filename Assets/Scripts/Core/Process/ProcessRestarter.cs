using System;
using System.Diagnostics;
using Core.Log;
using UnityEngine;

namespace Core.Process
{
    using Process = System.Diagnostics.Process;

    /// <summary>
    /// 进程重启器
    /// </summary>
    public class ProcessRestarter
    {
        /// <summary>
        /// 重新启动游戏进程
        /// </summary>
        public static void RestartProcess()
        {
            if (Application.isEditor)
            {
                LogManager.Log($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：模拟重启成功，请退出播放模式，重新进入");
                return;
            }

            try
            {
                Process.GetCurrentProcess().Refresh();
                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    LogManager.Log($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：模块文件：{module.FileName}，模块名称：{module.ModuleName}");
                }
                
                var processModule = Process.GetCurrentProcess().MainModule;
                if (processModule == null)
                { 
                    throw new Exception($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：processModule为null");
                }
                
                var exePath = processModule.FileName;
                if (string.IsNullOrEmpty(exePath))
                {
                    throw new Exception($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：exePath路径为null");
                }
                LogManager.Log($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：exePath路径为:{exePath}");

                // 防止无限重启
                if (Environment.CommandLine.Contains("--noRestart"))
                {
                    throw new Exception($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：CommandLine包含noRestart");
                }
                LogManager.Log($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：CommandLine不包含noRestart");
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(startInfo);
                Application.Quit();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：{e.Message}");
                Application.Quit(); // 至少退出
            }
        }
    }
}

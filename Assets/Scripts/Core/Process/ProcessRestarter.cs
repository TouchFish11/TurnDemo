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
                var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (string.IsNullOrEmpty(exePath))
                {
                    LogManager.LogError($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：exePath路径为null");
                    return;
                }

                // 防止无限重启
                if (Environment.CommandLine.Contains("--noRestart"))
                {
                    LogManager.LogError($"{nameof(ProcessRestarter)}.{nameof(RestartProcess)}：CommandLine包含noRestart");
                    return;
                }

                // 构造新参数
                var originalArgs = Environment.CommandLine;
                var argsWithoutExe = originalArgs
                    .Substring(originalArgs.IndexOf('"', 1) + 1) // 跳过第一个引号包围的 exe 路径
                    .Trim();
                var newArgs = $"{argsWithoutExe} --noRestart";

                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = newArgs,
                    UseShellExecute = true,
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

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Global;
using Core.Net;
using Core.Singleton;
using Core.Utility;
using UnityEngine;

namespace Core.Log
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public class LogManager : SingletonBase<LogManager>, ILogManager
    {
        // 日志队列
        private readonly ConcurrentQueue<string> _logs = new ConcurrentQueue<string>();
        // 日志线程
        private Thread logThread;
        // 是否正在运行日志线程
        private bool _isLogRunning;
        // 日志唯一ID
        private static ulong Id;
        // 日志字符串构建器
        private readonly StringBuilder sb = new StringBuilder();
        // 日志保存路径
        private static string LogSavePath;
        // 写入日志最大间隔时间
        private static ushort WriteLogMaxIntervalTime;

        private LogManager()
        {
            QuitHandler.QuitHandler.Instance.OnAppQuit += OnApplicationQuit;
            LogSavePath = PathUtility.GetLogLocalSavePath(FileUtility.LocalLogFileName);
            WriteLogMaxIntervalTime = GlobalSettings.Instance.writeLogMaxIntervalTime;
            InitLogFile();
            StartLogWrite();
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(object msg)
        {
            Instance.GenerateLog(msg.ToString(), GetStackTrace(2), LogType.Log);
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg);
#endif
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msgWarning">������־</param>
        public static void LogWarning(object msgWarning)
        {
            Instance.GenerateLog(msgWarning.ToString(), GetStackTrace(), LogType.Warning);
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(msgWarning);
#endif
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msgError">������־</param>
        public static void LogError(object msgError)
        {
            Instance.GenerateLog(msgError.ToString(), GetStackTrace(), LogType.Error);
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(msgError);
#endif
        }

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="exception">�쳣��־</param>
        public static void LogException(Exception exception)
        {
            Instance.GenerateLog(exception.ToString(), GetStackTrace(), LogType.Exception);
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(exception);
#endif
        }

        /// <summary>
        /// 上传日志
        /// </summary>
        /// <param name="progressCallBack"></param>
        public void UploadLog(UploadProgressCallBack progressCallBack)
        {
            UWRManager.Instance.UploadAssetAsync(GlobalSettings.Instance.uploadServerIp, LogSavePath, progressCallBack: progressCallBack);
        }

        /// <summary>
        /// 初始化日志文件
        /// </summary>
        private void InitLogFile()
        {
            if (!File.Exists(LogSavePath))
            {
                File.Create(LogSavePath).Close();
            }
            else
            {
                File.WriteAllText(LogSavePath, string.Empty);
            }
        }

        /// <summary>
        /// 开始日志写入
        /// </summary>
        private void StartLogWrite()
        {
            EnableLog = true;
            _isLogRunning = true;
            logThread ??= new Thread(WriteLogAsync);
            logThread.IsBackground = true;
            logThread.Start();
        }

        /// <summary>
        /// 保存剩余日志
        /// </summary>
        private void SaveRemainLog()
        {
            File.AppendAllLines(LogSavePath, _logs);
        }

        /// <summary>
        /// 生成日志
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void GenerateLog(string condition, string stackTrace, LogType type)
        {
            // 不启用日志
            if (!EnableLog)
            {
                return;
            }

            // 不记录任何日志
            if (((int)GlobalSettings.Instance.filterLevel & (int)type) == 0)
            {
                return;
            }
            
            sb.Clear();
            // 拼接日志信息
            sb.Append(++Id + Environment.NewLine).AppendLine($"[{type}]:{condition}").Append($"stackTrace:{stackTrace}\n");
            // 放入日志队列
            _logs.Enqueue(sb.ToString());
        }

        /// <summary>
        /// 获取堆栈跟踪
        /// </summary>
        /// <param name="skipFrames"></param>
        /// <returns></returns>
        private static string GetStackTrace(int skipFrames = 0)
        {
            try
            {
                var stackTrace = new StackTrace(skipFrames, true);
                var sb = new StringBuilder();
                
                for (var i = 0; i < stackTrace.FrameCount; i++)
                {
                    var frame = stackTrace.GetFrame(i);
                    if (frame == null)
                    {
                        continue;
                    }

                    // 获取方法
                    var method = frame.GetMethod();
                    if (method == null)
                    {
                        continue;
                    }

                    // 空行
                    sb.Append(Environment.NewLine);

                    // 
                    if (method.DeclaringType != null)
                        sb.Append($"{method.DeclaringType.Namespace}.{method.DeclaringType.Name}.{method.Name}:");
                    // 获取文件名
                    var fileFunllName = frame.GetFileName();
                    if (fileFunllName != null)
                    {
                        var index = fileFunllName.LastIndexOf('\\');
                        if (index != -1)
                        {
                            var fileName = fileFunllName.Substring(fileFunllName.LastIndexOf('\\') + 1);
                            sb.Append($"{fileName}");
                        }
                    }
                    sb.Append($"({frame.GetFileLineNumber()})");
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                return $"调用堆栈获取失败:{e.Message}";
            }
        }

        /// <summary>
        /// 异步写入日志
        /// </summary>
        private void WriteLogAsync()
        {
            var startTime = DateTime.Now.AddSeconds(WriteLogMaxIntervalTime);
            while (_isLogRunning)
            {
                if (DateTime.Now < startTime)
                {
                    continue;
                }
                
                try
                {
                    if (_logs.Count > 0)
                    {
                        File.AppendAllLines(LogSavePath, _logs);
                        _logs.Clear();
                    }
                }
                catch (Exception ex)
                {
                    LogError($"日志写入异常:{ex.Message}");
                }

                startTime = DateTime.Now.AddSeconds(WriteLogMaxIntervalTime);
            }
        }

        private async Task OnApplicationQuit()
        {
            // 停止日志写入线程
            _isLogRunning = false;
            // 保存未写入的日志
            SaveRemainLog();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 启用日志
        /// </summary>
        public bool EnableLog { get; set; }
    }
}

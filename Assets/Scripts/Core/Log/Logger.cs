using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Core.DI;
using Core.Global;
using Core.Mono;
using Core.Net;
using Core.Utility;

namespace Core.Log
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public class Logger : ILogger, IApplicationExitNotify
    {
        private static readonly ILogger _logger = DIContainer.Create<Logger>();
        private readonly IUWRManager _uWRManager;
        
        public int QuitPriority => 1;
        // 日志队列
        private readonly ConcurrentQueue<string> _logs = new();
        // 日志线程
        private Thread logThread;
        // 是否正在运行日志线程
        private bool _isLogRunning;
        // 日志唯一ID
        private static ulong Id;
        // 日志字符串构建器
        private readonly StringBuilder sb = new();
        // 日志保存路径
        private static string LogSavePath;
        // 写入日志最大间隔时间
        private static ushort WriteLogMaxIntervalTime;

        private Logger(IUWRManager uWRManager, IMonoAdapter monoAdapter)
        {
            _uWRManager = uWRManager;
            monoAdapter.AddApplicationExitNotify(this);
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
            _logger.GenerateLog(msg.ToString(), GetStackTrace(2), ELogLevel.Log);
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
            _logger.GenerateLog(msgWarning.ToString(), GetStackTrace(2), ELogLevel.Warning);
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
            _logger.GenerateLog(msgError.ToString(), GetStackTrace(2), ELogLevel.Error);
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
            _logger.GenerateLog(exception.ToString(), GetStackTrace(2), ELogLevel.Exception);
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
            _uWRManager.UploadAssetAsync(GlobalSettings.Instance.uploadServerIp, LogSavePath, progressCallBack: progressCallBack);
        }

        /// <summary>
        /// 初始化日志文件
        /// </summary>
        private static void InitLogFile()
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
        public void GenerateLog(string condition, string stackTrace, ELogLevel type)
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
            sb.Append($"{++Id}\t").Append($"[{type}]:{condition}\n");
            if (type is ELogLevel.Error or ELogLevel.Exception)
            {
                sb.Append($"stackTrace:{stackTrace}\n");
            }
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
            var list = new List<Assembly>();
            //DIContainer.GetInstance<IHotUpdateManager>().GetAssemblies(uniList.List);
            
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
                    if (method == null) continue;
                    
                    // 空行
                    sb.Append(Environment.NewLine);

                    // 声明该成员的类的Type对象不为null且存在于程序集列表中
                    if (method.DeclaringType != null)
                    {
                        sb.Append($"{method.DeclaringType.Namespace}.{method.DeclaringType.Name}.{method.Name}:");
                    }

                    // 获取文件名
                    var fileFullName = frame.GetFileName();
                    if (fileFullName != null)
                    {
                        var index = fileFullName.LastIndexOf('\\');
                        if (index != -1)
                        {
                            var fileName = fileFullName.Substring(fileFullName.LastIndexOf('\\') + 1);
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
        
        public void OnAppQuit()
        {
            // 停止日志写入线程
            _isLogRunning = false;
            Log($"{nameof(Logger)}.{nameof(OnAppQuit)}:---日志写入结束---");
            // 保存未写入的日志
            SaveRemainLog();
        }

        /// <summary>
        /// 启用日志
        /// </summary>
        public bool EnableLog { get; set; }
    }
}

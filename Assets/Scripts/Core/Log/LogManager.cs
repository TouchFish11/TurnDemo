using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Core.Global;
using Core.Mono;
using Core.Net;
using Core.Utility;
using Debug = UnityEngine.Debug;

namespace Core.Log
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    internal class LogManager : IApplicationExitNotify
    {
        private readonly IUWRManager _uWRManager;
        private readonly IMonoAdapter _monoAdapter;
        
        // 日志唯一ID
        private static ulong s_id;
        // 日志保存路径
        private static string s_logSavePath;
        // 写入日志最大间隔时间
        private static ushort s_writeLogMaxIntervalTime;
        
        // 日志缓存队列
        private readonly ConcurrentQueue<string> _logs = new();
        // 额外日志堆栈
        private readonly HashSet<(ELogLevel level, ELogTags tag)> _extraStaceStacks = new();
        // 日志线程
        private Thread _logThread;
        // 是否正在运行日志线程
        private bool _isLogRunning;
        // 日志字符串构建器
        private readonly StringBuilder _logBuilder = new();
        
        public int QuitPriority => 1;
        
        /// <summary>
        /// 是否启用日志，关闭后不会写入文件和打印到控制台
        /// </summary>
        public bool EnableLog { get; set; }
        
        private LogManager(IUWRManager uWRManager, IMonoAdapter monoAdapter)
        {
            monoAdapter.AddApplicationExitNotify(this);
            s_logSavePath = PathUtility.GetLogLocalSavePath(FileUtility.LocalLogFileName);
            s_writeLogMaxIntervalTime = GlobalSettings.Instance.logModuleConfig.writeLogMaxIntervalTime;
            InitLogFile();
            StartLogWrite();
            
            _uWRManager = uWRManager;
            _monoAdapter = monoAdapter;
        }
        
        /// <summary>
        /// 上传日志
        /// </summary>
        /// <param name="progressCallBack"></param>
        public void UploadLog(UploadProgressCallBack progressCallBack)
        {
            _uWRManager.UploadAssetAsync(GlobalSettings.Instance.uploadModuleConfig.uploadServerIp, s_logSavePath, progressCallBack: progressCallBack);
        }

        public void AddExtraStaceStackForCombinedType(ELogLevel level, ELogTags tag)
        {
            _extraStaceStacks.Add((level, tag));
        }

        /// <summary>
        /// 生成格式化后的日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="tag"></param>
        /// <param name="condition"></param>
        /// <param name="memberName"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public void GenerateFormatLog(ELogLevel logLevel, ELogTags tag, string condition, string memberName = "", string filePath = "", int lineNumber = 0)
        {
            if (!CanGenerateFormatLog(logLevel, tag))
            {
                return;
            }
            
            // 拼接日志信息
            _logBuilder.Clear();
            _logBuilder.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            _logBuilder.Append($"[{logLevel}] ");
            _logBuilder.Append($"[{tag}] ");
            _logBuilder.Append($"{Path.GetFileName(filePath)}:{lineNumber} ({memberName}) ");
            _logBuilder.Append($":{condition}.");
            
            // 该组合是否额外生成堆栈信息
            if (_extraStaceStacks.Contains((logLevel, tag)))
            {
                _logBuilder.Append($"[{condition}] ");
                _logBuilder.Append($"StackTrace:{GetStackTrace(2)}.");
            }
            _logBuilder.Append(Environment.NewLine);
            
            var formattedMsg = _logBuilder.ToString();
            // 编辑器：输出到 Console
#if UNITY_EDITOR
            switch (logLevel)
            {
                case ELogLevel.Debug:
                case ELogLevel.Info: 
                    Debug.Log(formattedMsg); 
                    break;
                case ELogLevel.Warning: Debug.LogWarning(formattedMsg); 
                    break;
                case ELogLevel.Error: Debug.LogError(formattedMsg);
                    break;
            }
#endif
            
            // 放入日志队列
            _logs.Enqueue(formattedMsg);
        }

        public void GenerateExceptionFormatLog(ELogTags tag, Exception exception, string memberName = "", string filePath = "", int lineNumber = 0)
        {
            if (!CanGenerateFormatLog(ELogLevel.Exception, tag))
            {
                return;
            }
            
            // 拼接日志信息
            _logBuilder.Clear();
            _logBuilder.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            _logBuilder.Append($"[{ELogLevel.Exception}] ");
            _logBuilder.Append($"[{tag}] ");
            _logBuilder.Append($"{Path.GetFileName(filePath)}:{lineNumber} ({memberName}) ");
            _logBuilder.Append($":{exception}");
            _logBuilder.Append(Environment.NewLine);
            
#if UNITY_EDITOR
            Debug.LogException(exception);
#endif
            // 放入日志队列
            _logs.Enqueue(_logBuilder.ToString());
        }

        /// <summary>
        /// 能否生成格式化日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private bool CanGenerateFormatLog(ELogLevel logLevel, ELogTags tag)
        {
            // 不启用日志
            if (!EnableLog)
            {
                return false;
            }
            
            // 不同时满足条件，不记录该日志
            if (((int)GlobalSettings.Instance.logModuleConfig.filterLevel & (int)logLevel) == 0 && 
                ((int)GlobalSettings.Instance.logModuleConfig.tag & (int)tag) == 0)
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// 开始日志写入
        /// </summary>
        private void StartLogWrite()
        {
            EnableLog = true;
            _isLogRunning = true;
            _logThread ??= new Thread(WriteLogAsync);
            _logThread.IsBackground = true;
            _logThread.Start();
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
            var startTime = DateTime.Now.AddSeconds(s_writeLogMaxIntervalTime);
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
                        File.AppendAllLines(s_logSavePath, _logs);
                        _logs.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ELogTags.None, ex);
                }

                startTime = DateTime.Now.AddSeconds(s_writeLogMaxIntervalTime);
            }
        }
        
        /// <summary>
        /// 初始化日志文件
        /// </summary>
        private static void InitLogFile()
        {
            if (!File.Exists(s_logSavePath))
            {
                File.Create(s_logSavePath).Close();
            }
            else
            {
                File.WriteAllText(s_logSavePath, string.Empty);
            }
        }
        
        /// <summary>
        /// 保存剩余日志
        /// </summary>
        private void SaveRemainLog()
        {
            File.AppendAllLines(s_logSavePath, _logs);
        }
        
        public void OnAppQuit()
        {
            // 停止日志写入线程
            _isLogRunning = false;
            // 保存未写入的日志
            SaveRemainLog();
            _monoAdapter.RemoveApplicationExitNotify(this);
        }
    }
}

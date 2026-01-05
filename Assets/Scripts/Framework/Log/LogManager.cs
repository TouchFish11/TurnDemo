using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public class LogManager : SingletonBase<LogManager>, ILogManager
    {
        // 日志列表
        private readonly ConcurrentQueue<string> _logs = new ConcurrentQueue<string>();
        // 日志写入线程
        private Thread logThread;
        // 是否正在执行日志线程
        private bool _isLogRunning;
        // 日志id
        private static ulong Id = 0;
        // 拼接日志信息
        private readonly StringBuilder sb = new StringBuilder();
        // 日志保存路径缓存
        private static string LogSavePath;
        // 日志写入最大间隔时间
        private static ushort WriteLogMaxIntervalTime;

        private LogManager()
        {
            QuitHandler.Instance.OnAppQuit += OnApplicationQuit;
            LogSavePath = PathUtility.GetLogLocalSavePath(FileUtility.LocalLogFileName);
            WriteLogMaxIntervalTime = GlobalSettings.Instance.WriteLogMaxIntervalTime;
            InitLogFile();
            StartLogWrite();
        }

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="msg">普通日志</param>
        public static void Log(object msg)
        {
            Instance.GenerateLog(msg.ToString(), GetStackTrace(2), LogType.Log);
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg);
#endif
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msgWarning">警告日志</param>
        public static void LogWarning(object msgWarning)
        {
            Instance.GenerateLog(msgWarning.ToString(), GetStackTrace(), LogType.Warning);
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(msgWarning);
#endif
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msgError">错误日志</param>
        public static void LogError(object msgError)
        {
            Instance.GenerateLog(msgError.ToString(), GetStackTrace(), LogType.Error);
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(msgError);
#endif
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="exception">异常日志</param>
        public static void LogException(Exception exception)
        {
            Instance.GenerateLog(exception.ToString(), GetStackTrace(), LogType.Exception);
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(exception);
#endif
        }

        /// <summary>
        /// 上传日志到服务器
        /// </summary>
        /// <param name="progressCallBack">上传进度回调当前进度（0-1）</param>
        public void UploadLog(UploadProgressCallBack progressCallBack)
        {
            UWRManager.Instance.UploadAssetAsync(GlobalSettings.Instance.UploadServerIp, LogSavePath, progressCallBack: progressCallBack);
        }

        /// <summary>
        /// 初始化日志文件
        /// </summary>
        private void InitLogFile()
        {
            //判断文件是否存在
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
        /// 开启日志写入
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
        /// 生成日志消息
        /// </summary>
        /// <param name="condition">日志内容</param>
        /// <param name="stackTrace">调用栈</param>
        /// <param name="type">日志类型</param>
        private void GenerateLog(string condition, string stackTrace, LogType type)
        {
            if (!EnableLog)
            {
                return;
            }

            //过滤未选择的日志级别
            if (((int)GlobalSettings.Instance.FilterLevel & (int)type) == default)
            {
                return;
            }

            //清空上次残留信息
            sb.Clear();
            //拼接日志信息
            sb.Append(++Id + Environment.NewLine).AppendLine($"[{type}]：{condition}").Append($"stackTrace：{stackTrace}");
            //存储日志
            _logs.Enqueue(sb.ToString());
        }

        /// <summary>
        /// 获取当前函数的调用堆栈信息
        /// </summary>
        /// <param name="skipFrames">跳过的堆栈帧数（默认0，不跳过）</param>
        /// <returns>格式化的堆栈字符串</returns>
        private static string GetStackTrace(int skipFrames = 0)
        {
            try
            {
                // 创建StackTrace对象：skipFrames表示跳过的帧数（比如跳过当前函数自身）
                // fNeedFileInfo：是否获取文件路径和行号（需开启调试模式）
                StackTrace stackTrace = new StackTrace(skipFrames, true);
                StringBuilder sb = new StringBuilder();

                // 遍历每一层堆栈帧
                for (int i = 0; i < stackTrace.FrameCount; i++)
                {
                    StackFrame frame = stackTrace.GetFrame(i);
                    if (frame == null)
                    {
                        continue;
                    }

                    // 获取方法信息
                    var method = frame.GetMethod();
                    if (method == null)
                    {
                        continue;
                    }

                    // 拼接堆栈信息（可自定义格式）
                    sb.Append(Environment.NewLine);

                    // 拼接：命名空间.类名.方法名
                    sb.Append($"{method.DeclaringType.Namespace}.{method.DeclaringType.Name}.{method.Name}:");
                    // 拼接：文件名（若存在）
                    string fileFunllName = frame.GetFileName();
                    if (fileFunllName != null)
                    {
                        int index = fileFunllName.LastIndexOf('\\');
                        if (index != -1)
                        {
                            string fileName = fileFunllName.Substring(fileFunllName.LastIndexOf('\\') + 1);
                            sb.Append($"{fileName}");
                        }
                    }
                    sb.Append($"({frame.GetFileLineNumber()})");
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                return $"获取堆栈失败：{e.Message}";
            }
        }

        /// <summary>
        /// 多线程写入日志
        /// </summary>
        private void WriteLogAsync()
        {
            System.DateTime startTime = System.DateTime.Now.AddSeconds(WriteLogMaxIntervalTime);
            while (_isLogRunning)
            {
                if (System.DateTime.Now >= startTime)
                {
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
                        LogError($"日志写入失败：{ex.Message}");
                    }

                    startTime = System.DateTime.Now.AddSeconds(WriteLogMaxIntervalTime);
                }
            }
        }

        private async Task OnApplicationQuit()
        {
            //关闭日志写入线程
            _isLogRunning = false;
            //写入剩余日志
            SaveRemainLog();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 是否启用日志
        /// </summary>
        public bool EnableLog { get; set; }
    }
}

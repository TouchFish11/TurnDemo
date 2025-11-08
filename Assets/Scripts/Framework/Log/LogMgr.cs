using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public sealed class LogMgr : SingletonAutoMono<LogMgr>
    {
        //日志列表
        private readonly ConcurrentQueue<string> _logs = new ConcurrentQueue<string>();
        //日志写入线程
        private Thread logThread;
        //是否正在执行日志线程
        private bool _isLogRunning;
        //日志id
        private static ulong Id = 0;
        //拼接日志信息
        private readonly StringBuilder sb = new StringBuilder();
        //日志保存路径缓存
        private static string LogSavePath;
        //日志写入最大间隔时间
        private static ushort WriteLogMaxIntervalTime;

        private void Awake()
        {
            LogSavePath = PathManager.GetLogLocalSavePath(FileUtility.LocalLogFileName);
            WriteLogMaxIntervalTime = GlobalSettings.Instance.WriteLogMaxIntervalTime;
            Application.logMessageReceived += LogMessageReceived;
            InitLogFile();
            StartLogWrite();
        }

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="msg">普通日志</param>
        public static void Log(object msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msgWarning">警告日志</param>
        public static void LogWarning(object msgWarning)
        {
            Debug.LogWarning(msgWarning);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msgError">错误日志</param>
        public static void LogError(object msgError)
        {
            Debug.LogError(msgError);
        }

        /// <summary>
        /// 上传日志到服务器
        /// </summary>
        /// <param name="progressCallBack">上传进度回调当前进度（0-1）</param>
        public void UploadLog(UploadProgressCallBack progressCallBack)
        {
            UWRMgr.Instance.UploadAssetAsync(GlobalSettings.Instance.UploadServerIp, LogSavePath, progressCallBack: progressCallBack);
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
        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (EnableLog)
            {
                //过滤未选择的日志级别
                if(((int)GlobalSettings.Instance.FilterLevel & (int)type) == default)
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

        private void OnApplicationQuit()
        {
            //关闭日志写入线程
            _isLogRunning = false;
            //写入剩余日志
            SaveRemainLog();
        }

        protected override void OnDestroy()
        {
            Application.logMessageReceived -= LogMessageReceived;
            base.OnDestroy();
        }

        /// <summary>
        /// 是否启用日志
        /// </summary>
        public bool EnableLog { get; set; }
    }
}

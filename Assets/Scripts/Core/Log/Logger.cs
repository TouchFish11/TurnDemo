using System;
using System.Runtime.CompilerServices;
using Core.DI;

namespace Core.Log
{
    /// <summary>
    /// 日志器
    /// </summary>
    public class Logger
    {
        private static readonly LogManager s_logManager;
        
        static Logger()
        {
            s_logManager = DIContainer.Create<LogManager>();
        }

        /// <summary>
        /// 打印调试信息
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="msg"></param>
        /// <param name="memberName"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void LogDebug(ELogTags tag, object msg, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            s_logManager.GenerateFormatLog(ELogLevel.Debug, tag, msg.ToString(), memberName, filePath, lineNumber);
        }

        /// <summary>
        /// 打印信息
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="msg"></param>
        /// <param name="memberName"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void LogInfo(ELogTags tag, object msg, [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            s_logManager.GenerateFormatLog(ELogLevel.Info, tag, msg.ToString(), memberName, filePath, lineNumber);
        }
        
        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="msgWarning"></param>
        /// <param name="memberName"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void LogWarning(ELogTags tag, object msgWarning, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            s_logManager.GenerateFormatLog(ELogLevel.Debug, tag, msgWarning.ToString(), memberName, filePath, lineNumber);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="msgError"></param>
        /// <param name="memberName"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void LogError(ELogTags tag, object msgError, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            s_logManager.GenerateFormatLog(ELogLevel.Error, tag, msgError.ToString(), memberName, filePath, lineNumber);
        }

        /// <summary>
        /// 打印异常
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="exception"></param>
        /// <param name="memberName"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static void LogException(ELogTags tag, Exception exception, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            s_logManager.GenerateExceptionFormatLog(tag, exception, memberName, filePath, lineNumber);
        }

        /// <summary>
        /// 为指定组合类型添加额外的调用堆栈日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="tag"></param>
        public static void AddExtraStaceStackForCombinedType(ELogLevel level, ELogTags tag)
        {
            s_logManager.AddExtraStaceStackForCombinedType(level, tag);
        }
    }
}

using System;

namespace Core.Exceptions
{
    /// <summary>
    /// UI相关异常
    /// </summary>
    public class UICreateException : ExceptionBase
    {
        /// <summary>
        /// UI控制器类型
        /// </summary>
        public Type UIType { get; }
        
        public UICreateException(Type uiType, int exceptionCode, string message, Exception inner) : base(exceptionCode, message, inner)
        {
            UIType = uiType;
        }
    }
}

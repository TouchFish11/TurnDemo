using System;

namespace Core.Exceptions
{
    /// <summary>
    /// 事件中心分发事件异常
    /// </summary>
    public class EventTriggerException : ExceptionBase
    {
        public EventTriggerException(int exceptionCode, string message, Exception inner) : base(exceptionCode, message, inner)
        {
        
        }
    }
}

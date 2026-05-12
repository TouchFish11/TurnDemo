using System;

namespace Core.Exceptions
{
    /// <summary>
    /// 自定义异常基类
    /// </summary>
    public abstract class ExceptionBase : Exception
    {
        public int ExceptionCode { get; private set; }
        
        protected ExceptionBase(int exceptionCode, string message, Exception inner) : base(message, inner)
        {
            ExceptionCode = exceptionCode;
        }
    }
}

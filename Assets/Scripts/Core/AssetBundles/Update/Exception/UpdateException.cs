using Core.Exceptions;

namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// 更新异常基类
    /// </summary>
    public abstract class UpdateException : ExceptionBase
    {
        protected UpdateException(string message, System.Exception inner = null) : base(-1, message, inner)
        {

        }
    }
}

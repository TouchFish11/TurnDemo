namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// 更新异常基类
    /// </summary>
    public abstract class UpdateException : System.Exception
    {
        protected UpdateException(string message) : base(message)
        {
            
        }
    }
}

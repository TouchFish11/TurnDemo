
namespace Core.Res
{
    /// <summary>
    /// 资源信息基类
    /// </summary>
    public abstract class BaseResourcesInfo
    {
        //引用计数
        protected uint _refCount;

        /// <summary>
        /// 引用计数
        /// </summary>
        public uint RefCount { get { return _refCount; } set { _refCount = value; } }
    }
}

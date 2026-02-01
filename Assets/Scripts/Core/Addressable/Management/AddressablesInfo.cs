#if DISABLE_ADDRESSABLES
    
#else
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Addressable.Management
{
    /// <summary>
    /// Addressables信息
    /// </summary>
    public sealed class  AddressablesInfo
    {
        /// <summary>
        /// 异步操作句柄
        /// </summary>
        public AsyncOperationHandle Handle { get; }
        
        /// <summary>
        /// 引用计数
        /// </summary>
        public uint RefCount { get; set; }

        public AddressablesInfo(AsyncOperationHandle handle)
        {
            this.Handle = handle;
            RefCount += 1;
        }
    }
}
#endif



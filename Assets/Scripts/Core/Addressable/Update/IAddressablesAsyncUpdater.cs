#if DISABLE_ADDRESSABLES

#else
using System;
using System.Threading.Tasks;

namespace Framework.Addressable.Update
{
    /// <summary>
    /// Addressables异步加载器接口
    /// </summary>
    public interface IAddressablesAsyncUpdater : IAddressablesUpdater
    {
        /// <summary>
        /// 异步检查更新
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
         Task CheckUpdateAsync(Action<UpdateCallbackData> callback);

        /// <summary>
        /// 异步更新资源
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
         Task UpdateAssetsAsync(Action<UpdateCallbackData> callback);
    }
}
#endif




using System.Collections.Generic;

namespace HotUpdate.Battle.Status
{
    /// <summary>
    /// 状态组件接口
    /// </summary>
    public interface IStatusComponent
    {
        /// <summary>
        /// 更新状态
        /// </summary>
        void UpdateStatus();

        /// <summary>
        /// 获取所有存活的状态
        /// </summary>
        /// <returns></returns>
        List<IStatus> GetStatuses();
    }
}

using System.Threading.Tasks;
using HotUpdate.Common.Config.Inventory;

namespace HotUpdate.UI.Inventory.State
{
    /// <summary>
    /// 背包界面状态接口
    /// </summary>
    public interface IInventoryState
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        /// <returns></returns>
        Task Enter();

        /// <summary>
        /// 当物品格子点击时执行
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task OnItemClick(Item item);
        
        /// <summary>
        /// 退出状态
        /// </summary>
        /// <returns></returns>
        Task Exit();
        
        /// <summary>
        /// 当物品栏刷新前优先调用
        /// </summary>
        void OnBeforeRefreshItem();
    }
}

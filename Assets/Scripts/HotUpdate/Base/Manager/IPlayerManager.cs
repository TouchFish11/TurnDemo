using System.Threading.Tasks;
using HotUpdate.Base.ECModule;

namespace HotUpdate.Base.Manager
{
    public interface IPlayerManager
    {
        /// <summary>
        /// 主玩家对象（固定UID为1001）
        /// </summary>
        IEntityObject CurrentEntity { get; }
        
        /// <summary>
        /// 清理玩家和相机
        /// </summary>
        void Clear();

        /// <summary>
        /// 创建玩家对象
        /// </summary>
        /// <param name="id"></param>
        Task CreatePlayer(int id);
    }
}

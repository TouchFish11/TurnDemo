using System.Threading.Tasks;
using HotUpdate.Base.Object;

namespace HotUpdate.Base.Manager
{
    public interface IPlayerManager
    {
        /// <summary>
        /// 主玩家对象（固定UID为1001）
        /// </summary>
        IEntityObject MainPlayer { get; }

        /// <summary>
        /// 创建玩家对象
        /// </summary>
        /// <param name="uid">玩家唯一标识</param>
        Task CreatePlayer(uint uid);
        
        /// <summary>
        /// 清理玩家和相机
        /// </summary>
        void Clear();
    }
}

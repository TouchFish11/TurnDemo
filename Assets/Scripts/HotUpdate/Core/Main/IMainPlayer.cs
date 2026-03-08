using Core.Components;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Core.Main
{
    public interface IMainPlayer : IEntityObject
    {
        /// <summary>
        /// 添加战斗实体到玩家的管理列表
        /// </summary>
        /// <param name="entityObject">待添加的战斗实体对象（实现IBattleEntityObject接口）</param>
        void AddEntity(IBattleEntityObject entityObject);
    }
}

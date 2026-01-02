using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Battle
{
    public interface IBattleContext
    {
        /// <summary>
        /// 当前战技点数
        /// </summary>
        int CurentBattlePointCount { get; }

        /// <summary>
        /// 最大战技点数
        /// </summary>
        int MaxBattlePointCount { get; }

        /// <summary>
        /// 获取所有战斗的实体
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBattleEntityObject> GetAllBattleEntity();

        /// <summary>
        /// 获取所有玩家角色实体
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBattleEntityObject> GetPlayerObjects();

        /// <summary>
        /// 获取所有怪物角色实体
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBattleEntityObject> GetMonsterObjects();

        /// <summary>
        /// 获取回合管理器
        /// </summary>
        /// <returns></returns>
        TurnController GetTurnManager();

        // TODO：封装目标获取逻辑，避免技能实例直接依赖管理器
        //IBattleEntityObject GetMainTaraget();
        //List<IBattleEntityObject> GetSelectedTargets();

        /// <summary>
        /// 初始化战斗
        /// </summary>
        Task InitBattle();

        /// <summary>
        /// 清理战斗
        /// </summary>
        void CleanupBattle();

        /// <summary>
        /// 获取战斗事件总线
        /// </summary>
        /// <returns></returns>
        BattleEventBus GetEventBus();

        void ConsumeSkillPoint(int cost);
        void ExpandSkillPoint(int cost);
        int GetPlayerObjectIndex(IBattleEntityObject battleEntity);
        int GetMonsterObjectIndex(IBattleEntityObject battleEntity);
    }
}

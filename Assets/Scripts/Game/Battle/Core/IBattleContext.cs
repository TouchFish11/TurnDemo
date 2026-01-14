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
        List<IBattleEntityObject> GetAllBattleEntity();

        /// <summary>
        /// 获取首个战斗实体
        /// </summary>
        /// <returns></returns>
        IBattleEntityObject GetFirstBattleEntity();

        /// <summary>
        /// 获取所有玩家角色实体
        /// </summary>
        /// <returns></returns>
        List<IBattleEntityObject> GetPlayerObjects();

        /// <summary>
        /// 获取存活的玩家角色实体
        /// </summary>
        /// <returns></returns>
        List<IBattleEntityObject> GetLivePlayerObjects();

        /// <summary>
        /// 获取所有怪物角色实体
        /// </summary>
        /// <returns></returns>
        List<IBattleEntityObject> GetMonsterObjects();

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

        /// <summary>
        /// 消耗战机点
        /// </summary>
        /// <param name="cost"></param>
        void ConsumeSkillPoint(int cost);

        /// <summary>
        /// 拓展最大战机点
        /// </summary>
        /// <param name="cost"></param>
        void ExpandSkillPoint(int cost);

        /// <summary>
        /// 获取玩家角色在行动轴的位置
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <returns></returns>
        int GetPlayerObjectIndex(IBattleEntityObject battleEntity);

        /// <summary>
        /// 获取怪物在行动轴的位置
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <returns></returns>
        int GetMonsterObjectIndex(IBattleEntityObject battleEntity);

        /// <summary>
        /// 获取当前行动实体
        /// </summary>
        /// <returns></returns>
        IBattleEntityObject GetCurrentEntity();

        /// <summary>
        /// 设置当前行动实体
        /// </summary>
        /// <param name="battleEntity"></param>
        void SetCurrentEntity(IBattleEntityObject battleEntity);

        /// <summary>
        /// 获取所有存活的实体
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBattleEntityObject> GetLiveEntitys();

        /// <summary>
        /// 获取所有死亡实体
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBattleEntityObject> GetDeadEntitys();

        /// <summary>
        /// 获取所有死亡的怪物
        /// </summary>
        /// <returns></returns>
        List<IBattleEntityObject> GetDeadMonsterEntitys();

        /// <summary>
        /// 获取所有存活的怪物实体
        /// </summary>
        /// <returns></returns>
        List<IBattleEntityObject> GetLiveMonsterObjects();
    }
}

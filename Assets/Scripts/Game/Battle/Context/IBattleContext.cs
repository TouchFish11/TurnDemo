using System;
using System.Collections.Generic;
using Game.Battle.Event;
using Game.Battle.Objects;
using Game.Battle.Turn;

namespace Game.Battle.Context
{
    /// <summary>
    /// 战斗上下文接口
    /// 定义战斗场景的核心上下文能力，包含战斗点数管理、实体管理、回合控制、事件总线等核心功能
    /// </summary>
    public interface IBattleContext
    {
        /// <summary>
        /// 获取当前剩余的战斗点数（技能点/行动点）
        /// </summary>
        int CurentBattlePointCount { get; }

        /// <summary>
        /// 获取战斗点数的最大上限值
        /// </summary>
        int MaxBattlePointCount { get; }
        
        /// <summary>
        /// 获取首个战斗实体（通常用于初始化或默认目标）
        /// </summary>
        /// <returns>首个战斗实体对象</returns>
        IBattleEntityObject GetFirstBattleEntity();

        /// <summary>
        /// 获取所有存活的玩家角色实体集合
        /// </summary>
        /// <returns>存活的玩家角色实体枚举集合</returns>
        IEnumerable<IBattleEntityObject> GetAlivePlayerEntitys();

        /// <summary>
        /// 获取所有存活的怪物角色实体集合
        /// </summary>
        /// <returns>存活的怪物角色实体枚举集合</returns>
        IEnumerable<IBattleEntityObject> GetAliveMonsterEntitys();

        /// <summary>
        /// 获取回合控制器（管理回合顺序、回合切换等逻辑）
        /// </summary>
        /// <returns>回合控制器实例</returns>
        ITurnController GetTurnManager();

        /// <summary>
        /// 清理战斗资源
        /// 战斗结束后释放资源、清空实体、注销事件等收尾操作
        /// </summary>
        void CleanupBattle();

        /// <summary>
        /// 获取战斗事件总线（用于战斗内事件的发布/订阅）
        /// </summary>
        /// <returns>战斗事件总线实例</returns>
        IBattleEventBus GetEventBus();

        /// <summary>
        /// 消耗战斗点数（技能点/行动点）
        /// </summary>
        /// <param name="cost">需要消耗的点数数量</param>
        void ConsumeSkillPoint(int cost);

        /// <summary>
        /// 扩充战斗点数上限
        /// </summary>
        /// <param name="cost">扩充的点数数量（注：参数命名为cost为"增量"）</param>
        void ExpandSkillPoint(int cost);

        /// <summary>
        /// 获取指定玩家角色实体在玩家列表中的索引
        /// </summary>
        /// <param name="battleEntity">目标玩家角色实体</param>
        /// <returns>实体对应的索引值；若不存在则返回-1（业务约定）</returns>
        int GetPlayerEntityIndex(IBattleEntityObject battleEntity);

        /// <summary>
        /// 获取指定怪物实体在怪物列表中的索引
        /// </summary>
        /// <param name="battleEntity">目标怪物实体</param>
        /// <returns>实体对应的索引值；若不存在则返回-1（业务约定）</returns>
        int GetMonsterEntityIndex(IBattleEntityObject battleEntity);

        /// <summary>
        /// 获取当前处于行动回合的战斗实体
        /// </summary>
        /// <returns>当前行动的实体；若无则返回null</returns>
        IBattleEntityObject GetCurrentEntity();

        /// <summary>
        /// 设置当前处于行动回合的战斗实体
        /// </summary>
        /// <param name="battleEntity">要设置为当前行动的实体</param>
        void SetCurrentEntity(IBattleEntityObject battleEntity);

        /// <summary>
        /// 获取所有存活的战斗实体（包含玩家和怪物）
        /// </summary>
        /// <returns>存活的战斗实体枚举集合</returns>
        IEnumerable<IBattleEntityObject> GetAliveEntitys();

        /// <summary>
        /// 获取所有已死亡的战斗实体（包含玩家和怪物）
        /// </summary>
        /// <returns>已死亡的战斗实体枚举集合</returns>
        IEnumerable<IBattleEntityObject> GetDeadEntitys();

        /// <summary>
        /// 获取所有已死亡的怪物实体
        /// </summary>
        /// <returns>已死亡的怪物实体枚举集合</returns>
        IEnumerable<IBattleEntityObject> GetDeadMonsterEntitys();

        /// <summary>
        /// 获取所有已死亡的玩家角色实体
        /// </summary>
        /// <returns>已死亡的玩家角色实体枚举集合</returns>
        IEnumerable<IBattleEntityObject> GetDeadPlayerEntitys();
        
        /// <summary>
        /// 获取下一个即将行动的战斗实体（基于回合顺序）
        /// </summary>
        /// <returns>下一个行动的实体；若无则返回null</returns>
        IBattleEntityObject GetNextEntity();
        
        /// <summary>
        /// 对战斗实体列表进行排序
        /// </summary>
        /// <param name="comparison">排序比较器，定义实体间的排序规则</param>
        void Sort(Comparison<IBattleEntityObject> comparison);
        
        /// <summary>
        /// 添加战斗实体到上下文管理中
        /// </summary>
        /// <param name="battleEntity">要添加的战斗实体</param>
        void AddBattleEntity(IBattleEntityObject battleEntity);
        
        /// <summary>
        /// 从上下文管理中移除指定战斗实体
        /// </summary>
        /// <param name="battleEntity">要移除的战斗实体</param>
        /// <returns>移除成功返回true；实体不存在返回false</returns>
        bool RemoveBattleEntity(IBattleEntityObject battleEntity);
        
        /// <summary>
        /// 获取指定战斗实体在全局实体列表中的索引
        /// </summary>
        /// <param name="battleEntity">目标战斗实体</param>
        /// <returns>实体对应的索引值；若不存在则返回-1（业务约定）</returns>
        int GetEntityIndex(IBattleEntityObject battleEntity);
        
        /// <summary>
        /// 在指定索引位置插入战斗实体
        /// </summary>
        /// <param name="index">要插入的目标索引</param>
        /// <param name="battleEntityObject">要插入的战斗实体</param>
        void Insert(int index, IBattleEntityObject battleEntityObject);
    }
}
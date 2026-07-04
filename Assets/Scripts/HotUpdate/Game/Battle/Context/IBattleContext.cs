using System;
using System.Collections.Generic;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.StateMeachine;
using HotUpdate.Game.Battle.Turn;

namespace HotUpdate.Game.Battle.Context
{
    /// <summary>
    /// 战斗上下文接口
    /// 定义战斗场景的核心上下文能力，包含战斗点数管理、实体管理、回合控制、事件总线等核心功能
    /// </summary>
    public interface IBattleContext
    {
        /// <summary>
        /// 获取当前剩余的战技点
        /// </summary>
        int CurentBattlePointCount { get; }

        /// <summary>
        /// 获取战技点的最大上限值
        /// </summary>
        int MaxBattlePointCount { get; }
        
        /// <summary>
        /// 持有当前行动回合的实体，不受终结技、追击等“插队”逻辑的影响
        /// </summary>
        IBattleEntityObject CurrentTurnOwner { get; }

        /// <summary>
        /// 行动基准线
        /// </summary>
        float ActionLine { get; set; }

        /// <summary>
        /// 战斗指令列表，按优先级存储待执行的战斗指令
        /// </summary>
        List<ICommand> BattleCommands { get; }

        /// <summary>
        /// 当前正在执行的指令，可能会为null
        /// </summary>
        ICommand CurrentCommand { get; set; }

        /// <summary>
        /// 战斗局部事件总线
        /// </summary>
        BattleEventBus EventBus { get; }
        
        /// <summary>
        /// 战斗状态机
        /// </summary>
        IBattleStateMachine BattleMachine { get; }

        /// <summary>
        /// 战斗实体总列表
        /// </summary>
        List<IBattleEntityObject> AllBattleEntity { get; }

        /// <summary>
        /// 场景怪物列表
        /// </summary>
        List<IBattleEntityObject> SceneMonsterObjects { get; }

        /// <summary>
        /// 场景玩家列表
        /// </summary>
        List<IBattleEntityObject> SceneRoleObjects { get; }

        /// <summary>
        /// 初始化战斗上下文
        /// </summary>
        /// <param name="eventBus"></param>
        /// <param name="battleStateMachine"></param>
        void Init(BattleEventBus eventBus, BattleStateMachine battleStateMachine);
        
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
        /// 消耗战技点
        /// </summary>
        /// <param name="cost">需要消耗的点数数量</param>
        void ConsumeSkillPoint(int cost);

        /// <summary>
        /// 扩充战技点上限
        /// </summary>
        /// <param name="cost">扩充的点数数量（注：参数命名为cost为"增量"）</param>
        void ExpandSkillPoint(int cost);
        
        /// <summary>
        /// 获取所有存活的战斗实体（包含玩家和怪物）
        /// </summary>
        /// <returns>存活的战斗实体枚举集合</returns>
        IEnumerable<IBattleEntityObject> GetAliveEntitys();
        
        /// <summary>
        /// 设置持有当前回合的行动实体
        /// </summary>
        /// <param name="battleEntityObject"></param>
        void SetCurrentTurnOwner(IBattleEntityObject battleEntityObject);

        /// <summary>
        /// 清理战斗数据
        /// </summary>
        void CleanData();
    }
}
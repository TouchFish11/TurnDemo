using System.Collections;
using Core.Components;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 战斗实体对象接口
    /// 定义所有战斗实体（角色、怪物、NPC等）必须实现的核心战斗行为和属性
    /// </summary>
    public interface IBattleEntityObject : IEntityObject
    {
        /// <summary>
        /// 子游戏物体
        /// 通常指向战斗实体挂载核心逻辑的子GameObject（如表现层、碰撞体载体）
        /// 需保证返回的是该实体下第一个有效子物体
        /// </summary>
        GameObject SubGameObject { get; }

        /// <summary>
        /// 战斗实体唯一ID
        /// 用于战斗场景内区分不同实体，与全局实体ID区分
        /// </summary>
        int BattleEntityId { get; }

        /// <summary>
        /// 实体位置索引
        /// 标识实体在战斗阵型的位置编号
        /// </summary>
        int EntityPosIndex { get; set; }

        /// <summary>
        /// 是否死亡状态
        /// true=已死亡，false=存活
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// 是否可执行行动
        /// 用于判断实体当前是否具备行动能力（如未被眩晕、冻结、死亡）
        /// </summary>
        bool CanAct { get; set; }

        /// <summary>
        /// 战斗上下文
        /// 指向当前所属的战斗场景上下文，用于获取战斗全局数据（如战斗管理器、其他实体）
        /// </summary>
        IBattleContext Context { get; }

        /// <summary>
        /// 行动值
        /// 战斗回合制中用于判定行动顺序的核心数值（行动值满则可执行行动）
        /// </summary>
        float ActionValue { get; }

        /// <summary>
        /// 设置行动值
        /// 直接修改当前实体的行动值（如加速/减速效果、回合重置）
        /// </summary>
        /// <param name="actionValue">目标行动值</param>
        void SetActionValue(float actionValue);

        /// <summary>
        /// 战斗初始化
        /// 战斗开始时初始化实体的核心战斗数据
        /// </summary>
        /// <param name="id">战斗实体ID</param>
        /// <param name="context">当前战斗上下文</param>
        void BattleInit(int id, IBattleContext context);

        /// <summary>
        /// 执行行动
        /// 触发实体的核心行动逻辑（如普攻、释放技能、移动）
        /// 由战斗管理器在行动阶段调用
        /// </summary>
        void ExecuteAction();

        /// <summary>
        /// 受到治疗
        /// 为实体增加生命值（不会超过最大生命值）
        /// </summary>
        /// <param name="value">治疗量（正数）</param>
        void TakeHeal(int value);

        /// <summary>
        /// 受到伤害
        /// 处理实体受到的伤害计算、扣血、伤害表现等逻辑
        /// </summary>
        /// <param name="damageResult">伤害结算结果对象（包含伤害类型、数值、来源等）</param>
        /// <returns>是否成功承受伤害（true=成功，false=免疫/格挡等未生效）</returns>
        void TakeDamage(DamageResult damageResult);

        /// <summary>
        /// 死亡处理协程
        /// 实现实体死亡的完整逻辑（如播放死亡动画、移除战斗状态、通知战斗管理器）
        /// 使用协程可处理异步动画/音效等耗时操作
        /// </summary>
        /// <returns>迭代器用于协程执行</returns>
        IEnumerator Die();
        
        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillId"></param>
        void CastSkill(int skillId);

        /// <summary>
        /// 提供护盾
        /// </summary>
        /// <param name="sheildAmount">护盾量</param>
        void TakeSheild(int sheildAmount);
    }
}
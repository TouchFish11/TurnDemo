using Framework;
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 角色韧性组件（管理目标的韧性系统）
    /// </summary>
    public class ToughnessComponent : BattleComponent, IToughnessComponent
    {
        // 当前韧性状态
        private Toughness _toughness;

        public void Init(IBattleEntityObject owner, List<E_PropertyType> weakPropertys, float initialToughness)
        {
            _toughness = new Toughness(weakPropertys, initialToughness);
            // 订阅“技能释放事件”（监听所有技能释放，计算韧性）
            BattleEventCenter.AddListener<SkillCastEvent>(OnSkillCastHandler);
        }

        /// <summary>
        /// 事件回调：技能释放后，计算韧性伤害
        /// </summary>
        /// <param name="evt"></param>
        private void OnSkillCastHandler(SkillCastEvent skillCastEvent)
        {
            // 只处理当前组件所属角色的韧性（避免处理其他角色）
            if (!skillCastEvent.Contain(EntityObject as IBattleEntityObject))
            {
                return;
            }

            // 技能对韧性造成削减（调用韧性API）
            _toughness.ReduceToughness(skillCastEvent.PropertyType, 25);

            // 若韧性为0且未触发过破盾（防止重复触发）
            if (_toughness.IsBroken)
            {
                LogMgr.Log($"\n{(EntityObject as IBattleEntityObject).Name}被击破！");

                // 广播“破盾事件”（通知其他模块“目标已破盾”）
                BattleEventCenter.TriggerEvent(new ToughnessBrokenEvent(skillCastEvent.Context, skillCastEvent.Caster, EntityObject as IBattleEntityObject));
            }
        }

        /// <summary>
        /// 获取当前韧性状态
        /// </summary>
        /// <returns></returns>
        public bool IsToughnessBroken() => _toughness.IsBroken;
    }
}

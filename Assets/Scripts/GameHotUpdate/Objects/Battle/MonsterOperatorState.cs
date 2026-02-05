using System.Collections;
using System.Collections.Generic;
using Game.Battle.Objects;
using Game.Battle.Skill.Component;
using UnityEngine;

namespace GameHotUpdate.Objects.Battle
{
    public class MonsterOperatorState : OperatorState
    {
        /// <summary>
        /// 怪物可释放的技能ID列表
        /// </summary>
        private List<int> skillIds;
        
        public MonsterOperatorState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            skillIds = BattleEntity.GetComponent<SkillComponent>().GetSkillIds();
            base.Enter();
        }

        /// <summary>
        /// 怪物行动逻辑的核心协程
        /// 该方法为怪物AI的入口
        /// </summary>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnExceuteAction()
        {
            // 封装韧性恢复和怪物技能释放指令，执行指令时先判断是否恢复韧性条，再处理技能释放
            
            var skillComponent = BattleEntity.GetComponent<SkillComponent>();
            
            // 随机从技能列表中选择一个技能ID
            // TODO：可以封装随机选择的策略类，用于玩家/怪物AI
            var skillId = skillIds[Random.Range(0, skillIds.Count)];
            // 释放选中的技能
            skillComponent.CastSkill(skillId);
            // 行动结束
            BattleEntity.CanAct = false;
            yield break;
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {

        }
    }
}

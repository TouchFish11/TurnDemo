using GameLogic.BattleMoudule.Core;
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Skill
{
    /// <summary>
    /// 角色技能组件（管理角色技能，提供释放入口）
    /// </summary>
    public class SkillComponent : MonoBehaviour, ISkillComponent
    {
        private IBattleEntity _owner;

        // 技能列表（配置表加载）
        private Dictionary<int, ISkill> _skills = new Dictionary<int, ISkill>();

        public void Init(IBattleEntity owner)
        {
            _owner = owner;
            // 从配置表加载技能（示例：加载ID=1的弱点攻击技能）
            _skills.Add(1, new WeakPointAttackSkill("穿刺射击", 1.5f, E_PropertyType.Physical));
            // 添加召唤技能（配置表加载）
            this.AddSkill(2, new SummonMimiSkill());
        }

        /// <summary>
        /// 释放指定ID的技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="context"></param>
        /// <param name="target"></param>
        public void CastSkill(int skillId, IBattleContext context, List<IBattleEntity> targets)
        {
            if (_skills.TryGetValue(skillId, out ISkill skill))
            {
                skill.Cast(context, _owner, targets);
            }
        }

        /// <summary>
        /// 添加指定ID的技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="newSkill"></param>
        public void AddSkill(int skillId, ISkill newSkill)
        {
            if (!_skills.TryGetValue(skillId, out ISkill _))
            {
                _skills.Add(skillId, newSkill);
            }
        }
    }
}

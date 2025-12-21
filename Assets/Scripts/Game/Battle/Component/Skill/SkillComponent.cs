using Framework;
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 角色技能组件（管理角色技能，提供释放入口）
    /// </summary>
    public class SkillComponent : BattleComponent, ISkillComponent
    {
        // 技能列表（配置表加载）  可能这个组件只有技能Id列表就可以了
        private readonly Dictionary<int, ISkill> _skills = new Dictionary<int, ISkill>();

        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);
            
            // TODO：通过技能工厂加载技能（配置表读取角色技能ID列表）
            int[] skillIds = TextUtility.SplitToIntArr((this.BattleEntity as PlayerObject).RoleInfo.f_skillIds, 2);
            foreach (int skillId in skillIds)
            {
                switch (skillId)
                {
                    case 10:
                        // 从配置表加载技能（示例：加载ID=1的弱点攻击技能）
                        _skills.Add(skillId, new WeakPointAttackSkill());
                        break;
                    case 11:
                        // 添加召唤技能（配置表加载）
                        this.AddSkill(skillId, new SummonMimiSkill());
                        break;
                }
            }
        }

        /// <summary>
        /// 释放指定ID的技能
        /// </summary>
        /// <param name="skillId"></param>
        public void CastSkill(int skillId)
        {
            if (_skills.TryGetValue(skillId, out var _))
            {
                SkillInfo skillInfo = BinaryDataMgr.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[skillId];
                // 发送技能命令到回合队列
                SkillManager.Instance.AddSkillCommand(skillInfo, (this.BattleEntity as PlayerObject).RoleInfo);
            }
            else
            {
                LogManager.Log($"未找到技能ID， skillId = {skillId}");
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

        /// <summary>
        /// 获取所有的技能ID
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetSkillIds()
        {
            return _skills.Keys;
        }
    }
}

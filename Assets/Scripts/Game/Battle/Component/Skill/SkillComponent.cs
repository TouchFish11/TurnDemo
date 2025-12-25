using Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace Game.Battle
{
    /// <summary>
    /// 战斗实体技能组件（管理实体技能，提供释放入口）
    /// </summary>
    public class SkillComponent : BattleComponent, ISkillComponent
    {
        // 技能列表（配置表加载）  可能这个组件只有技能Id列表就可以了
        private readonly Dictionary<int, ISkill> _skills = new Dictionary<int, ISkill>();
        // 技能工厂接口
        private ISkillFactory skillFactory;

        /// <summary>
        /// 初始化技能列表
        /// </summary>
        /// <param name="f_skillIds"></param>
        public void InitSkills(string f_skillIds, ISkillFactory skillFactory)
        {
            this.skillFactory = skillFactory;
            // TODO：通过技能工厂加载技能（配置表读取角色技能ID列表）
            int[] skillIds = TextUtility.SplitToIntArr(f_skillIds, 2);
            var skills = skillFactory.CreateSkills(skillIds);

            foreach (ISkill skill in skills)
            {
                _skills.Add(skill.SkillInfo.f_id, skill);
            }
        }

        /// <summary>
        /// 释放指定ID的技能
        /// </summary>
        /// <param name="skillId"></param>
        public void CastSkill(int skillId)
        {
            if (_skills.TryGetValue(skillId, out var skill))
            {
                E_SkillType skillType = skill.SkillInfo.f_SkillType.ToSkillType();
                // 玩家释放
                if (skillType != E_SkillType.Monster)
                {
                    PlayerCastSkill(skill);
                }
                // 怪物释放
                else
                {
                    MonsterCastSkill(skill);
                }
            }
            else
            {
                LogManager.Log($"未找到技能实例， skillId = {skillId}");
            }
        }

        /// <summary>
        /// 能否释放
        /// TODO：暂时这样写，之后优化，因为怪物/玩家角色共用一个技能组件，所以会有判断，之后可能独立成两个组件
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        private bool CanCast(ISkill skill)
        {
            switch (skill.SkillInfo.f_SkillType.ToSkillType())
            {
                case E_SkillType.Monster:
                    return true;
                case E_SkillType.NormalAttack:
                case E_SkillType.CombatSkill:
                    int tempBP = this.BattleEntity.Context.CurentBattlePointCount;
                    if (tempBP - skill.SkillInfo.f_costBP >= 0)
                    {
                        //LogManager.Log($"释放技能，消耗战技点：{skill.SkillInfo.f_costBP}");
                        return true;
                    }
                    else
                    {
                        LogManager.Log("战技点不足，无法释放该技能");
                        return false;
                    }
                case E_SkillType.UltimateSkill:
                    // 若为终结技，需判断能量是否足够
                    RoleProperty playerProperty = this.BattleEntity.GetComponent<PropertyComponent>().GetProperty<RoleProperty>();
                    if (playerProperty.CurrentEnergy == playerProperty.BaseEnergy)
                    {
                        return true;
                    }
                    else
                    {
                        // 提示玩家能量不足
                        LogManager.Log("能量不足，无法释放终结技");
                        return false;
                    }
                case E_SkillType.EnhancedNormalAttack:
                case E_SkillType.EnhancedCombatSkill:
                    return true;
                default:
                    return false;
            }
        }

        private void PlayerCastSkill(ISkill skill)
        {
            if (!CanCast(skill))
            {
                return;
            }

            // 发送技能命令到回合队列
            SkillManager.Instance.AddSkillCommand(skill, this.BattleEntity);
        }

        private void MonsterCastSkill(ISkill skill)
        {
            // 发送技能命令到回合队列
            SkillManager.Instance.AddSkillCommand(skill, this.BattleEntity);
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

        /// <summary>
        /// 获取所有的技能
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISkill> GetSkills()
        {
            return _skills.Values;
        }
    }
}

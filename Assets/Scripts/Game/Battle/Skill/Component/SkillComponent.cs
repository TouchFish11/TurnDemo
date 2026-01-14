using Framework;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 战斗实体技能组件
    /// 管理实体技能，提供释放入口
    /// </summary>
    public abstract class SkillComponent : BattleComponent, ISkillComponent
    {
        // 技能列表（配置表加载）  可能这个组件只有技能Id列表就可以了
        protected readonly Dictionary<int, ISkill> skills = new Dictionary<int, ISkill>();
        // 技能释放条件列表
        protected List<ICastSkillCondition> castSkillConditions = new List<ICastSkillCondition>();
        // 技能目标选择策略列表
        protected List<ITargetSelectStrategy> targetSelectStrategies = new List<ITargetSelectStrategy>();

        // 技能工厂接口
        protected ISkillFactory skillFactory;

        public abstract bool IsRelease { get; protected set; }

        /// <summary>
        /// 初始化技能列表
        /// </summary>
        /// <param name="f_skillIds"></param>
        public void InitSkills(string f_skillIds, ISkillFactory skillFactory)
        {
            this.skillFactory = skillFactory;
            // 通过技能工厂加载技能（配置表读取角色技能ID列表）
            int[] skillIds = TextUtility.SplitToIntArr(f_skillIds, 2);
            var skills = skillFactory.CreateSkills(this.BattleEntity, skillIds);

            foreach (ISkill skill in skills)
            {
                this.skills.Add(skill.SkillInfo.f_id, skill);
            }
        }

        /// <summary>
        /// 释放指定ID的技能
        /// </summary>
        /// <param name="skillId"></param>
        public void CastSkill(int skillId)
        {
            if (skills.TryGetValue(skillId, out var skill))
            {
                if (!CanCast(skill))
                {
                    return;
                }

                IsRelease = false;

                // 发送技能命令到回合队列
                skill.SetTargetSelectStrategy(targetSelectStrategies[0]);
                SkillManager.Instance.AddSkillCommand(skill);
            }
            else
            {
                LogManager.LogError($"未找到技能实例， skillId = {skillId}");
            }
        }

        /// <summary>
        /// 能否释放
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        protected bool CanCast(ISkill skill)
        {
            foreach (ICastSkillCondition condition in castSkillConditions)
            {
                if (!condition.CanCast(this.BattleEntity, skill))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 添加指定ID的技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="newSkill"></param>
        public void AddSkill(int skillId, ISkill newSkill)
        {
            if (!skills.TryGetValue(skillId, out ISkill _))
            {
                skills.Add(skillId, newSkill);
            }
        }

        /// <summary>
        /// 添加释放条件
        /// </summary>
        /// <param name="castSkillCondition"></param>
        public void AddCastCondition(ICastSkillCondition castSkillCondition)
        {
            if (!castSkillConditions.Contains(castSkillCondition))
            {
                castSkillConditions.Add(castSkillCondition);
            }
        }

        /// <summary>
        /// 移除释放条件
        /// </summary>
        /// <param name="castSkillCondition"></param>
        public void RemoveCastCondition(ICastSkillCondition castSkillCondition)
        {
            castSkillConditions.Remove(castSkillCondition);
        }

        /// <summary>
        /// 添加目标选择策略
        /// </summary>
        /// <param name="targetSelectStrategy"></param>
        public void AddTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            targetSelectStrategies.Add(targetSelectStrategy);
            SortTargetStratgy();
        }

        /// <summary>
        /// 移除目标选择策略
        /// </summary>
        /// <param name="targetSelectStrategy"></param>
        public void RemoveTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            targetSelectStrategies.Remove(targetSelectStrategy);
            SortTargetStratgy();
        }

        /// <summary>
        /// 排序目标选择策略
        /// </summary>
        private void SortTargetStratgy()
        {
            targetSelectStrategies.Sort((s1, s2) =>
            {
                if (s1.Priority > s2.Priority)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
        }

        /// <summary>
        /// 获取所有的技能ID
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetSkillIds()
        {
            return skills.Keys;
        }

        /// <summary>
        /// 获取所有的技能
        /// </summary>
        /// <returns></returns>
        public List<ISkill> GetSkills()
        {
            return new List<ISkill>(skills.Values);
        }
    }
}

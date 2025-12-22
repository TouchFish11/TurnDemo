using Framework;
using System.Collections.Generic;

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
                // 发送技能命令到回合队列
                SkillManager.Instance.AddSkillCommand(skill, this.BattleEntity);
            }
            else
            {
                LogManager.Log($"未找到技能实例， skillId = {skillId}");
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

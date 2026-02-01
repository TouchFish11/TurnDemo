using System.Collections.Generic;
using Core.Log;
using Core.Utility;
using Game.Battle.Component;
using Game.Battle.Skill.Base;
using Game.Battle.Skill.Condition;
using Game.Battle.Skill.Interface;
using Game.Battle.TargetSelect;

namespace Game.Battle.Skill.Component
{
    /// <summary>
    /// ս��ʵ�弼�����
    /// ����ʵ�弼�ܣ��ṩ�ͷ����
    /// </summary>
    public abstract class SkillComponent : BattleComponent, ISkillComponent
    {
        // �����б������ñ����أ�  ����������ֻ�м���Id�б��Ϳ�����
        protected readonly Dictionary<int, ISkill> skills = new();
        // �����ͷ������б�
        protected readonly List<ICastSkillCondition> castSkillConditions = new();
        // ����Ŀ��ѡ������б�
        protected readonly List<ITargetSelectStrategy> targetSelectStrategies = new();
        
        public abstract bool IsRelease { get; protected set; }

        /// <summary>
        /// ��ʼ�������б�
        /// </summary>
        /// <param name="f_skillIds"></param>
        /// <param name="skillFactory"></param>
        public void InitSkills(string f_skillIds, ISkillFactory skillFactory)
        {
            // ͨ�����ܹ������ؼ��ܣ����ñ���ȡ��ɫ����ID�б���
            var skillIds = TextUtility.SplitToIntArr(f_skillIds, 2);
            var skills = skillFactory.CreateSkills(BattleEntity, skillIds);

            foreach (var skill in skills)
            {
                this.skills.Add(skill.SkillInfo.f_id, skill);
            }
        }

        /// <summary>
        /// �ͷ�ָ��ID�ļ���
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

                // ���ͼ�������غ϶���
                skill.SetTargetSelectStrategy(targetSelectStrategies[0]);
                SkillManager.Instance.AddSkillCommand(skill);
            }
            else
            {
                LogManager.LogError($"δ�ҵ�����ʵ���� skillId = {skillId}");
            }
        }

        /// <summary>
        /// �ܷ��ͷ�
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        protected bool CanCast(ISkill skill)
        {
            foreach (ICastSkillCondition condition in castSkillConditions)
            {
                if (!condition.CanCast(BattleEntity, skill))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// ����ָ��ID�ļ���
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
        /// �����ͷ�����
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
        /// �Ƴ��ͷ�����
        /// </summary>
        /// <param name="castSkillCondition"></param>
        public void RemoveCastCondition(ICastSkillCondition castSkillCondition)
        {
            castSkillConditions.Remove(castSkillCondition);
        }

        /// <summary>
        /// ����Ŀ��ѡ�����
        /// </summary>
        /// <param name="targetSelectStrategy"></param>
        public void AddTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            targetSelectStrategies.Add(targetSelectStrategy);
            SortTargetStratgy();
        }

        /// <summary>
        /// �Ƴ�Ŀ��ѡ�����
        /// </summary>
        /// <param name="targetSelectStrategy"></param>
        public void RemoveTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            targetSelectStrategies.Remove(targetSelectStrategy);
            SortTargetStratgy();
        }

        /// <summary>
        /// ����Ŀ��ѡ�����
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
        /// ��ȡ���еļ���ID
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetSkillIds()
        {
            return skills.Keys;
        }

        /// <summary>
        /// ��ȡ���еļ���
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISkill> GetSkills()
        {
            foreach (var skill in skills.Values)
            {
                yield return skill;
            }
        }
    }
}

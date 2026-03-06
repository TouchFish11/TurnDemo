using System.Collections.Generic;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Component;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.UI.SkillKey.Provider
{
    /// <summary>
    /// 基础技能按键UI数据提供器
    /// 实现ISkillKeyUIDataProvider接口，用于为战斗界面的技能按键提供基础的UI展示数据
    /// </summary>
    public class BaseSkillKeyUIDataProvider : ISkillKeyUIDataProvider
    {
        /// <summary>
        /// 获取技能按键UI展示所需的数据
        /// </summary>
        /// <param name="provider">战斗实体对象（如角色、怪物等），作为技能数据的提供方</param>
        /// <returns>封装好的技能按键UI数据对象</returns>
        public SkillKeyUIData GetData(IBattleEntityObject provider)
        {
            // 初始化技能按键UI数据，传入空的技能信息列表和数据提供方实体
            var skillKeyUIData = new SkillKeyUIData(new List<SkillInfo>(), provider);
            // 从战斗实体中获取技能组件，并提取所有技能列表
            var skills = new List<ISkill>(provider.GetComponent<SkillComponent>().GetSkills());

            // 遍历所有技能，筛选非终极技能的技能信息加入UI数据
            foreach (var skill in skills)
            {
                // 获取当前技能的基础信息
                var skillInfo = skill.SkillInfo;
                // 过滤掉终极技能（终极技能不展示在基础技能按键UI中）
                if ((E_SkillType)skillInfo.f_SkillType == E_SkillType.UltimateSkill)
                {
                    continue;
                }

                // 将非终极技能的信息添加到UI数据的技能信息列表中
                skillKeyUIData.SkillInfos.Add(skillInfo);
            }

            // 返回组装好的技能按键UI数据
            return skillKeyUIData;
        }
    }
}
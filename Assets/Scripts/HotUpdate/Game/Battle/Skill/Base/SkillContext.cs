using System.Collections.Generic;
using Core.Utility;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能上下文
    /// </summary>
    public class SkillContext
    {
        /// <summary>
        /// 技能释放者（释放该技能的战斗实体，如角色、怪物）
        /// </summary>
        public IBattleEntityObject Caster { get; private set; }
        
        /// <summary>
        /// 技能配置信息（从配置表加载的技能基础属性）
        /// </summary>
        public SkillInfo SkillInfo { get; private set; }
        
        /// <summary>
        /// 技能附带的Buff/状态ID数组
        /// </summary>
        public int[] StatusIds { get; private set; }
        
        /// <summary>
        /// 释放者的属性组件（用于读取/修改释放者的属性，如攻击力、能量等）
        /// </summary>
        public IPropertyComponent PropertyComponent { get; private set; }
        
        /// <summary>
        /// 技能释放后置处理器（处理技能释放完成后的附加逻辑）
        /// </summary>
        public ISkillCastPostHandler SkillCastPostHandler { get; set; }

        /// <summary>
        /// 目标选择策略（定义技能如何选择作用目标）
        /// </summary>
        public ITargetSelectStrategy TargetSelectStrategy { get; set; }
        
        /// <summary>
        /// 投射物数据
        /// </summary>
        public ProjectileData ProjectileData {get; set;}
        
        /// <summary>
        /// 投射物变换数据（控制投射物的位置/旋转等）
        /// </summary>
        public ProjectileTrans ProjectileTrans {get; set;}
        
        /// <summary>
        /// 视觉特效信息（技能特效的配置数据）
        /// </summary>
        public VFXInfo VFXInfo { get; set; }
        
        /// <summary>
        /// 技能主要目标（技能优先作用的单个目标）
        /// </summary>
        public IBattleEntityObject MainTarget { get; set; }

        /// <summary>
        /// 技能所有目标（技能作用的全部目标列表，含主要目标）
        /// </summary>
        public List<IBattleEntityObject> AllTargets { get; set; }
        
        /// <summary>
        /// 弹射物对象
        /// </summary>
        public IProjectile Projectile { get; set; }

        public SkillContext(IBattleEntityObject caster, SkillInfo skillInfo, IPropertyComponent propertyComponent)
        {
            Caster = caster;
            SkillInfo = skillInfo;
            // 解析技能配置中的状态ID（分割字符串为int数组，分隔符为2？注：此处需确认分割规则，2为自定义分隔符标识）
            StatusIds = TextUtility.SplitToIntArr(skillInfo.f_statusId, 2);
            PropertyComponent = propertyComponent;
        }
    }
}

using System.Collections.Generic;
using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能工厂
    /// </summary>
    public abstract class SkillFactory : ISkillFactory
    {
        [Inject] protected SkillNodeBuildPipeline SkillNodeBuildPipeline;
         
        /// <summary>
        /// 技能构建数据
        /// </summary>
        public readonly struct SKillBuildData
        {
            public ISkillCastPostHandler SkillCastPostHandler { get; }
            
            public List<ISkillNode> Effects { get; }

            public SKillBuildData(ISkillCastPostHandler skillCastPostHandler, List<ISkillNode> effects)
            {
                SkillCastPostHandler = skillCastPostHandler;
                Effects = effects;
            }
        }
        
        [Inject] protected IBinaryDataManager binaryDataManager;
        [Inject] protected ISkillCastPostHandlerFactory skillCastPostHandlerFactory;
        
        public ISkillData CreateSkill(IBattleEntityObject caster, int skillId, ITargetSelectStrategy targetSelectStrategy)
        {
            var skillContext = CreateContext(caster, skillId);
            skillContext.TargetSelectStrategy = targetSelectStrategy;
            
            // 创建技能对象
            ISkill skill = DIContainer.Create<Skill>(parameterValues: skillContext);
            SkillNodeBuildPipeline.SetSkill(skill);
            
            var buildData = CreateSKillBuildData(skillId);
            skillContext.SkillCastPostHandler = buildData.SkillCastPostHandler;
            skill.SetEffects(buildData.Effects);
            
            return new SkillData(skill);
        }
        
        private SkillContext CreateContext(IBattleEntityObject caster, int skillId)
        {
            // 从二进制配置管理器加载技能配置信息
            var skillInfo = binaryDataManager.GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
            // 获取释放者的属性组件
            var propertyComponent = caster.GetComponent<PropertyComponent>();
            return new SkillContext(caster, skillInfo, propertyComponent);
        }

        /// <summary>
        /// 创建技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        protected abstract SKillBuildData CreateSKillBuildData(int skillId);
    }
}

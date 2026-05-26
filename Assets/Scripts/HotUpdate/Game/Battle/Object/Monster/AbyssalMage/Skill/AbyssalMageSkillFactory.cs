using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.Factory;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Handler;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Core;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 深渊法师技能工厂
    /// </summary>
    public class AbyssalMageSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 103:
                    var handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var frostfallSkill = DIContainer.Create<FrostfallSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(frostfallSkill, handler);
                case 104:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var ashfallSkill = DIContainer.Create<AshfallSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(ashfallSkill, handler);
                case 105:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    var abyssGiftSkill = DIContainer.Create<AbyssGiftSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(abyssGiftSkill, handler);
                case 106:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    var abyssLockSkill = DIContainer.Create<AbyssLockSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(abyssLockSkill, handler);
                default:
                    return null;
            }
        }
    }
}

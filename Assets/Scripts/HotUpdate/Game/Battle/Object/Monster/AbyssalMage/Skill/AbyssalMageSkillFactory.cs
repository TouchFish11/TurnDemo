using Core.DI;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Handler;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 深渊法师技能工厂
    /// </summary>
    public class AbyssalMageSkillFactory : SkillFactory
    {
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            return skillId switch
            {
                103 or 104 => skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>(),
                105 or 106 => skillCastPostHandlerFactory.GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>(),
                _ => null
            };
        }
    }
}

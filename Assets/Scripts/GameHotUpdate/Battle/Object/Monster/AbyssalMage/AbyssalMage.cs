using Core.Log;
using Game.Battle.Context;
using Game.Battle.Skill.Component;
using GameHotUpdate.Battle.Object.Monster.AbyssalMage.Skill;

namespace GameHotUpdate.Battle.Object.Monster.AbyssalMage
{
    /// <summary>
    /// 深渊法师
    /// </summary>
    public class AbyssalMage : MonsterObject
    {
        private int currrentIndex;
        
        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);
            
            GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new AbyssalMageSkillFactory());
        }
        
        protected override int SelectSkill()
        {
            // 随机从技能列表中选择一个技能ID
            var skillIds = this.GetComponent<SkillComponent>().GetSkillIds();
            LogManager.Log($"技能数量：{skillIds.Count}");
            
            
            var skillId = skillIds[currrentIndex];
            LogManager.Log($"当选择的技能ID：{skillId}");
            
            ++currrentIndex;
            if (currrentIndex >= skillIds.Count)
            {
                currrentIndex = 0;
            }

            return skillId;
        }
    }
}

using HotUpdate.Battle.Object.Monster.AbyssalMage.Skill;
using HotUpdate.Battle.Skill.Component;
using HotUpdate.Core.Battle;

namespace HotUpdate.Battle.Object.Monster.AbyssalMage
{
    /// <summary>
    /// 深渊法师
    /// </summary>
    public class AbyssalMage : MonsterObject
    {
        private int rowIndex;
        private int colIndex;

        private readonly int[][] skillIdGroups =
        {
            new []{105, 103},
            new []{106, 104},
        };
        
        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);
            
            GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new AbyssalMageSkillFactory());
        }
        
        public override int SelectSkill()
        {
            var ids = skillIdGroups[rowIndex];
            
            var skillId = ids[colIndex];
            ++colIndex;
            if (colIndex == ids.Length)
            {
                colIndex = 0;
                ++rowIndex;
                if (rowIndex == skillIdGroups.Length)
                {
                    rowIndex = 0;
                }
            }
            
            return skillId;
        }
    }
}

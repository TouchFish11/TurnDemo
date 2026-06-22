using Core.DI;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.TargetSelect.Strategys;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage
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

        protected override ISkillFactory GetSkillFactory()
        {
            return DIContainer.Create<AbyssalMageSkillFactory>();
        }

        protected override ICastSkillCondition GetSkillCondition()
        {
            return castSkillConditionFactory.GetCastSkillCondition<MonsterDefaultCastSkillCondition>();
        }

        protected override ITargetSelectStrategy GetTargetSelectStrategy()
        {
            return targetSelectStrategyFactory.GetTargetSelectStrategy<MonsterBaseTargetSelectStrategy>();
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
